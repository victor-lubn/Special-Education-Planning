using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Platform.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Api.Services.DataMigration
{

    /// <summary>
    ///     Plan item migration service
    /// </summary>
    public class PlanItemMigrationService : MigrationBaseService<RomItem, QuoteItemMigrationModel, int>
    {

        private static int defaultCatalogId;

        /// <summary>
        ///     Creates a new instance of <see cref="PlanItemMigrationService" />
        /// </summary>
        /// <param name="dbContextAccesor">DbContext accessor</param>
        /// <param name="logger">Logger</param>
        public PlanItemMigrationService(IDistributedCache cache, IDbContextAccessor dbContextAccesor,
            ILogger<MigrationBaseService<RomItem, QuoteItemMigrationModel, int>> logger,
            IIdentityProvider identityProvider, DbContextOptions dbContextOptions,
            ILogger<DataContext> dataContextLogger, IObjectMapper mapper) : base(cache, dbContextAccesor, logger, identityProvider,
            dbContextOptions, dataContextLogger, mapper, false, true)
        {
        }

        #region Methods Protected

        /// <summary>
        ///     There is no secondary key. Returning always null
        /// </summary>
        /// <param name="query"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected override RomItem FilterBySecondaryKey(IQueryable<RomItem> query, QuoteItemMigrationModel model)
        {
            return null;
        }

        /// <summary>
        ///     Returns matching entities
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        protected override IEnumerable<RomItem> GetEntities(IEnumerable<QuoteItemMigrationModel> models)
        {
            if (!avoidDuplicates)
            {
                return new List<RomItem>();
            }

            var ids = models.Where(x => x.TdpId > 0).Select(x => x.TdpId);

            return DbSet<RomItem>().Where(x => ids.Contains(x.Id));
        }

        /// <summary>
        ///     Createas a new RomItemLookup for the entities
        /// </summary>
        /// <param name="entities">Entities to lookup</param>
        /// <returns>Lookup</returns>
        protected override EntityBaseLookup<RomItem, int, int> GetLookup(IEnumerable<RomItem> entities)
        {
            return new RomItemLookup(entities);
        }

        /// <summary>
        ///     This table do not have secondary key... using HashCode to avoid false matches
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Model HasCode</returns>
        protected override int GetSecondaryKey(QuoteItemMigrationModel model)
        {
            return model.GetHashCode();
        }

        /// <summary>
        ///     Maps a QuoteItem into a RomItem
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override RomItem Map(QuoteItemMigrationModel model, RomItem entity)
        {
            entity = base.Map(model, entity);
            entity.VersionId = model.QuoteTdpId; // QUoteTdpId is mapped in the view to TdpVersionId, so should be fine!

            if (entity.VersionId == 0)
            {
                _logger.LogWarning("VersionId 0 found. This RomItem won't be migrated: SourceId #{sourceId}", model.Id);

                return null;
            }

            entity.CatalogId = GetDefaultCatalogId();

            return entity;
        }

        #endregion

        #region Methods Private

        private int GetDefaultCatalogId()
        {
            if (defaultCatalogId == 0)
            {
                var catalog = DbSet<Catalog>().AsNoTracking().First();
                defaultCatalogId = catalog.Id;
            }

            return defaultCatalogId;
        }

        #endregion

    }

}