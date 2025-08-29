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
.Business.DtoModel;
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
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.AreaSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class AreaRepository : BaseRepository<Area>, IAreaRepository
    {
        private readonly IObjectMapper mapper;
        private readonly ILogger<AreaRepository> logger;

        public AreaRepository(ILogger<AreaRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) : base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<ICollection<AiepModel>>> GetAreaAieps(int areaId)
        {
            logger.LogDebug("AreaRepository called GetAreaAieps");

            var spec = Specification<Area>.True;
            spec = spec.And(new EntityByIdSpecification<Area>(areaId));
            var area = await repository.Where(spec).Include(x => x.Aieps).FirstOrDefaultAsync();
            if (area.IsNull())
            {
                var errors = new Collection<string>();
                errors.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("AreaRepository end call GetAreaAieps -> return Repository response with errors Entity not found");

                return new RepositoryResponse<ICollection<AiepModel>>(errors);
            }
            var result = mapper.Map<Area, AreaModel>(area).Aieps;

            logger.LogDebug("AreaRepository end call GetAreaAieps -> return Repository response Collection of AiepModel");

            return new RepositoryResponse<ICollection<AiepModel>>(result);
        }

        public async Task<RepositoryResponse<IPagedQueryResult<AreaModel>>> GetAreasFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("AreaRepository called GetAreasFilteredAsync");

            var spec = Specification<Area>.True;
            if (searchModel.Filters.Any(f => f.Member.Contains("AiepCount")))
            {
                var filter = searchModel.Filters.Where(f => f.Member.Contains("AiepCount")).FirstOrDefault();
                spec = spec.And(new Specification<Area>(p => p.Aieps.Count().ToString() == filter.Value));
            }

            var modelSpec = SpecificationBuilder.Create<AreaModel>(searchModel.Filters);

            var query = new AreaMaterializedAreaModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("AreaRepository end call GetAreasFilteredAsync -> return Repository response Paged query Areas");

            return new RepositoryResponse<IPagedQueryResult<AreaModel>>(result);
        }

        public async Task<RepositoryResponse<AreaModel>> SaveArea(AreaDtoModel areaDtoModel)
        {
            logger.LogDebug("AreaRepository called SaveArea");

            var areaEntity = areaDtoModel.Id == 0 ?
                await repository.Where(new AreaByNameSpecification(areaDtoModel.KeyName)).FirstOrDefaultAsync() :
                await repository.Where(new AreaByNameAndDifferentIdSpecification(areaDtoModel.KeyName, areaDtoModel.Id)).FirstOrDefaultAsync();
            if (areaEntity != null)
            {
                logger.LogError("{type}#{Area} already exists", typeof(AreaModel), areaDtoModel.KeyName);

                logger.LogDebug("AreaRepository end call SaveArea -> return Repository response Errors Entity already exists");

                return new RepositoryResponse<AreaModel>(null, ErrorCode.EntityAlreadyExist, "Area already exists");
            }

            try
            {
                UnitOfWork.BeginTransaction();
                Area area = await Repository.FindOneAsync<Area>(areaDtoModel.Id);

                var areaByName = await repository.Where(new AreaByNameSpecification(areaDtoModel.KeyName)).FirstOrDefaultAsync();

                area = await base.Repository.CreateOrUpdateArea(areaDtoModel, mapper, logger);

                if (areaByName != null && area.Id != areaByName.Id)
                {
                    logger.LogError(ErrorCode.EntityAlreadyExist.GetDescription(), "Area name repeated");

                    logger.LogDebug("AreaRepository end call SaveArea -> return Repository response Errors Entity already exists Name repeated");

                    return new RepositoryResponse<AreaModel>(null, ErrorCode.EntityAlreadyExist, "Name repeated");
                }

                area = await base.Repository.SetAiepAreas(area, areaDtoModel.AiepIds, logger);

                logger.LogDebug("AreaRepository SaveArea Commit");

                UnitOfWork.Commit();

                logger.LogDebug("AreaRepository end call SaveArea -> return Repository response Area model");

                return new RepositoryResponse<AreaModel>(mapper.Map<Area, AreaModel>(area));

            }
            catch (Exception)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("AreaRepository end call SaveArea -> exception");

                throw;
            }
        }
    }
}
