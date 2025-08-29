using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
.Domain.Specification.CatalogCodeSpecification;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class CatalogRepository : BaseRepository<Domain.Entity.Catalog>, ICatalogRepository
    {
        private readonly IObjectMapper mapper;

        private readonly ILogger<CatalogRepository> logger;

        public CatalogRepository(ILogger<CatalogRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {

            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<CatalogModel>> GetCodeFromCatalogue(string value, string EducationOrigin)
        {
            logger.LogDebug("CatalogRepository called GetCodeFromCatalogue");

            var spec = new CodeByCatalogSpecification(value, EducationOrigin);
            var catalog = await this.repository.Where(spec)
                .Include(x => x.EducationToolOrigin)
                .SingleOrDefaultAsync();

            if (catalog.IsNull())
            {
                logger.LogDebug("CatalogRepository end call GetCodeFromCatalogue -> return Repository response Errors No result");

                return new RepositoryResponse<CatalogModel>(ErrorCode.NoResults.GetDescription());
            }

            logger.LogDebug("CatalogRepository end call GetCodeFromCatalogue -> return Repository response Catalog model");

            return new RepositoryResponse<CatalogModel>(mapper.Map<Domain.Entity.Catalog, CatalogModel>(catalog));
        }

        public async Task<RepositoryResponse<CatalogModel>> GetCatalogueFromCode(string code, string EducationOrigin)
        {
            logger.LogDebug("CatalogRepository called GetCatalogueFromCode");

            var spec = new CatalogByCodeSpecification(code, EducationOrigin);
            var catalog = await this.repository.Where(spec)
                .Include(x => x.EducationToolOrigin)
                .SingleOrDefaultAsync();

            if (catalog.IsNull())
            {
                logger.LogDebug("CatalogRepository end call GetCatalogueFromCode -> return Repository response Errors No result");

                return new RepositoryResponse<CatalogModel>(ErrorCode.NoResults.GetDescription());
            }

            logger.LogDebug("CatalogRepository end call GetCatalogueFromCode -> return Repository response Catalog model");

            return new RepositoryResponse<CatalogModel>(mapper.Map<Domain.Entity.Catalog, CatalogModel>(catalog));
        }

        public async Task<RepositoryResponse<IEnumerable<CatalogModel>>> GetCatalogs(string EducationOrigin)
        {
            logger.LogDebug("CatalogRepository called GetCatalogs");

            var spec = new CatalogEnabledSpecification(EducationOrigin);
            var catalogs = await this.repository.Where(spec)
                .Include(x => x.EducationToolOrigin)
                .ToListAsync();

            var catalogModels = mapper.Map<IList<Domain.Entity.Catalog>, IList<CatalogModel>>(catalogs);

            if (catalogs.IsNull())
            {
                logger.LogDebug("CatalogRepository end call GetCatalogs -> return Repository response Errors No result");

                return new RepositoryResponse<IEnumerable<CatalogModel>>(ErrorCode.NoResults.GetDescription());
            }
            logger.LogDebug("CatalogRepository end call GetCatalogs -> return Repository response List of Catalog Model");

            return new RepositoryResponse<IEnumerable<CatalogModel>>() { Content = catalogModels };
        }

        public async Task<RepositoryResponse<CatalogModel>> GetCatalogById(int catalogId)
        {
            logger.LogDebug($"{nameof(CatalogRepository)} called {nameof(GetCatalogById)}");
            var spec = new Specification<Catalog>(x => x.Id == catalogId);

            var catalogEntity = await repository.Where(spec)
                .Include(x => x.EducationToolOrigin)
                .FirstOrDefaultAsync();

            var catalogModel = mapper.Map<Catalog, CatalogModel>(catalogEntity);

            return new RepositoryResponse<CatalogModel>(catalogModel);
        }

        public async Task<RepositoryResponse<CatalogModel>> CreateCatalog(CatalogModel value)
        {
            logger.LogDebug($"{nameof(CatalogRepository)} called {nameof(CreateCatalog)}");

            var EducationToolOrigin = string.IsNullOrEmpty(value.EducationOrigin)
                ? null
                : (await this.repository.Where(new Specification<EducationToolOrigin>(x => x.Name == value.EducationOrigin))
                    .SingleOrDefaultAsync());

            var catalogEntity = mapper.Map<CatalogModel, Catalog>(value);
            catalogEntity.EducationToolOriginId = EducationToolOrigin?.Id;

            var results = await Add(catalogEntity);

            var catalogModel = mapper.Map<Catalog, CatalogModel>(results);

            return new RepositoryResponse<CatalogModel>(catalogModel);
        }
    }
}

