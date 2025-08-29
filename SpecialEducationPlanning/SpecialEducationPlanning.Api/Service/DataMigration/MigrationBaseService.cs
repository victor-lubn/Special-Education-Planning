using Koa.Domain;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Platform.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Api.Services.DataMigration
{

    /// <summary>
    ///     Base migration service
    /// </summary>
    public abstract class MigrationBaseService<T, M, SK>
        where T : class, IEntity<int>
        where M : MigrationBaseModel
    {

        private static readonly int maxRetries = 5;

        private static readonly int waitMilliseconds = 2000;

        private static readonly int waitMultiplier = 4;

        /// <summary>
        ///     Class logger
        /// </summary>
        protected readonly ILogger<MigrationBaseService<T, M, SK>> _logger;

        private readonly bool bulkMode;

        /// <summary>
        ///     Cache
        /// </summary>
        /// h
        protected readonly IDistributedCache cache;

        private readonly ILogger<DataContext> dataContextLogger;
        protected readonly IObjectMapper mapper;

        /// <summary>
        ///     Repository supporting this migratino
        /// </summary>
        private readonly DataContext dbContext;

        private readonly DbContextOptions dbContextOptions;

        private readonly IIdentityProvider identityProvider;

        /// <summary>
        ///     Avoid duplicates
        /// </summary>
        protected bool avoidDuplicates;

        /// <summary>
        ///     Creates a new instance of <see cref="MigrationBaseService{T, M}" />
        /// </summary>
        /// <param name="cache">Distributed cache</param>
        /// <param name="dbContextAccesor">DBContext to migrate the information</param>
        /// <param name="logger">Class logger</param>
        /// <param name="avoidDuplicates"></param>
        public MigrationBaseService(IDistributedCache cache, IDbContextAccessor dbContextAccesor,
            ILogger<MigrationBaseService<T, M, SK>> logger, IIdentityProvider identityProvider,
            DbContextOptions dbContextOptions, ILogger<DataContext> dataContextLogger,
            IObjectMapper mapper,
            bool avoidDuplicates = true,
            bool bulkMode = false)
        {
            dbContext = dbContextAccesor.GetCurrentContext() as DataContext;
            _logger = logger;
            this.identityProvider = identityProvider;
            this.dbContextOptions = dbContextOptions;
            this.dataContextLogger = dataContextLogger;
            this.mapper = mapper;
            this.avoidDuplicates = avoidDuplicates;
            this.bulkMode = bulkMode;
            this.cache = cache;
        }

        #region Methods Public

        /// <summary>
        ///     Executes the data migration
        /// </summary>
        /// <param name="data">Data to migrate</param>
        /// <returns>Migrated data</returns>
        public IEnumerable<M> Migrate(IEnumerable<M> data)
        {
            dbContext.DisableUpdateAclOnSave = true;
            var returnData = bulkMode ? MigrateBulk(data) : MigrateSerial(data);
            dbContext.DisableUpdateAclOnSave = false;

            return returnData;
        }

        #endregion

        #region Methods Protected

        /// <summary>
        ///     Adds a new entity
        /// </summary>
        /// <typeparam name="E"></typeparam>
        protected EntityEntry<E> Add<E>(E entity) where E : class
        {
            return dbContext.Set<E>().Add(entity);
        }

        /// <summary>
        ///     Creates a new entity instance
        /// </summary>
        /// <returns>New entity instance</returns>
        protected virtual T CreateEntity()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        ///     Default access to dbset for current entity
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<T> DbSet()
        {
            return DbSet<T>();
        }

        /// <summary>
        ///     Default access to the dbset for the given entity.
        ///     Is important to use this to avoid the ACLs filters.
        ///     Otherwise the migration process won't be able to read any data!
        /// </summary>
        protected virtual IQueryable<E> DbSet<E>() where E : class
        {
            return dbContext.Set<E>().AsNoTracking().IgnoreQueryFilters();
        }

        protected virtual EntityEntry DetachEntity<K>(K entity)
        {
            var entityEntry = dbContext.Entry(entity);
            entityEntry.State = EntityState.Detached;

            return entityEntry;
        }

        /// <summary>
        ///     Tries to find an entity by its secondary key
        /// </summary>
        /// <param name="query">Base query to use</param>
        /// <param name="model">Model to filter by</param>
        /// <returns></returns>
        protected abstract T FilterBySecondaryKey(IQueryable<T> query, M model);

        /// <summary>
        ///     Given a collection of remote models, this functions returns the equivalent in the database
        /// </summary>
        /// <param name="models">Source collection of data</param>
        /// <returns>Queryable expression to filter <see cref="T" /> entities</returns>
        protected abstract IEnumerable<T> GetEntities(IEnumerable<M> models);

        /// <summary>
        ///     Transaction level for the oprations
        /// </summary>
        /// <returns>Tx level</returns>
        protected virtual IsolationLevel GetIsolationLevel()
        {
            return IsolationLevel.ReadUncommitted;
        }

        /// <summary>
        ///     Given a collection of entities, builds a dictionary using the migration key
        /// </summary>
        /// <param name="entities">A collectin of models</param>
        /// <returns>A lookup to search by</returns>
        protected abstract EntityBaseLookup<T, int, SK> GetLookup(IEnumerable<T> entities);

        /// <summary>
        ///     Gets the model secondary key
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Secondary key</returns>
        protected abstract SK GetSecondaryKey(M model);

        /// <summary>
        ///     Maps the model to a new entity;
        /// </summary>
        /// <param name="model">Model to map from</param>
        /// <returns>Mapped entity</returns>
        protected T Map(M model)
        {
            return Map(model, Activator.CreateInstance<T>());
        }

        /// <summary>
        ///     Maps a model to an existing entity
        /// </summary>
        /// <param name="model">Model to map from</param>
        /// <param name="entity">Entity to map to</param>
        /// <returns>Mapped entity</returns>
        protected virtual T Map(M model, T entity)
        {
            return this.mapper.Map(model, entity);
        }

        /// <summary>
        ///     Maps back properties from the entity to the model
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        protected virtual void MapBack(T entity, M model)
        {
            model.TdpId = entity.Id;
            model.Status = "Migrated";
        }

        /// <summary>
        ///     Save actions. By default it saves the information to the database.
        /// </summary>
        /// <param name="entity">Entity to save, with </param>
        /// <param name="model">Model used to create this entity</param>
        protected virtual void Save(T entity, M model)
        {
            if (entity.Id == 0)
            {
                dbContext.Add(entity);
            }

            dbContext.SaveChanges();
        }

        /// <summary>
        ///     Saves changes
        /// </summary>
        protected void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        #endregion

        #region Methods Private

        /// <summary>
        ///     All possible ways to look for an entity, in a compromise between performance and duplication avoidance
        /// </summary>
        /// <param name="lookup">current lookup</param>
        /// <param name="model">model to look for</param>
        /// <returns></returns>
        private T GetEntity(EntityBaseLookup<T, int, SK> lookup, M model)
        {
            return lookup.GetByPrimaryKey(model.TdpId)
                   ?? (model.TdpId.IsNull() || model.TdpId < 1
                       ? null
                       : dbContext.Set<T>().AsNoTracking().IgnoreQueryFilters()
                           .FirstOrDefault(x => x.Id == model.TdpId))
                   ?? lookup.GetBySecondaryKey(GetSecondaryKey(model))
                   ?? FilterBySecondaryKey(dbContext.Set<T>().AsNoTracking().IgnoreQueryFilters(), model)
                   ?? Activator.CreateInstance<T>();
        }

        private void LogDebugWarning(int count, int countMax, string message, params object[] args)
        {
            if (count >= countMax)
            {
                _logger.LogWarning(message, args);
            }
            else
            {
                _logger.LogDebug(message, args);
            }
        }

        private void LogErrorWarning(int count, int countMax, Exception ex, string message, params object[] args)
        {
            if (count >= countMax)
            {
                _logger.LogError(ex, message, args);
            }
            else
            {
                _logger.LogWarning(ex, message, args);
            }
        }

        private string MessageFromException(Exception ex)
        {
            var message = "";

            do
            {
                message += string.Join(Environment.NewLine, ex.Message, ex.StackTrace);
                ex = ex.InnerException;
            } while (ex != null);

            return message;
        }

        private IEnumerable<M> MigrateBulk(IEnumerable<M> data)
        {
            var modelEntityDictionary = new Dictionary<M, T>();
            var lookup = RefreshDataLookup(data);

            SetIsolationLevel(GetIsolationLevel(), TransactionScopeOption.Required, new TimeSpan(1, 0, 0), () =>
            {
                var retryCount = 0;
                var retry = false;

                do
                {
                    try
                    {
                        var count = data.Count();

                        for (var i = 0; i < count; i++)
                        {
                            var model = data.ElementAt(i);

                            _logger.LogDebug("{current} out of {total}: Migrating model {modelType} to {entityType}",
                                i + 1,
                                count, typeof(M), typeof(T));

                            var entity = avoidDuplicates ? GetEntity(lookup, model) : Activator.CreateInstance<T>();

                            _logger.LogDebug("Mapping {modelType}#{migrationId} to {entityType}#{tdpid}", typeof(M),
                                model.Id, typeof(T), entity.Id);

                            entity = Map(model, entity);

                            if (entity != null)
                            {
                                dbContext.Add(entity);
                            }

                            modelEntityDictionary.Add(model, entity);
                        }

                        SaveChanges();

                        foreach (var keyValue in modelEntityDictionary)
                        {
                            var entity = keyValue.Value;
                            var model = keyValue.Key;
                            MapBack(entity, model);

                            _logger.LogDebug("Model {modelType}#{migrationId} migrated to {entityType}#{tdpid}",
                                typeof(M), model.Id, typeof(T), entity.Id);
                        }

                        retry = false;
                        retryCount = 0;
                    }
                    catch (Exception ex)
                    {
                        LogErrorWarning(retryCount, maxRetries, ex, "Error migrating {targetType}", typeof(T));
                        var message = MessageFromException(ex);

                        foreach (var migrationBaseModel in data)
                        {
                            migrationBaseModel.Status = "Error";
                            migrationBaseModel.Message = message;
                        }

                        lookup = RefreshDataLookup(data);
                        retry = true;
                        retryCount++;

                        if (retryCount < maxRetries)
                        {
                            Thread.Sleep(waitMilliseconds * retryCount * waitMultiplier);
                        }
                    }
                } while (retry && retryCount <= maxRetries);
            });

            return data;
        }

        private IEnumerable<M> MigrateSerial(IEnumerable<M> data)
        {
            var lookup = RefreshDataLookup(data);

            SetIsolationLevel(GetIsolationLevel(), TransactionScopeOption.Required, new TimeSpan(1, 0, 0), () =>
            {
                var retryCount = 0;
                var count = data.Count();

                for (var i = 0; i < count; i++)
                {
                    var model = data.ElementAt(i);

                    _logger.LogDebug("{current} out of {total}: Migrating model {modelType} to {entityType}", i + 1,
                        count, typeof(M), typeof(T));

                    T entity = null;

                    try
                    {
                        _logger.LogDebug("Looking for equivalent entity...");
                        entity = avoidDuplicates ? GetEntity(lookup, model) : Activator.CreateInstance<T>();

                        _logger.LogDebug("Mapping {modelType}#{migrationId} to {entityType}#{tdpid}", typeof(M),
                            model.Id, typeof(T), entity.Id);

                        entity = Map(model, entity);

                        if (entity != null)
                        {
                            _logger.LogDebug("Saving to database...");
                            Save(entity, model); // We save entities one by one. Is slower, but avoids page faults
                        }

                        MapBack(entity, model);

                        LogDebugWarning(retryCount, 1,
                            "Model Retry:{retryCount} {modelType}#{migrationId} migrated to {entityType}#{tdpid}",
                            retryCount, typeof(M), model.Id, typeof(T), entity.Id);

                        retryCount = 0;
                    }
                    catch (Exception ex)
                    {
                        LogErrorWarning(retryCount, maxRetries, ex, "Error migrating {targetType}", typeof(T));
                        model.Status = "Error";
                        model.Message = MessageFromException(ex);

                        if (entity != null)
                        {
                            DetachEntity(entity);
                        }

                        if (retryCount <= maxRetries)
                        {
                            i--;
                            lookup = RefreshDataLookup(data, i - 1);
                            _logger.LogDebug(ex, "Retry migrating {targetType}", typeof(T));
                            Thread.Sleep(waitMilliseconds * retryCount * waitMultiplier);
                            retryCount++;
                        }
                        else
                        {
                            retryCount = 0;
                        }
                    }
                }
            });

            return data;
        }

        private EntityBaseLookup<T, int, SK> RefreshDataLookup(IEnumerable<M> data, int skip = 0)
        {
            IEnumerable<T> entities = new List<T>();
            _logger.LogDebug("Loading possible related data...");
            entities = GetEntities(data.Skip(skip));
            _logger.LogDebug("Creating lookups...");
            var lookup = GetLookup(entities);

            return lookup;
        }

        private static void SetIsolationLevel(IsolationLevel isolationLevel,
            TransactionScopeOption transactionScopeOption,
            TimeSpan timeout, Action action)
        {
            var transactionOptions = new TransactionOptions { IsolationLevel = isolationLevel, Timeout = timeout };

            using (var transactionScope = new TransactionScope(transactionScopeOption, transactionOptions))
            {
                action();
                transactionScope.Complete();
            }
        }

        #endregion

    }

}