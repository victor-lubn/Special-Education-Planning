using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class BuilderEducationerAiepRepository : BaseRepository<BuilderEducationerAiep>, IBuilderEducationerAiepRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly IEntityRepository entityRepository;
        private readonly IEfUnitOfWork unitOfWork;
        private readonly IObjectMapper mapper;
        private readonly ILogger<BuilderEducationerAiepRepository> logger;


        public BuilderEducationerAiepRepository(ILogger<BuilderEducationerAiepRepository> logger, IEntityRepository<int> entityRepositoryKey, IEntityRepository entityRepository, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder) :
            base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.entityRepository = entityRepository;
            this.logger = logger;
        }



        public async Task<RepositoryResponse<BuilderEducationerAiepModel>> GetBuilderEducationerAiepModelRelation(int builderId, int AiepId)
        {
            logger.LogDebug("BuilderEducationerAiepRepository called GetBuilderEducationerAiepModelRelation");

            var spec = new IsExistingBuilderEducationerAiepSpec(builderId, AiepId);
            if (!(await repository.Where(spec).ToListAsync()).Any())
            {
                logger.LogDebug("BuilderEducationerAiepRepository end call GetBuilderEducationerAiepModelRelation -> return Repository response Errors Entity not found");

                return new RepositoryResponse<BuilderEducationerAiepModel>(ErrorCode.EntityNotFound.GetDescription());
            }

            logger.LogDebug("BuilderEducationerAiepRepository end call GetBuilderEducationerAiepModelRelation -> return Repository response BuilderEducationerAiepModel");

            return new RepositoryResponse<BuilderEducationerAiepModel>();
        }



        public async Task<RepositoryResponse<BuilderEducationerAiepModel>> CreateBuilderEducationerAiepModelRelation(int builderId, int AiepId)
        {
            logger.LogDebug("BuilderEducationerAiepRepository called CreateBuilderEducationerAiepModelRelation");

            var builderResponse = await repository.Where(new EntityByIdSpecification<Builder>(builderId)).IgnoreQueryFilters().FirstOrDefaultAsync();

            var AiepResponse = await base.FindOneAsync<Aiep>(AiepId);

            var builderModel = mapper.Map<Builder, BuilderModel>(builderResponse);

            var AiepModel = mapper.Map<Aiep, AiepModel>(AiepResponse);

            if (builderModel.IsNull() || AiepModel.IsNull())
            {
                logger.LogDebug("BuilderEducationerAiepRepository end call CreateBuilderEducationerAiepModelRelation -> return Repository response Errors Entity not found");

                return new RepositoryResponse<BuilderEducationerAiepModel>(ErrorCode.EntityNotFound.GetDescription());
            }

            var builderEducationerAiep = new BuilderEducationerAiepModel(builderModel, AiepModel);
            BuilderEducationerAiep entity = mapper.Map<BuilderEducationerAiepModel, BuilderEducationerAiep>(builderEducationerAiep);
            await Add(entity);

            logger.LogDebug("BuilderEducationerAiepRepository end call CreateBuilderEducationerAiepModelRelation -> return Repository response BuilderEducationerAiepModel");

            return new RepositoryResponse<BuilderEducationerAiepModel>(builderEducationerAiep);
        }

        public async Task<RepositoryResponse<BuilderEducationerAiepModel>> GetBuilderEducationerAiepModelRelationByBuilderId(int builderId)
        {
            logger.LogDebug("BuilderEducationerAiepRepository called GetBuilderEducationerAiepModelRelationByBuilderId");

            var spec = new BuilderEducationerAiepByBuilderIdSpecification(builderId);
            var builderEducationerAiep = (await repository.Where(spec).Include(b => b.Builder).Include(d => d.Aiep).FirstOrDefaultAsync());
            var builderEducationerAiepModel = mapper.Map<BuilderEducationerAiep, BuilderEducationerAiepModel>(builderEducationerAiep);

            logger.LogDebug("BuilderEducationerAiepRepository end call GetBuilderEducationerAiepModelRelationByBuilderId -> return Repository response BuilderEducationerAiepModel");

            return new RepositoryResponse<BuilderEducationerAiepModel>(builderEducationerAiepModel);
        }

        public async Task<RepositoryResponse<BuilderEducationerAiepModel>> GetBuilderEducationerAiepModelByBuilderIdAiepId(int builderId, int AiepId)
        {
            logger.LogDebug("BuilderEducationerAiepRepository called GetBuilderEducationerAiepModelByBuilderIdAiepId");

            var spec = new BuilderEducationerAiepCheckByBuilderIdAiepIdSpecification(builderId, AiepId);
            var builderEducationerAiep = (await repository.Where(spec).Include(b => b.Builder).Include(d => d.Aiep).FirstOrDefaultAsync());
            var builderEducationerAiepModel = mapper.Map<BuilderEducationerAiep, BuilderEducationerAiepModel>(builderEducationerAiep);

            logger.LogDebug("BuilderEducationerAiepRepository end call GetBuilderEducationerAiepModelByBuilderIdAiepId -> return Repository response BuilderEducationerAiepModel");

            return new RepositoryResponse<BuilderEducationerAiepModel>(builderEducationerAiepModel);
        }
    }
}


