using Koa.Domain.Specification;
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
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.RomItemSpecifications;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class RomItemRepository : BaseRepository<RomItem>, IRomItemRepository
    {
        private readonly IObjectMapper mapper;
        private readonly DataContext context;
        private readonly ILogger<RomItemRepository> logger;

        public RomItemRepository(ILogger<RomItemRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository entityRepository) :
             base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.mapper = mapper;
            this.context = (DataContext)dbContextAccessor.GetCurrentContext();
            this.logger = logger;
        }

        public async Task<RepositoryResponse<RomItem>> GetRomItemByNameAsync(string name)
        {
            logger.LogDebug("RomItemRepository called GetRomItemByNameAsync");

            ISpecification<RomItem> spec = new RomItemByNameSpecification(name);
            var result = await repository.Where(spec).ToListAsync();
            if (result.Count == 0)
            {
                logger.LogDebug("RomItemRepository end call GetRomItemByNameAsync -> return Repository response Errors No results");

                return new RepositoryResponse<RomItem>(ErrorCode.NoResults.GetDescription());
            }

            logger.LogDebug("RomItemRepository end call GetRomItemByNameAsync -> return Repository response RomItem");

            return new RepositoryResponse<RomItem>(result.First());
        }

        public async Task<RepositoryResponse<RomItemModel>> CreateRomItem(RomItemModel romItemModel, int versionId, int catalogId)
        {
            logger.LogDebug("RomItemRepository called CreateRomItem");

            var repositoryResponse = new RepositoryResponse<RomItemModel>();

            if (romItemModel.IsNull())
            {
                repositoryResponse.ErrorList.Add(ErrorCode.ArgumentErrorBusiness.GetDescription());

                logger.LogDebug("RomItemRepository end call CreateRomItem -> return Repository response Errors Argument error business");

                return repositoryResponse;
            }

            if (!await repository.AnyAsync(new EntityByIdSpecification<Version>(versionId)))
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("RomItemRepository end call CreateRomItem -> return Repository response Errors Argument Entity not found");

                return repositoryResponse;
            }

            if (!await repository.AnyAsync(new EntityByIdSpecification<Catalog>(catalogId)))
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("RomItemRepository end call CreateRomItem -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }

            UnitOfWork.BeginTransaction();

            var romItem = new RomItem()
            {
                ItemName = romItemModel.ItemName,
                CatalogId = catalogId,
                Range = romItemModel.Range,
                Colour = romItemModel.Colour,
                Qty = romItemModel.Qty,
                VersionId = versionId
            };

            var newRomItem = repository.Add(romItem);

            UnitOfWork.Commit();

            repositoryResponse.Content = mapper.Map<RomItem, RomItemModel>(newRomItem);

            logger.LogDebug("RomItemRepository end call CreateRomItem -> return Repository response RomItemModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<bool>> DeleteRomItemsFromVersion(int versionId)
        {
            logger.LogDebug("RomItemRepository called DeleteRomItemsFromVersion");

            var versionRomItems = await repository.Where(new RomItemsByVersionIdSpecification(versionId)).ToListAsync();

            if (versionRomItems == null || versionRomItems.Count == 0)
            {

                logger.LogDebug("RomItemRepository end call DeleteRomItemsFromVersion -> return Repository response Errors Entity not found");
                return new RepositoryResponse<bool>(false, ErrorCode.EntityNotFound);
            }

            UnitOfWork.BeginTransaction();
            foreach (var item in versionRomItems)
            {
                repository.Remove(item);
            }

            logger.LogDebug("RomItemRepository DeleteRomItemsFromVersion call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("RomItemRepository end call DeleteRomItemsFromVersion -> return Repository response True");

            return new RepositoryResponse<bool>(true);
        }
    }
}
