using Koa.Domain;
using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Domain.Specification.EndUserSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class EndUserRepository : BaseRepository<EndUser>, IEndUserRepository
    {
        private readonly IDbContextAccessor dbContextAccessor;
        private readonly IAzureSearchManagementService azureSearchManagementService;
        private readonly ILogger<EndUserRepository> logger;
        private readonly IObjectMapper mapper;


        public EndUserRepository(ILogger<EndUserRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, IAzureSearchManagementService azureSearchManagementService, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.dbContextAccessor = dbContextAccessor;
            this.azureSearchManagementService = azureSearchManagementService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<EndUserModel>> GetEndUserById(int id)
        {
            var spec = new FindExistingEndUserById(id);
            var endUser = await base.Repository.Where<EndUser>(spec).IgnoreQueryFilters().FirstOrDefaultAsync();

            if (endUser.IsNull())
            {
                return new RepositoryResponse<EndUserModel>(null, ErrorCode.EntityNotFound, "End User not found");
            }

            var endUserModel = mapper.Map<EndUser, EndUserModel>(endUser);
            return new RepositoryResponse<EndUserModel>(endUserModel);
        }

        public IEnumerable<EndUserDiffModel> CompareEndUsers(EndUserModel source, EndUserModel target)
        {
            logger.LogDebug("EndUserRepository called CompareEndUsers");

            var differencesList = new Collection<EndUserDiffModel>();
            var properties = typeof(EndUserModel).GetProperties().Where(property => !property.PropertyType.GetInterfaces().Contains(typeof(IModel)));
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source)?.ToString();
                var targetValue = property.GetValue(target)?.ToString();

                if (sourceValue != targetValue)
                {
                    differencesList.Add(new EndUserDiffModel(property.Name, sourceValue, targetValue));
                }
            }

            logger.LogDebug("EndUserRepository end call CompareEndUsers -> return Collection of End user diff model");

            return differencesList;
        }

        public async Task<RepositoryResponse<EndUserModel>> FindExistingEndUser(EndUserModel endUser)
        {
            logger.LogDebug("EndUserRepository called FindExistingEndUser");

            if (endUser != null)
            {
                var spec = new FindExistingEndUserByMandatoryFieldsSpecification(endUser.Surname, endUser.Postcode, endUser.Address1);
                var entity = await base.Repository.Where(spec).Include(x => x.Plans).FirstOrDefaultAsync();

                if (entity != null)
                {
                    var planModels = mapper.Map<Plan, PlanModel>(entity.Plans);
                    var endUserModel = mapper.Map<EndUser, EndUserModel>(entity);
                    endUserModel.Plans = planModels.ToList();

                    logger.LogDebug("EndUserRepository end call FindExistingEndUser -> return Repository response End user model");

                    return new RepositoryResponse<EndUserModel>(endUserModel);
                }
            }
            var errors = new Collection<string>
            {
                ErrorCode.EntityNotFound.GetDescription()
            };

            logger.LogDebug("EndUserRepository end call FindExistingEndUser -> return Repository response Errors");

            return new RepositoryResponse<EndUserModel>(errors);
        }

        //public async Task<RepositoryResponse<EndUserModel>> GetEndUserPerAiep(EndUserModel endUserModel, int AiepId)
        //{
        //    var endUserEntityInput = Mapper.Map<EndUserModel, EndUser>(endUserModel);
        //    var endUserEntity = await repository.GetEndUserByMandatoryFieldsAsync(endUserEntityInput);

        //    //If not exists return an empty element with a negative ID
        //    if (endUserEntity == null)
        //    {
        //        return new RepositoryResponse<EndUserModel>(new EndUserModel() { Id = int.MinValue });   // null,  bb   ; //"End User not found");
        //    }

        //    //If exists, check Aiep
        //    var AiepFromUser = await repository.GetAiepFromEndUserAsync(endUserModel.Id, AiepId);
        //    if(AiepFromUser == null)
        //    {
        //        return new RepositoryResponse<EndUserModel>(endUserEntity, null()
        //    }




        //    if (!await repository.AssignEndUserToAiepAsync(endUserEntity.Id, AiepId))
        //    {
        //        return new RepositoryResponse<EndUserModel>(null, ErrorCode.EntityNotFound, "Aiep not found");
        //    }

        //    var endUserModelModified = Mapper.Map<EndUser, EndUserModel>(endUserEntity);
        //    return new RepositoryResponse<EndUserModel>(endUserModelModified);
        //}



        public async Task<RepositoryResponse<EndUserModel>> GetOrCreateEndUserAssignAiep(EndUserModel endUserModel, int AiepId)
        {
            logger.LogDebug("EndUserRepository called GetOrCreateEndUserAssignAiep");

            var endUserEntityInput = mapper.Map<EndUserModel, EndUser>(endUserModel);
            UnitOfWork.BeginTransaction();
            var endUserEntity = await base.Repository.GetOrCreateEndUserByMandatoryFieldsAsync(endUserEntityInput, logger);
            if (endUserEntity == null)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("EndUserRepository end call GetOrCreateEndUserAssignAiep -> return Repository response End user model Create");

                return new RepositoryResponse<EndUserModel>(new EndUserModel() { Id = int.MinValue });// null,  bb   ; //"End User not found");
            }

            logger.LogDebug("EndUserRepository GetOrCreateEndUserAssignAiep Commit");

            UnitOfWork.Commit();

            if (!await base.Repository.AssignEndUserToAiepAsync(endUserEntity.Id, AiepId, logger))
            {
                UnitOfWork.Rollback();

                logger.LogDebug("EndUserRepository end call GetOrCreateEndUserAssignAiep -> return Repository response Errors Entity not found");

                return new RepositoryResponse<EndUserModel>(null, ErrorCode.EntityNotFound, "Aiep not found");
            }

            logger.LogDebug("EndUserRepository GetOrCreateEndUserAssignAiep Second Commit");

            UnitOfWork.Commit();

            var endUserModelModified = mapper.Map<EndUser, EndUserModel>(endUserEntity);

            logger.LogDebug("EndUserRepository end call GetOrCreateEndUserAssignAiep -> return Repository response End user model");

            return new RepositoryResponse<EndUserModel>(endUserModelModified);
        }

        public async Task<RepositoryResponse<AiepModel>> GetEndUserLatestUserAiepAsync(int endUserId)
        {
            logger.LogDebug("EndUserRepository called GetEndUserLatestAiepAsync");

            var AiepEntity = await base.Repository.GetEndUserLatestUserAiepAsync(endUserId, logger);
            if (AiepEntity == null)
            {
                logger.LogDebug("EndUserRepository end call GetEndUserLatestAiepAsync -> return Repository response Aiep model Null");

                return new RepositoryResponse<AiepModel>(content: null);
            }
            var AiepModel = mapper.Map<Aiep, AiepModel>(AiepEntity);

            logger.LogDebug("EndUserRepository end call GetEndUserLatestAiepAsync -> return Repository response Aiep model");

            return new RepositoryResponse<AiepModel>(AiepModel);
        }

        public async Task<RepositoryResponse<AiepModel>> GetEndUserOwnOrLatestUserAiepAsync(int endUserId, int AiepId)
        {
            logger.LogDebug("EndUserRepository called GetEndUserOwnOrLatestUserAiepAsync");

            var onMyAiepEntity = await base.Repository.GetEndUserExistsInAiepAsync(endUserId, AiepId, logger);
            AiepModel AiepResponse = null;
            if (onMyAiepEntity != null)
            {
                AiepResponse = mapper.Map<Aiep, AiepModel>(onMyAiepEntity);

                logger.LogDebug("EndUserRepository end call GetEndUserOwnOrLatestUserAiepAsync -> return Repository response Aiep model");

                return new RepositoryResponse<AiepModel>(AiepResponse);
            }

            var onLastAiepEntity = await base.Repository.GetEndUserLatestUserAiepAsync(endUserId, logger);
            if (onLastAiepEntity != null)
            {
                AiepResponse = mapper.Map<Aiep, AiepModel>(onLastAiepEntity);

                logger.LogDebug("EndUserRepository end call GetEndUserOwnOrLatestUserAiepAsync -> return Repository response Aiep model");

                return new RepositoryResponse<AiepModel>(AiepResponse);
            }

            logger.LogDebug("EndUserRepository end call GetEndUserOwnOrLatestUserAiepAsync -> return Repository response Aiep model Null content");

            return new RepositoryResponse<AiepModel>(content: null);
        }

        public async Task<RepositoryResponse<EndUserModel>> GetEndUserByMandatoryFieldsAsync(EndUserModel endUser)
        {
            logger.LogDebug("EndUserRepository called GetEndUserByMandatoryFieldsAsync");

            var mapEndUser = mapper.Map<EndUserModel, EndUser>(endUser);
            var response = await base.Repository.GetEndUserByMandatoryFieldsAsync(mapEndUser, logger);
            if (response == null)
            {
                logger.LogDebug("EndUserRepository end call GetEndUserByMandatoryFieldsAsync -> return Repository response End user model Null content");

                return new RepositoryResponse<EndUserModel>() { Content = null };
            }
            var responseMapedModel = mapper.Map<EndUser, EndUserModel>(response);

            logger.LogDebug("EndUserRepository end call GetEndUserByMandatoryFieldsAsync -> return Repository response End user model");

            return new RepositoryResponse<EndUserModel>(responseMapedModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<RepositoryResponseGeneric> EndUserCleanManagment()
        {
            logger.LogDebug("EndUserRepository called EndUserCleanManagement");

            UnitOfWork.BeginTransaction();

            var response = new RepositoryResponseGeneric();
            // Alternatively from checking the UpdateDate we can bring all plans to bring all enduser related which UpdateDate <= 2 years
            var spec = new FindExistingEndUserByModificationDateAfter2YearSpecification();
            var endUser = await base.Repository.Where(spec).IgnoreQueryFilters().ToListAsync();
            if (!endUser.Any())
            {
                response.AddError(ErrorCode.GenericBusinessError, "Invalid EndUser");

                logger.LogDebug("EndUserRepository end call EndUserCleanManagement -> return Repository response generic Errors Generic Business");

                return response;
            }

            foreach (EndUser user in endUser)
            {
                user.Address0 = "******";
                user.Address1 = Convert.ToString(DateTime.UtcNow.Ticks);
                user.Address2 = "******";
                user.Address3 = "******";
                user.Address4 = "******";
                user.Address5 = "******";
                user.Comment = "******";
                user.ContactEmail = "******";
                user.CountryCode = 0;
                user.FirstName = "******";
                user.LandLineNumber = "******";
                user.MobileNumber = "******";
                user.Postcode = "N/P";
                user.Surname = "******";
                user.TitleId = 1;
                user.UniqueIdentifier = "******";
                user.CreationUser = "******";
                user.UpdateUser = "******";
                user.FullName = "******";
            }

            logger.LogDebug("EndUserRepository EndUserCleanManagement Commit");

            UnitOfWork.Commit();

            logger.LogDebug("EndUserRepository end call EndUserCleanManagement -> return Repository response generic");

            return response;
        }

        public async Task CallIndexerAsync(int take, int skip, DateTime? updateDate, int? indexerWindowInDays)
        {
            logger.LogDebug("EndUserRepository called CallIndexerAsync");

            var endUsers = await base.Repository.GetEntitiesNoFiltersAsync<EndUser>(take, skip, updateDate, indexerWindowInDays).ToListAsync();

            azureSearchManagementService.MergeOrUploadDocuments
                (azureSearchManagementService.GetDocuments<OmniSearchEndUserIndexModel, EndUser>(endUsers));

            logger.LogDebug("EndUserRepository end call CallIndexerAsync");
        }
    }
}
