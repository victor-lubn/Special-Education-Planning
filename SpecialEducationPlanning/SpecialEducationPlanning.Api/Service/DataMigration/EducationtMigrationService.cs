using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Platform.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Api.Configuration.DataMigration;
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
    ///     Service in charge of the Aiep migration
    /// </summary>
    public class AiepMigrationService : MigrationBaseService<Aiep, AiepMigrationModel, string>
    {
        private IOptions<DataMigrationConfiguration> dataMigrationConfigurationOptions;

        /// <summary>
        ///     Createas a new instance of <see cref="AiepMigrationService " />
        /// </summary>
        /// <param name="cache">Distributed cache</param>
        /// <param name="dbContextAccessor">DataContext accessor</param>
        /// <param name="logger">Logger</param>
        public AiepMigrationService(IDistributedCache cache, IDbContextAccessor dbContextAccessor,
            ILogger<MigrationBaseService<Aiep, AiepMigrationModel, string>> logger,
            IIdentityProvider identityProvider, DbContextOptions dbContextOptions,
            ILogger<DataContext> dataContextLogger, IObjectMapper mapper, IOptions<DataMigrationConfiguration> options) : base(cache, dbContextAccessor, logger, identityProvider,
            dbContextOptions, dataContextLogger, mapper)
        {
            this.dataMigrationConfigurationOptions = options;
        }

        #region Methods Protected

        /// <summary>
        ///     Filters a Aiep by AiepNmbr
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="model">Aiep model to use as filter</param>
        /// <returns></returns>
        protected override Aiep FilterBySecondaryKey(IQueryable<Aiep> query, AiepMigrationModel model)
        {
            var key = GetSecondaryKey(model);

            return query.FirstOrDefault(x => x.AiepCode == key);
        }

        /// <summary>
        ///     Retrieve all existing Aieps from the database using AiepCode as reference
        /// </summary>
        /// <param name="models">Models willing to insert</param>
        /// <returns>List of matched Aieps</returns>
        protected override IEnumerable<Aiep> GetEntities(IEnumerable<AiepMigrationModel> models)
        {
            if (!avoidDuplicates)
            {
                return new List<Aiep>();
            }

            var ids = models.Where(x => x.TdpId > 0).Select(x => x.TdpId).ToList();
            var AiepCodes = models.Select(m => m.AiepNmbr).ToList();

            return DbSet<Aiep>().Where(d => ids.Contains(d.Id) || AiepCodes.Contains(d.AiepCode)).ToList();
        }

        /// <summary>
        ///     Creates a new Aiep lookup
        /// </summary>
        /// <param name="entities">Entities to lookup</param>
        /// <returns>Entities lookup</returns>
        protected override EntityBaseLookup<Aiep, int, string> GetLookup(IEnumerable<Aiep> entities)
        {
            return new AiepLookup(entities);
        }

        /// <summary>
        ///     This service uses AiepNmbr as secondary key
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected override string GetSecondaryKey(AiepMigrationModel model)
        {
            return model.AiepNmbr;
        }

        /// <summary>
        ///     Executes the standard migration mapping and then completes the entity with required relationships
        /// </summary>
        /// <param name="model">Model to map from</param>
        /// <param name="entity">Entity to map to</param>
        /// <returns>Mapped entity</returns>
        protected override Aiep Map(AiepMigrationModel model, Aiep entity)
        {
            entity = base.Map(model, entity);
            MapRelationships(model, entity);

            return entity;
        }

        #endregion

        #region Methods Private

        /// <summary>
        ///     Tries getting an area from the distributed cache or the database
        /// </summary>
        /// <param name="model">migration model of reference</param>
        /// <returns>Id or 0</returns>
        private int GetArea(AiepMigrationModel model)
        {
            var cachedId = GetCacheId<Area>(model.Area);

            if (!cachedId.HasValue)
            {
                cachedId = DbSet<Area>().AsNoTracking().FirstOrDefault(x => x.KeyName == model.Area)?.Id;

                if (cachedId.HasValue)
                {
                    SetCacheId<Area>(model.Area, cachedId.Value);
                }
            }

            return cachedId ?? 0;
        }

        /// <summary>
        ///     Tries to find an Id in the cache
        /// </summary>
        /// <typeparam name="T">Entity to get the id</typeparam>
        /// <param name="keyPart">A reference value for this key</param>
        /// <returns>Id or null</returns>
        private int? GetCacheId<T>(string keyPart)
        {
            var key = string.Join(".", typeof(T).GetType().ToString(), keyPart);
            var stringId = cache.GetString(key);

            return int.TryParse(stringId, out var id) ? new int?(id) : null;
        }

        /// <summary>
        ///     Tries getting a country from the distributed cache or the database
        /// </summary>
        /// <param name="model">migration model of reference</param>
        /// <returns>Id or 0</returns>
        private int GetCountry(AiepMigrationModel model)
        {
            var cachedId = GetCacheId<Country>(model.Country);

            if (!cachedId.HasValue)
            {
                cachedId = DbSet<Country>().AsNoTracking().FirstOrDefault(x => x.KeyName == model.Country)?.Id;

                if (cachedId.HasValue)
                {
                    SetCacheId<Country>(model.Country, cachedId.Value);
                }
            }

            return cachedId ?? 0;
        }

        /// <summary>
        ///     Tries getting a region from the distributed cache or the database
        /// </summary>
        /// <param name="model">migration model of reference</param>
        /// <returns>Id or 0</returns>
        private int GetRegion(AiepMigrationModel model)
        {
            var cachedId = GetCacheId<Region>(model.Region);

            if (!cachedId.HasValue)
            {
                cachedId = DbSet<Region>().AsNoTracking().FirstOrDefault(x => x.KeyName == model.Region)?.Id;

                if (cachedId.HasValue)
                {
                    SetCacheId<Region>(model.Region, cachedId.Value);
                }
            }

            return cachedId ?? 0;
        }

        /// <summary>
        ///     This functions executes the Profile mapper and then query for additional relationship we need to satisfy.
        ///     If any of the relations do no exist, news are created
        /// </summary>
        /// <param name="model">Model to map</param>
        /// <param name="entity">Entity to map</param>
        /// <returns>Fully mapped entity</returns>
        private void MapRelationships(AiepMigrationModel model, Aiep entity)
        {
            if (model.Country.IsNullOrEmpty())
            {
                model.Country = "UK";
            }

            entity.AreaId = GetArea(model);

            if (entity.AreaId == 0)
            {
                entity.Area = new Area
                {
                    KeyName = model.Area ?? dataMigrationConfigurationOptions.Value.UnknownMessage,
                    RegionId = GetRegion(model)
                };

                if (entity.Area.RegionId == 0)
                {
                    entity.Area.Region = new Region
                    {
                        KeyName = model.Region ?? dataMigrationConfigurationOptions.Value.UnknownMessage,
                        CountryId = GetCountry(model)
                    };

                    if (entity.Area.Region.CountryId == 0)
                    {
                        entity.Area.Region.Country = new Country
                        {
                            KeyName = model.Country
                        };
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a new entry in the cache
        /// </summary>
        /// <typeparam name="T">Entity the Id refers to</typeparam>
        /// <param name="keyPart">A reference value for the key</param>
        /// <param name="value">Value to store</param>
        private void SetCacheId<T>(string keyPart, int value)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };

            if (value == 0)
            {
                return;
            }

            var key = string.Join(".", typeof(T).GetType().ToString(), value);
            cache.SetString(key, value.ToString(), options);
        }

        #endregion

    }

}
