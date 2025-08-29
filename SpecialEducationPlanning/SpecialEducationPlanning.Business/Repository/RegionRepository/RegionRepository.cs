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
.Domain.Specification.RegionSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class RegionRepository : BaseRepository<Region>, IRegionRepository
    {
        private readonly IObjectMapper mapper;
        private readonly ILogger<RegionRepository> logger;

        public RegionRepository(ILogger<RegionRepository> logger, IEntityRepository repository,IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey,  dbContextAccessor)
        {
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<RepositoryResponse<ICollection<AreaModel>>> GetRegionAreas(int regionId)
        {
            logger.LogDebug("RegionRepository called GetRegionAreas");

            var spec = Specification<Region>.True;
            spec = spec.And(new EntityByIdSpecification<Region>(regionId));
            var region = await repository.Where(spec).Include(x => x.Areas).FirstOrDefaultAsync();

            if (region.IsNull())
            {
                var errors = new Collection<string>();
                errors.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("RegionRepository end call GetRegionAreas -> return Repository response Collection of AreaModel");

                return new RepositoryResponse<ICollection<AreaModel>>(errors);
            }
            var result = mapper.Map<Region, RegionModel>(region).Areas;

            logger.LogDebug("RegionRepository end call GetRegionAreas -> return Repository response Collection AreaModel");

            return new RepositoryResponse<ICollection<AreaModel>>(result);
        }

        public async Task<RepositoryResponse<IPagedQueryResult<RegionModel>>> GetRegionsFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("RegionRepository called GetRegionsFilteredAsync");

            var spec = Specification<Region>.True;
            if (searchModel.Filters.Any(f => f.Member.Contains("AreasCount")))
            {
                var filter = searchModel.Filters.Where(f => f.Member.Contains("AreasCount")).FirstOrDefault();
                spec = spec.And(new Specification<Region>(p => p.Areas.Count().ToString() == filter.Value));
            }

            var modelSpec = SpecificationBuilder.Create<RegionModel>(searchModel.Filters);

            var query = new RegionMaterializedRegionModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("RegionRepository end call GetRegionsFilteredAsync -> return Repository response Paged query RegionModel");

            return new RepositoryResponse<IPagedQueryResult<RegionModel>>(result);
        }

        public async Task<RepositoryResponse<RegionModel>> GetDuplicatedRegion(RegionModel regionModel)
        {
            logger.LogDebug("RegionRepository called GetDuplicatedRegion");

            var region = await repository.Where(new RegionByNameSpecification(regionModel.KeyName)).FirstOrDefaultAsync();
            if (region != null)
            {
                logger.LogError("{type}#{Region} already exists", typeof(RegionModel), regionModel.KeyName);

                logger.LogDebug("RegionRepository end call GetDuplicatedRegion -> return Repository response Entity already exist");

                return new RepositoryResponse<RegionModel>(null, ErrorCode.EntityAlreadyExist, "Region already exists");
            }

            logger.LogDebug("RegionReposiotry end call GetDuplicatedRegion -> return Repository response RegionModel");

            return new RepositoryResponse<RegionModel>(regionModel);
        }
    }
}