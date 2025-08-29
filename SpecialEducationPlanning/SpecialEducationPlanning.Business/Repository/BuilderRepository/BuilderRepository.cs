using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.DtoModel.BuilderSapSearch;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications;
using SpecialEducationPlanning
.Domain.Specification.CustomerSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class BuilderRepository : BaseRepository<Builder>, IBuilderRepository
    {
        private readonly IEfUnitOfWork unitOfWork;
        private readonly ILogger<BuilderRepository> logger;
        private readonly IObjectMapper mapper;

        //private readonly IDbContextAccessor dbContextAccessor;
        private readonly IAzureSearchManagementService azureSearchManagementService;

        public BuilderRepository(ILogger<BuilderRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, IAzureSearchManagementService azureSearchManagementService, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
            // this.dbContextAccessor = dbContextAccessor;
            this.azureSearchManagementService = azureSearchManagementService;
        }

        // TODO Finish method
        public async Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetAiepBuilders(int customerId)
        {
            logger.LogDebug("BuilderRepository called GetAiepBuilders -> return Null");

            /*
            var spec = Specification<Customer>.True;
            spec = spec.And(new CustomerByIdSpecification(customerId));
            var customer = repository.Where(spec).Include(x => x.GroupUsers).Select(gu=>gu.GroupUsers).ToList();
            var result = customer != null ? mapper.Map<Customer, CustomerModel>(customer.Result).AiepCustomers : null;*/
            return null;
        }

        // TODO Finish method
        public async Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetProjectBuilders(int builderId)
        {
            logger.LogDebug("BuilderRepository called GetProjectBuilders -> return Null");

            /* var spec = Specification<Customer>.True;
             spec = spec.And(new CustomerByIdSpecification(customerId));
             var customer = repository.Where(spec).Include(x => x.ProjectCustomers).FirstOrDefaultAsync();
             var result = customer != null ? mapper.Map<Customer, CustomerModel>(customer.Result).ProjectCustomers : null;*/
            return null;
        }

        public async Task<Builder> AddBuilder(Builder entity)
        {
            return await base.Add(entity);
        }

            public async Task<RepositoryResponse<BuilderModel>> GetExistingBuilderAsync(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called GetExistingBuilderAsync");

            var response = await IsValidBuilder(builderModel);
            if (!response.ErrorList.Any())
            {
                var builderSpec = new IsExistingBuilderSpec(builderModel.TradingName, builderModel.Postcode, builderModel.Address1, builderModel.AccountNumber);

                var builder = await repository.Where(builderSpec).SingleOrDefaultAsync();
                if (builder.IsNotNull())
                {
                    response.Content = mapper.Map<Builder, BuilderModel>(builder);

                    logger.LogDebug("BuilderRepository end call GetExistingBuilderAsync -> return Repository response Builder model");

                    return response;
                }
                response.Content = builderModel;
            }

            logger.LogDebug("BuilderRepository end call GetExistingBuilderAsync -> return Repository response Errors");

            return response;
        }

        public async Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetBuildersByIdsAsync(IEnumerable<int> ids, int skip, int take, int AiepId, SortDescriptor azureSort)
        {
            logger.LogDebug("BuilderRepository called GetBuildersByIdsAsync");

            ISpecification<Builder> spec = new Specification<Builder>(b => ids.Contains(b.Id));

            if (AiepId != -1)
            {
                spec = spec.And(new Specification<Builder>(b => b.BuilderEducationerAieps.Any(bdd => bdd.AiepId == AiepId)));
            }

            List<Builder> builders;

            var builderQueryable = repository.Where(spec).Include(b => b.BuilderEducationerAieps).Take(take);

            if (azureSort.IsNull())
            {
                builders = await builderQueryable.OrderByDescending(b => b.UpdatedDate).ToListAsync();
            }
            else
            {
                MethodInfo order = System.Linq.QueryableExtensions.OrderByMethod;

                if (azureSort.Direction == SortDirection.Descending)
                {
                    order = System.Linq.QueryableExtensions.OrderByDescendingMethod;
                }

                builders = await builderQueryable.OrderByPropertyName(order, azureSort.Member).ToListAsync();
            }
            var builderModels = mapper.Map<IList<Builder>, IList<BuilderModel>>(builders, new List<BuilderModel>());

            logger.LogDebug("BuilderRepository end call GetBuildersByIdsAsync -> return Repository response List of Builder model");

            return new RepositoryResponse<IEnumerable<BuilderModel>>(builderModels);
        }


        public async Task<RepositoryResponse<BuilderModel>> GetExistingBuilderOrEmptyAsync(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called GetExistingBuilderOrEmptyAsync");

            var response = await IsValidBuilder(builderModel);
            if (!response.ErrorList.Any())
            {
                var builderSpec = new IsExistingBuilderSpec(builderModel.TradingName, builderModel.Postcode, builderModel.Address1, builderModel.AccountNumber);

                var builder = await repository.Where(builderSpec).IgnoreQueryFilters().SingleOrDefaultAsync();
                if (builder.IsNotNull())
                {
                    response.Content = mapper.Map<Builder, BuilderModel>(builder);

                    logger.LogDebug("BuilderRepository end call GetExistingBuilderOrEmptyAsync -> return Repository response Builder model");

                    return response;
                }
                response.Content = null;
            }

            logger.LogDebug("BuilderRepository end call GetExistingBuilderOrEmptyAsync -> return Repository response Errors or Null");

            return response;
        }

        public async Task<RepositoryResponse<ValidationBuilderModel>> GetPosibleTdpMatchingBuilders(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called GetPosibleTdpMatchingBuilder");

            var response = new RepositoryResponse<ValidationBuilderModel>(new ValidationBuilderModel()
            {
                Type = BuilderMatchType.NotExact,
                Builders = new List<BuilderSapSearch>()
            });

            var builder = mapper.Map<BuilderModel, Builder>(builderModel);
            var exactBuilder = await base.Repository.GetExistingBuilderOnlyMandatoryFieldsAsync(builder, logger);
            if (exactBuilder != null && exactBuilder.Id != builderModel.Id)
            {
                var exactBuilderModel = mapper.Map<Builder, BuilderModel>(exactBuilder);
                response.Content = new ValidationBuilderModel()
                {
                    Type = BuilderMatchType.Exact,
                    Builders = new List<BuilderSapSearch>() { new BuilderSapSearch() { Builder = exactBuilderModel, BuilderSearchType = exactBuilder.AccountNumber.IsNotNull() ? BuilderSearchTypeEnum.Credit : BuilderSearchTypeEnum.Cash } }
                };

                logger.LogDebug("BuilderRepository end call GetPosibleTdpMatchingBuilder -> return Repository response ValidationBuilderModel Exact match");

                return response;
            }
            else
            {
                var buildersList = await GetPosibleBuildersMatch(builderModel);

                if (buildersList.Content.Any())
                {
                    response.Content = new ValidationBuilderModel()
                    {
                        Type = BuilderMatchType.NotExact,
                        Builders = buildersList.Content.Where(b => b.Id != builderModel.Id).Select(b => new BuilderSapSearch() { Builder = b, BuilderSearchType = b.AccountNumber.IsNotNull() ? BuilderSearchTypeEnum.Credit : BuilderSearchTypeEnum.Cash }).ToList()
                    };
                }

                logger.LogDebug("BuilderRepository end call GetPosibleTdpMatchingBuilder -> return Repository response ValidationBuilderModel Not exact match");

                return response;
            }
        }

        /// <summary>
        /// Get the builders where the postcode matches with the postcode of the given builder
        /// </summary>
        /// <param name="builderModel"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<IEnumerable<BuilderModel>>> GetPosibleBuildersMatch(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called GetPosibleBuildersMatch");

            var spec = new Specification<Builder>(x => x.Postcode == builderModel.Postcode);
            var builders = await repository.Where(spec).IgnoreQueryFilters().ToListAsync();
            var builderModels = mapper.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(builders);

            logger.LogDebug("BuilderRepository end call GetPosibleBuildersMatch -> return Repository response List of Builder model");

            return new RepositoryResponse<IEnumerable<BuilderModel>>(builderModels);
        }

        public async Task<RepositoryResponse<ValidationBuilderModel>> GetPossibleTdpMatchingBuilderByAccountNumberAsync(string accountNumber)
        {
            logger.LogDebug("BuilderRepository called GetPossibleTdpMatchingBuilderByAccountNumberAsync");

            var response = new RepositoryResponse<ValidationBuilderModel>(new ValidationBuilderModel()
            {
                Type = BuilderMatchType.Exact,
                Builders = new List<BuilderSapSearch>()
            });

            var exactBuilder = await base.Repository.GetExistingBuilderOnlyAccountNumberAsync(accountNumber, logger);
            if (exactBuilder != null)
            {
                var exactBuilderModel = mapper.Map<Builder, BuilderModel>(exactBuilder);
                response.Content = new ValidationBuilderModel()
                {
                    Type = BuilderMatchType.Exact,
                    Builders = new List<BuilderSapSearch>() { new BuilderSapSearch() { Builder = exactBuilderModel, BuilderSearchType = exactBuilderModel.AccountNumber.IsNotNull() ? BuilderSearchTypeEnum.Credit : BuilderSearchTypeEnum.Cash } }
                };

                logger.LogDebug("BuilderRepository end call GetPossibleTdpMatchingBuilderByAccountNumberAsync -> return Repository response Validation builder model Exact builder not null");

                return response;
            }

            logger.LogDebug("BuilderRepository end call GetPossibleTdpMatchingBuilderByAccountNumberAsync -> return Repository response Validation builder model Exact builder null");

            return response;
        }

        public async Task<Builder> GetExistingDbBuilderAsync(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called ExistsBuilderInDBAsync");

            var exactBuilder = await base.Repository.GetExistingBuilderOnlyAccountNumberAsync(builderModel.AccountNumber, logger);
            if (exactBuilder != null)
            {
                logger.LogDebug("BuilderRepository end call ExistsBuilderInDBAsync -> return Builder");
                return exactBuilder;
            }

            logger.LogDebug("BuilderRepository end call ExistsBuilderInDBAsync -> return NULL");

            return null;
        }

        public async Task<RepositoryResponse<BuilderModel>> ValidateEducationerAiep(int EducationerId, int AiepId)
        {
            logger.LogDebug("BuilderRepository called ValidateEducationerAiep");

            //TODO: Waiting for FrontEnd EducationerId and AiepId knowledge so we can apply all the logic below

            //var Educationer = await repository.FindOneAsync<Educationer>(EducationerId);
            var Educationer = await base.FindOneAsync<Builder>(1);
            if (Educationer.IsNull())
            {
                logger.LogDebug("BuilderRepository end call ValidateEducationerAiep -> return Repository response Errors Entity not found Educationer");

                return new RepositoryResponse<BuilderModel>(ErrorCode.EntityNotFound.GetDescription());
            }
            //var Aiep = await repository.FindOneAsync<Aiep>(AiepId);
            var Aiep = await base.FindOneAsync<Aiep>(3);
            if (Aiep.IsNull())
            {
                logger.LogDebug("BuilderRepository end call ValidateEducationerAiep -> return Repository response Errors Entity not found Aiep");

                return new RepositoryResponse<BuilderModel>(ErrorCode.EntityNotFound.GetDescription());
            }
            logger.LogDebug("BuilderRepository end call ValidateEducationerAiep -> return Repository response Builder model");

            return new RepositoryResponse<BuilderModel>();
        }

        public async Task<RepositoryResponse<BuilderModel>> CreateAsync(BuilderModel builderModel, int EducationerId, int AiepId)
        {
            logger.LogDebug("BuilderRepository called CreateAsync");

            unitOfWork.BeginTransaction();
            var builderEntity = mapper.Map(builderModel, new Builder());
            var Educationer = await base.FindOneAsync<User>(EducationerId);
            var Aiep = await base.FindOneAsync<Aiep>(AiepId);
            if (Educationer == null || Aiep == null)
            {
                logger.LogDebug("BuilderRepository end call CreateAsync -> return Repository response Errors Entity not found");

                return new RepositoryResponse<BuilderModel>(ErrorCode.EntityNotFound.GetDescription());
            }

            builderEntity.BuilderEducationerAieps.Add(new BuilderEducationerAiep() { Builder = builderEntity, AiepId = AiepId });
            builderEntity = repository.Add(builderEntity);

            logger.LogDebug("BuilderRepository CreateAsync Commit");

            unitOfWork.Commit();

            logger.LogDebug("BuilderRepository end call CreateAsync -> return Repository response Builder model");

            return new RepositoryResponse<BuilderModel>
            {
                Content = mapper.Map<Builder, BuilderModel>(builderEntity)
            };
        }

        /// <summary>
        /// Deletes an Account number by Builder
        /// </summary>
        /// <param name="idBuilder"></param>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public async Task<RepositoryResponseGeneric> DeleteAccountNumberAsync(int idBuilder, string accountNumber)
        {
            logger.LogDebug("BuilderRepository called DeleteAccountNumberAsync");

            var builder = await base.FindOneAsync<Builder>(idBuilder);
            if (builder == null)
            {
                var response = new RepositoryResponseGeneric();
                response.AddError(ErrorCode.ArgumentErrorBusiness);

                logger.LogDebug("BuilderRepository end call DeleteAccountNumberAsync -> return Repository response generic Errors Argument error business Builder");

                return response;
            }
            if (builder.AccountNumber.IsNullOrEmpty() || !builder.AccountNumber.Equals(accountNumber))
            {
                var response = new RepositoryResponseGeneric();
                response.AddError(ErrorCode.ArgumentErrorBusiness, "Invalid Account number");

                logger.LogDebug("BuilderRepository end call DeleteAccountNumberAsync -> return Repository response generic Errors Argument error business Account number");

                return response;
            }

            unitOfWork.BeginTransaction();
            builder.AccountNumber = null;

            logger.LogDebug("BuilderRepository DeleteAccountNumberAsync Commit");

            unitOfWork.Commit();

            logger.LogDebug("BuilderRepository end call DeleteAccountNumberAsync -> return Repository response generic");

            return new RepositoryResponseGeneric();
        }

        public async Task<RepositoryResponse<BuilderModel>> EndUserRefreshAsync(int endUserId)
        {
            logger.LogDebug("BuilderRepository called EndUserRefreshAsync");

            RepositoryResponse<BuilderModel> repositoryResponse = new RepositoryResponse<BuilderModel>();
            var endUser = await base.FindOneAsync<EndUser>(endUserId);
            if (endUser.IsNull())
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("BuilderRepository end call EndUserRefreshAsync -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }
            //repositoryResponse = await BuilderEndUserRefreshAsync(endUser.BuilderId);

            logger.LogDebug("BuilderRepository end call EndUserRefreshAsync -> return Repository response Builder model");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<BuilderModel>> IsValidBuilder(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called IsValidBuilder");

            RepositoryResponse<BuilderModel> repositoryResponse = new RepositoryResponse<BuilderModel>();
            if (String.IsNullOrEmpty(builderModel.AccountNumber))
            {
                // CASH
                repositoryResponse.ErrorList = IsValidCashBuilder(builderModel);
            }
            else
            {
                // SAP
                repositoryResponse.ErrorList = IsValidCreditBuilder(builderModel);
            }
            repositoryResponse.Content = builderModel;

            logger.LogDebug("BuilderRepository end call IsValidBuilder -> return Repository response Builder");

            return await Task.FromResult(repositoryResponse);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idBuilder"></param>
        /// <returns></returns>
        public async Task<RepositoryResponseGeneric> BuilderCleanManagment(int idBuilder)
        {
            logger.LogDebug("BuilderRepository called BuilderCleanManagement");

            unitOfWork.BeginTransaction();
            var response = new RepositoryResponseGeneric();
            var builder = await base.FindOneAsync<Builder>(idBuilder);
            if (builder == null)
            {
                response.AddError(ErrorCode.ArgumentErrorBusiness, "Invalid Builder");

                logger.LogDebug("BuilderRepository end call BuilderCleanManagement -> return Repository response Error Argument error business");

                return response;
            }

            builder.AccountNumber = "******";
            builder.Address0 = "******";
            builder.Address1 = Convert.ToString(DateTime.UtcNow.Ticks);
            builder.Address2 = "******";
            builder.Address3 = "******";
            builder.Email = "******";
            builder.LandLineNumber = "******";
            builder.MobileNumber = "******";
            builder.Name = "******";
            builder.Notes = "******";
            builder.Postcode = "N/P";
            builder.TradingName = Convert.ToString(DateTime.UtcNow.Ticks);
            builder.Name = "******";
            builder.UniqueIdentifier = "******";
            builder.CreationUser = "******";
            builder.UpdateUser = "******";
            builder.SAPAccountStatus = "******";
            builder.OwningAiepCode = "******";
            builder.OwningAiepName = "******";
            builder.BuilderStatus = BuilderStatus.None;

            logger.LogDebug("BuilderRepository BuilderCleanManagement Commit");

            unitOfWork.Commit();

            logger.LogDebug("BuilderRepository end call BuilderCleanManagement -> return Repository response generic");

            return response;
        }

        public async Task<RepositoryResponse<IPagedQueryResult<BuilderModel>>> GetBuildersFiltered(IPageDescriptor searchModel, int? AiepId = null)
        {
            logger.LogDebug("BuilderRepository called GetBuildersFiltered");

            var spec = Specification<Builder>.True;
            if (AiepId.HasValue && AiepId.Value != -1)
            {
                spec = spec.And(new BuilderByAiepIdSpecification(AiepId.Value));
            }

            BuilderModel builderModel = new BuilderModel();
            foreach (var filter in searchModel.Filters)
            {
                if (filter.Member.ToLower() == nameof(builderModel.MobileNumber).ToLower() || filter.Member.ToLower() == nameof(builderModel.LandLineNumber).ToLower())
                {
                    filter.Value = filter.Value.NormaliseNumber();
                }

                if (filter.Member.ToLower() == nameof(builderModel.Postcode).ToLower())
                {
                    filter.Value = filter.Value.NormalisePostcode();
                }
            }

            var modelSpec = SpecificationBuilder.Create<BuilderModel>(searchModel.Filters);

            var query = new BuilderMaterializedBuilderModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            if (result.Total > 1000)
            {
                logger.LogDebug("BuilderRepository end call GetBuildersFiltered -> return Repository response Paged query Builder model Error Max take exceeded");

                return new RepositoryResponse<IPagedQueryResult<BuilderModel>>(result, ErrorCode.MaxTakeExceeded, "The maximum number of builders has been exceeded");
            }

            var repositoryResponse = new RepositoryResponse<IPagedQueryResult<BuilderModel>>
            {
                Content = result
            };

            logger.LogDebug("BuilderRepository end call GetBuildersFiltered -> return Repository response Paged query Builder model");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<ICollection<BuilderModel>>> UpdateBuildersFromSapAsync(IEnumerable<BuilderSapModel> builderModels)
        {
            logger.LogDebug("BuilderRepository called UpdateBuildersFromSapAsync");

            var response = new List<BuilderModel>();
            var errorList = new List<string>();
            foreach (var builderSapModel in builderModels)
            {
                Builder existingBuilder = null;
                builderSapModel.Postcode = builderSapModel.Postcode.NormalisePostcode();
            
                if (!string.IsNullOrEmpty(builderSapModel.AccountNumber))
                {
                    existingBuilder = await repository.Where(new BuilderByAccountNumberSpecification(builderSapModel.AccountNumber)).IgnoreQueryFilters().FirstOrDefaultAsync();
                }
                if (existingBuilder.IsNotNull())
                {
                    try
                    {
                        unitOfWork.BeginTransaction();
                        builderSapModel.Notes = existingBuilder.Notes;
                        if (string.IsNullOrEmpty(builderSapModel.Address1))
                            builderSapModel.Address1 = existingBuilder.Address1 ?? ComposeAddress0(existingBuilder) ?? "N/A";
                        mapper.Map(builderSapModel, existingBuilder);
                        existingBuilder.BuilderStatus = (builderSapModel.BuilderStatus == BuilderStatus.Deleted) ? BuilderStatus.Closed : BuilderStatus.Active;

                        logger.LogDebug("BuilderRepository UpdateBuilderFromSapAsync Commit");

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Full exception: {0}", ex.ToString());
                        logger.LogError($"Base exception: {0}", ex.GetBaseException().ToString());
                        logger.LogError($"Inner exception: {0}", ex.InnerException.ToString());
                        logger.LogError($"Error updating builder from SAP " +
                            $"Account Number: {builderSapModel.AccountNumber} " +
                            $"Trading Name: {builderSapModel.TradingName} " +
                            $"Address1: {builderSapModel.Address1} " +
                            $"Postcode: {builderSapModel.Postcode}");
                        unitOfWork.Rollback();

                        logger.LogDebug("BuilderRepository UpdateBuilderFromSapAsync exception");

                        throw;
                    }

                    bool plansTradingNameUpdated = await UpdateBuilderPlansTradingName(existingBuilder.Id, existingBuilder.TradingName);
                    if (!plansTradingNameUpdated)
                    {
                        logger.LogError("Unable to update the trading name of the plans assigned to " + existingBuilder.Name + " with ID " + existingBuilder.Id + " and trading name " + existingBuilder.TradingName);
                    }

                    response.Add(mapper.Map<Builder, BuilderModel>(existingBuilder));
                }
                else
                {
                    logger.LogDebug($"BuilderRepository UpdateBuilderFromSapAsync: Builder not found by account number");

                    errorList.Add(ErrorCode.NoResults.GetDescription());
                }
            }

            logger.LogDebug("BuilderRepository end call UpdateBuilderFromSapAsync -> return Repository response Collection of Builder model");

            return new RepositoryResponse<ICollection<BuilderModel>>(response, errorList);
        }

        public async Task<RepositoryResponse<BuilderModel>> UpdateBuilderFromSAPByAccountNumberAsync(BuilderModel SAPBuilder, BuilderModel builderTDP)
        {
            logger.LogDebug("BuilderRepository called UpdateBuilderFromSapByAccountNumberAsync");

            var builderSpec = new IsExistingBuilderSpec(builderTDP.TradingName, builderTDP.Postcode, builderTDP.Address1, builderTDP.AccountNumber);
            var tdpBuilderEntity = await repository.Where(builderSpec).SingleOrDefaultAsync();
            if (tdpBuilderEntity == null)
            {
                var otherBuilder = repository.Where(new Specification<Builder>(b => b.AccountNumber.Equals(builderTDP.AccountNumber))).FirstOrDefault();
                if (otherBuilder == null)
                {
                    logger.LogDebug("BuilderRepository end call UpdateBuilderFromSapByAccountNumberAsync -> return Repository response Errors No results");

                    return new RepositoryResponse<BuilderModel>(ErrorCode.NoResults.GetDescription());
                }

                unitOfWork.BeginTransaction();
                var Response = mapper.Map<Builder, BuilderModel>(otherBuilder, SAPBuilder);

                logger.LogDebug("BuilderRepository UpdateBuilderFromSapByAccountNumberAsync Commit");

                unitOfWork.Commit();

                logger.LogDebug("BuilderRepository end call UpdateBuilderFromSapByAccountNumberAsync -> return Repository response Builder model");

                return new RepositoryResponse<BuilderModel>(Response);
            }

            if (!tdpBuilderEntity.AccountNumber.IsNullOrEmpty())
            {
                if (tdpBuilderEntity.AccountNumber != SAPBuilder.AccountNumber)
                {
                    logger.LogDebug("BuilderRepository end call UpdateBuilderFromSapByAccountNumberAsync -> return Repository response Errors Entity not found");

                    return new RepositoryResponse<BuilderModel>(ErrorCode.EntityNotFound.GetDescription());
                }
            }

            unitOfWork.BeginTransaction();
            var builderModelResponse = mapper.Map<Builder, BuilderModel>(tdpBuilderEntity, SAPBuilder);

            logger.LogDebug("BuilderRepository UpdateBuilderFromSapByAccountNumberAsync Commit");

            unitOfWork.Commit();

            logger.LogDebug("BuilderRepository end call UpdateBuilderFromSapByAccountNumberAsync -> return Repository response Builder model");

            return new RepositoryResponse<BuilderModel>(builderModelResponse);
        }

        private string ComposeAddress0(Builder builder)
        {
            logger.LogDebug("BuilderRepository called ComposeAddress0");

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(builder.Address1).Append(" ");
            stringBuilder.Append(builder.Address2).Append(" ");
            stringBuilder.Append(builder.Address3);

            logger.LogDebug("BuilderRepository end call ComposeAddress0 -> return Address string");

            return stringBuilder.ToString();
        }

        private string ValidateMandatoryFields(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called ValidateMandatoryFields");

            if (
                    string.IsNullOrEmpty(builderModel.Address1) ||
                    string.IsNullOrEmpty(builderModel.TradingName) ||
                    string.IsNullOrEmpty(builderModel.Postcode)
                )
            {
                logger.LogDebug("BuilderRepository end call ValidateMandatoryFields -> return Argument error business");

                return ErrorCode.ArgumentErrorBusiness.GetDescription();
            }

            logger.LogDebug("BuilderRepository end call ValidateMandatoryFields -> return Null");

            return null;
        }

        private ICollection<string> IsValidCashBuilder(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called IsValidCashBuilder");

            ICollection<string> error = new Collection<string>();

            var accountNumberValidationError = !string.IsNullOrEmpty(builderModel.AccountNumber) ? ErrorCode.ArgumentErrorBusiness.GetDescription() : null;
            var mandatoryFieldsValidationError = ValidateMandatoryFields(builderModel);

            //if (!accountNumberValidationError.IsNull() || !mandatoryFieldsValidationError.IsNull()) { error.Add(accountNumberValidationError); }
            //Condition changed

            if (!accountNumberValidationError.IsNull())
            {
                logger.LogDebug("BuilderRepository IsValidCashBuilder -> Added Account number error");

                error.Add(accountNumberValidationError);
            }

            if (!mandatoryFieldsValidationError.IsNull())
            {
                logger.LogDebug("BuilderRepository IsValidCashBuilder -> Added Mandatory fields error");

                error.Add(mandatoryFieldsValidationError);
            }

            logger.LogDebug("BuilderRepository end call IsValidCashBuilder -> return List of errors");

            return error;
        }

        private ICollection<string> IsValidCreditBuilder(BuilderModel builderModel)
        {
            logger.LogDebug("BuilderRepository called IsValidCreditBuilder");

            ICollection<string> error = new Collection<string>();

            var accountNumberError = string.IsNullOrEmpty(builderModel.AccountNumber) ? ErrorCode.ArgumentErrorBusiness.GetDescription() : null;

            if (!accountNumberError.IsNull()) { error.Add(accountNumberError); }

            logger.LogDebug("BuilderRepository end call IsValidCreditBuilder -> return List of errors");

            return error;
        }



        public async Task<RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>> GetBuildersOmniSearch(string textToSearch, int take, int? AiepId = null)
        {
            logger.LogDebug("BuilderRepository called GetBuilderOmniSearch");

            int countBuilders = 0;
            IEnumerable<Builder> builderList;
            var baseSpec = Specification<Builder>.True;
            if (AiepId.HasValue && AiepId.Value != -1)
            {
                baseSpec = baseSpec.And(new BuilderByAiepIdSpecification(AiepId.Value));
            }

            if (string.IsNullOrEmpty(textToSearch))
            {
                builderList = await repository.Where<Builder>(baseSpec).OrderByDescending(x => x.UpdatedDate).Take(take).ToListAsync();
                countBuilders = builderList.Count();
            }
            else
            {
                var search = textToSearch;

                ISpecification<Builder> spec = new Specification<Builder>(x => x.Postcode.Contains(search));
                spec = spec.Or(new Specification<Builder>(x => x.Postcode.Contains(search.NormalisePostcode())));
                spec = spec.Or(new Specification<Builder>(x => x.MobileNumber.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.AccountNumber.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.Address0.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.Email.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.LandLineNumber.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.TradingName.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.Name.Contains(search)));
                spec = spec.Or(new Specification<Builder>(x => x.BuilderStatus.Equals((Enum.Parse(typeof(BuilderStatus), search)))));
                baseSpec = baseSpec.And(spec);

                builderList = await repository.GetAll<Builder>().Where(baseSpec).OrderByDescending(x => x.UpdatedDate).Take(take).ToListAsync();
                countBuilders = builderList.Count();
            }

            var builderModelList = mapper.Map<Builder, BuilderModel>(builderList).ToList();

            var builderModelListCountTuple = new Tuple<IEnumerable<BuilderModel>, int>(builderModelList, countBuilders);

            logger.LogDebug("BuilderRepository end call GetBuildersOmniSearch -> return Repository response List of Builder model");

            return new RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>(builderModelListCountTuple);
        }

        public async Task<RepositoryResponse<Builder>> GetAssignedPlansAsync(int builderId)
        {
            logger.LogDebug("BuilderRepository called GetAssignedPlansAsync");

            var builder = await this.repository.Where(new Specification<Builder>(x => x.Id == builderId)).Include(p => p.Plans).SingleOrDefaultAsync();
            if (!builder.IsNotNull())
            {
                logger.LogDebug("BuilderRepository end call GetAssignedPlansAsync -> return Repository response Errors Entity not found");

                return new RepositoryResponse<Builder>(ErrorCode.EntityNotFound.GetDescription());
            }

            logger.LogDebug("BuilderRepository end call GetAssignedPlansAsync -> return Repository response Builder");

            return new RepositoryResponse<Builder>(builder);
        }

        public RepositoryResponse<ValidationBuilderModel> MergeBuilderTdpAndSapSearch(ValidationBuilderModel tdpValidationBuilderModel, ValidationBuilderModel sapValidationBuilderModel)
        {
            logger.LogDebug("BuilderRepository called MergeBuilderTdpAndSapSearch");

            var responseTdpSapMatch = new ValidationBuilderModel()
            {
                Type = BuilderMatchType.NotExact,
                Builders = new List<BuilderSapSearch>()
            };

            if (tdpValidationBuilderModel != null && tdpValidationBuilderModel.Builders != null)
                responseTdpSapMatch.Builders.AddRange(tdpValidationBuilderModel.Builders);
            if (sapValidationBuilderModel != null && sapValidationBuilderModel.Builders != null)
            {
                foreach (var item in sapValidationBuilderModel.Builders)
                {
                    if (!responseTdpSapMatch.Builders.Any(b => b.Builder.AccountNumber == item.Builder.AccountNumber))
                    {
                        responseTdpSapMatch.Builders.Add(item);
                    }
                }
            }

            logger.LogDebug("BuilderRepository end call MergeBuilderTdpAndSapSearch -> return Repository response Validation builder model");

            return new RepositoryResponse<ValidationBuilderModel>(responseTdpSapMatch);
        }

        public async Task<RepositoryResponse<BuilderModel>> ModifyBuilderNotes(int builderId, string notes)
        {
            logger.LogDebug("BuilderRepository called ModifyBuilderNotes");

            var builder = await repository.Where(new EntityByIdSpecification<Builder>(builderId)).FirstOrDefaultAsync();

            if (builder == null)
            {
                logger.LogDebug("BuilderRepository end call ModifyBuilderNotes -> return Repository response Errors Entity not found");

                return new RepositoryResponse<BuilderModel>(null, ErrorCode.EntityNotFound, "Builder not found");
            }
            unitOfWork.BeginTransaction();

            builder.Notes = notes;

            logger.LogDebug("BuilderRepository ModifyBuilderNotes Commit");

            unitOfWork.Commit();

            var builderModel = mapper.Map<Builder, BuilderModel>(builder);

            logger.LogDebug("BuilderRepository end call ModifyBuilderNotes -> return Repository response Builder model");

            return new RepositoryResponse<BuilderModel>(builderModel);
        }

        private async Task<bool> UpdateBuilderPlansTradingName(int builderId, string builderTradingName)
        {
            logger.LogDebug("BuilderRepository called UpdateBuilderPlansTradingName");

            try
            {
                UnitOfWork.BeginTransaction();
                var planList = await this.repository.GetAll<Plan>().Where(p => p.BuilderId == builderId).IgnoreQueryFilters().ToListAsync();

                foreach (var plan in planList)
                {
                    if (builderTradingName != plan.BuilderTradingName)
                    {
                        plan.BuilderTradingName = builderTradingName;
                    }
                }

                logger.LogDebug("BuilderRepository UpdateBuilderPlansTradingName");

                UnitOfWork.Commit();

                logger.LogDebug("BuilderRepository end call UpdateBuilderPlansTradingName -> return True");

                return true;
            }
            catch (Exception)
            {
                logger.LogDebug("BuilderRepository end call UpdateBuilderPlansTradingName -> exception return False");

                return false;
            }
        }

        public async Task CallIndexerAsync(int take, int skip, DateTime? updatedDate, int? indexerWindowInDays)
        {
            logger.LogDebug("BuilderRepository called CallIndexerAsync");

            var builders = await base.Repository.GetEntitiesNoFiltersAsync<Builder>(take, skip, updatedDate, indexerWindowInDays).ToListAsync();           

            azureSearchManagementService.MergeOrUploadDocuments
                (azureSearchManagementService.GetDocuments<OmniSearchBuilderIndexModel, Builder>(builders));

            logger.LogDebug("BuilderRepository end call CallIndexerAsync");
        }

    }
}

