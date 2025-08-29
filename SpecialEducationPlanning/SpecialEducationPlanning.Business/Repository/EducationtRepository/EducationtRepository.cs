using Koa.Domain.Search.Page;
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
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
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
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.AiepSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class AiepRepository : BaseRepository<Aiep>, IAiepRepository
    {
        private readonly IObjectMapper mapper;
        private readonly ILogger<AiepRepository> logger;

        private readonly IDbContextAccessor dbContextAccessor;

        public AiepRepository(ILogger<AiepRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.mapper = mapper;
            this.dbContextAccessor = dbContextAccessor;
            this.logger = logger;
        }
        public async Task<RepositoryResponse<bool>> CheckBuilderInAiepAsync(int AiepId, int builderId)
        {
            logger.LogDebug("AiepRepository called CheckBuilderInAiepAsync");

            var spec = new AiepByBuilderIdSpecification(builderId, AiepId);
            var result = await repository.Where(spec).IgnoreQueryFilters().AnyAsync();

            logger.LogDebug("AiepRepository end call CheckBuilderInAiepAsync -> return Repository response Bool");

            return new RepositoryResponse<bool>(result);
        }

        public async Task<RepositoryResponse<IEnumerable<int>>> GetAllIdsIgnoreAcl()
        {
            logger.LogDebug("AiepRepository called GetAllIdsIgnoreAcl");

            var AiepIds = await repository.GetAll<Aiep>().IgnoreQueryFilters().Select(d => d.Id).ToListAsync();

            logger.LogDebug("AiepRepository end call GetAllIdsIgnoreAcl -> return Repository response List of Int Aiep Ids");

            return new RepositoryResponse<IEnumerable<int>>(AiepIds);
        }

        public async Task<RepositoryResponse<ICollection<ProjectModel>>> GetAiepProjectsAsync(int AiepId)
        {
            logger.LogDebug("AiepRepository called GetAiepProjectAsync");

            var spec = Specification<Aiep>.True;
            spec = spec.And(new EntityByIdSpecification<Aiep>(AiepId));
            var Aiep = await repository.Where(spec).Include(x => x.Projects).FirstOrDefaultAsync();

            if (Aiep.IsNull())
            {
                var errors = new Collection<string>
                {
                    ErrorCode.EntityNotFound.GetDescription()
                };

                logger.LogDebug("AiepRepository end call GetAiepProjectAsync -> return Repository response Errors Entity not found");

                return new RepositoryResponse<ICollection<ProjectModel>>(errors);
            }

            var result = mapper.Map<Aiep, AiepModel>(Aiep).Projects;

            logger.LogDebug("AiepRepository end call GetAiepProjectAsync -> return RepositoryResponse Collection of Project model");

            return new RepositoryResponse<ICollection<ProjectModel>>(result);
        }

        public async Task<RepositoryResponse<bool>> UpdateAllAiepAclAsync()
        {
            logger.LogDebug("AiepRepository called UpdateAllAiepAclAsync");

            var context = dbContextAccessor.GetCurrentContext();
            var timeout = context.Database.GetCommandTimeout();
            logger.LogInformation("AiepRepository UpdateAllAiepAclAsync timeout -> {timeout}", timeout);

            try
            {
                UnitOfWork.BeginTransaction();
                var AiepEntityList = await repository.GetAll<Aiep>().IgnoreQueryFilters().ToListAsync();

                foreach (var AiepEntity in AiepEntityList)
                {
                    if (!dbContextAccessor.GetCurrentContext().AiepUpdateAcl(AiepEntity.Id))
                    {
                        UnitOfWork.Rollback();

                        logger.LogDebug("AiepRepository end call UpdateAllAiepAclAsync -> return Repository response Errors Generic controller");

                        return new RepositoryResponse<bool>(false, ErrorCode.GenericControllerError,
                            "repository.UpdateAiepAclAsync");
                    }

                    logger.LogDebug("AiepRepository UpdateAllAiepAclAsync Commit");

                    UnitOfWork.Commit();
                }
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("AiepRepository end call UpdateAllAiepAclAsync -> return Repository response Errors Commit repository");

                return new RepositoryResponse<bool>(false, ErrorCode.CommitRepositoryError, e.Message);
            }

            logger.LogDebug("AiepRepository end call UpdateAllAiepAclAsync -> return Repository response Bool");

            return new RepositoryResponse<bool>(true);
        }

        public async Task<RepositoryResponse<bool>> UpdateAiepAclAsync(int AiepId)
        {
            logger.LogDebug("AiepRepository called UpdateAiepAclAsync");

            try
            {
                UnitOfWork.BeginTransaction();
                var AiepEntity = await base.FindOneAsync<Aiep>(AiepId);

                if (AiepEntity == null)
                {
                    logger.LogDebug("AiepRepository end call UpdateAiepAclAsync -> return Repository response Errors Entity not found");

                    return new RepositoryResponse<bool>(false, ErrorCode.EntityNotFound);
                }

                if (dbContextAccessor.GetCurrentContext().AiepUpdateAcl(AiepId))
                {
                    logger.LogDebug("AiepRepository UpdateAiepAclAsync Commit");

                    UnitOfWork.Commit();

                    logger.LogDebug("AiepRepository UpdateAiepAclAsync -> return Repository response Bool");

                    return new RepositoryResponse<bool>(true);
                }

                UnitOfWork.Rollback();

                logger.LogDebug("AiepRepository end call UpdateAiepAclAsync -> return Repository response Errors Generic controller");

                return new RepositoryResponse<bool>(false, ErrorCode.GenericControllerError,
                    "repository.UpdateAiepAclAsync");
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("AiepRepository end call UpdateAiepAclAsync -> return Repository response Errors Commit response");

                return new RepositoryResponse<bool>(false, ErrorCode.CommitRepositoryError,
                    e.Message);
            }
        }

        public async Task<RepositoryResponse<bool>> UpdateAiepAclByAiepCodeAsync(string AiepCode)
        {
            logger.LogDebug("AiepRepository called UpdateAiepAclByAiepCodeAsync");

            var Aiep = await GetAiepByCode(AiepCode);

            if (Aiep.Content is null)
            {
                logger.LogDebug("AiepRepository end call UpdateAiepAclByAiepCodeAsync -> return Repository response Errors Entity not found");
                return  new RepositoryResponse<bool>(false, ErrorCode.EntityNotFound); ;
            }

            int AiepId = Aiep.Content.Id;

            logger.LogDebug("UpdateAiepAclByAiepCodeAsync calls UpdateAiepAclAsync");
            return await UpdateAiepAclAsync(AiepId);
        }

        public async Task<RepositoryResponse<AiepModel>> GetAiepByCode(string AiepCode, bool detached = false)
        {
            logger.LogDebug("AiepRepository called GetAiepByCode");

            var spec = Specification<Aiep>.True;
            spec = spec.And(new AiepByKeyNameSpecification(AiepCode));
            Aiep Aiep = await repository.Where(spec).FirstOrDefaultAsync();

            if (Aiep == null)
            {
                logger.LogDebug("AiepRepository end call GetAiepByCode -> return Repository response Errors Entity not found");

                return await Task.FromResult(new RepositoryResponse<AiepModel>(null, ErrorCode.EntityNotFound));
            }

            logger.LogDebug("AiepRepository end call GetAiepByCode -> return Repository response Aiep model");

            if (detached) {
                await base.DetachEntity(Aiep);
            }

            return await Task.FromResult(new RepositoryResponse<AiepModel>(mapper.Map<Aiep, AiepModel>(Aiep)));

        }

        public async Task<RepositoryResponse<IPagedQueryResult<AiepModel>>> GetAiepsFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("AiepRepository called GetAiepsFilteredAsync");

            var spec = Specification<Aiep>.True;

            var modelSpec = SpecificationBuilder.Create<AiepModel>(searchModel.Filters);

            var query = new AiepMaterializedAiepModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("AiepRepository end call GetAiepsFilteredAsync -> return Repository response Paged query Aiep model");

            return new RepositoryResponse<IPagedQueryResult<AiepModel>>(result);
        }

        public async Task<RepositoryResponse<AiepAreaModel>> GetAllAiepsByAreaAsync(int areaId)
        {
            logger.LogDebug("AiepRepository called GetAllAiepsByAreaAsync");

            var area = await base.FindOneAsync<Area>(areaId);
            if (area.IsNull())
            {
                logger.LogError("{type}#{role} not found", typeof(AreaModel), areaId);

                logger.LogDebug("AiepRepository end call GetAllAiepsByAreaAsync -> return Repository response Errors Entity not found");

                return new RepositoryResponse<AiepAreaModel>(null, ErrorCode.EntityNotFound, "Area not Found");
            }

            var Aieps = await repository.GetAll<Aiep>().ToListAsync();
            var AiepsModel = mapper.Map<IEnumerable<Aiep>, IEnumerable<AiepModel>>(Aieps);

            var AiepsAssigned = AiepsModel.Where(x => x.AreaId == areaId);

            logger.LogDebug("AiepRepository end call GetAllAiepsByAreaAsync -> return Repository response Aiep area model");

            return new RepositoryResponse<AiepAreaModel>(new AiepAreaModel
            {
                AiepsAssigned = AiepsAssigned,
                AiepsAvailables = AiepsModel.Except(AiepsAssigned)
            });
        }

        // Create or Update Aiep Core
        public async Task<AiepModel> CreateUpdateAiepAsync(AiepModel AiepModel)
        {
            logger.LogDebug("AiepRepository called CreateUpdateAiepAsync");

            UnitOfWork.BeginTransaction();
            var entity = await base.Repository.CreateOrUpdateAiep(AiepModel, mapper, logger);
            if (AiepModel.Id == 0)
            {
                repository.Add(entity);
            }

            logger.LogDebug("AiepRepository CreateUpdateAiepAsync Commit");

            UnitOfWork.Commit();

            var model = mapper.Map<Aiep, AiepModel>(entity);

            logger.LogDebug("AiepRepository end call CreateUpdateAiepAsync -> return Aiep model");

            return model;
        }

        public async Task<RepositoryResponse<AiepModel>> GetAiepByIdIgnoreAclAsync(int AiepId)
        {
            logger.LogDebug("AiepRepository called GetAiepByIdIgnoreAclAsync");

            AiepByIdSpecification spec = new(AiepId);
            Aiep Aiep = await this.repository.Where(spec).IgnoreQueryFilters().FirstOrDefaultAsync();

            if (Aiep is null)
            {
                logger.LogDebug("AiepRepository end call GetAiepByIdIgnoreAclAsync -> return Repository response Errors Entity not found");

                return new RepositoryResponse<AiepModel>(null, ErrorCode.EntityNotFound);
            }

            var AiepsModel = mapper.Map<Aiep, AiepModel>(Aiep);

            logger.LogDebug("AiepRepository end call. Found Aiep with Id {AiepId}", AiepId);

            return new RepositoryResponse<AiepModel>()
            {
                Content = AiepsModel
            };
        }
    }
}
