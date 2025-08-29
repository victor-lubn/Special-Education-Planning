using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Specification;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class SortingFilteringRepository : BaseRepository<SortingFiltering>, ISortingFilteringRepository
    {
        private readonly IObjectMapper mapper;
        private readonly ILogger<SortingFilteringRepository> logger;

        public SortingFilteringRepository(ILogger<SortingFilteringRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder,IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<IEnumerable<SortingFilteringModel>>> GetSortingFilteringOptionsByEntity(string entity)
        {
            logger.LogDebug("SortingFilteringRepository called GetSortingFilteringOptionsByEntity");

            var spec = new SortingFilteringByEntitySpecification(entity);
            var sortingFilteringOptions = await repository.Where(spec).ToListAsync();

            var sortingFilteringModels = mapper.Map<IEnumerable<SortingFiltering>, IEnumerable<SortingFilteringModel>>(sortingFilteringOptions);

            logger.LogDebug("SortingFilteringRepository end call GetSortingFilteringOptionsByEntity -> return Repository response List of SortingFilteringModel");

            return new RepositoryResponse<IEnumerable<SortingFilteringModel>>(sortingFilteringModels);
        }
    }
}