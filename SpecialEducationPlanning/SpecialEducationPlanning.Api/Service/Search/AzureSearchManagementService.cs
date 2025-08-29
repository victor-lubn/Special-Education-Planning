using Koa.Domain;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;

namespace SpecialEducationPlanning
.Api.Service.Search
{
    /// <summary>
    ///  Service to manage the Azure Search
    /// </summary>
    public class AzureSearchManagementService : IAzureSearchManagementService
    {
        private readonly AzureSearchConfiguration azureSearchConfiguration;
        private readonly ILogger<IAzureSearchManagementService> logger;
        private readonly string dataSourceConnectionString;
        private readonly IEnumerable<SearchPropertiesConfiguration> searchPropertiesConfigs;
        private readonly IServiceProvider serviceProvider;
        private readonly IObjectMapper mapper;
        private bool deleteAndRecreate = false;

        public static ISearchServiceClient SearchServiceClient { get; set; }

        /// <summary>
        /// Azure Search management service
        /// </summary>
        /// <param name="configuration"></param>
        public AzureSearchManagementService(IOptions<AzureSearchConfiguration> configuration, ILogger<IAzureSearchManagementService> logger,
            IServiceProvider serviceProvider, IObjectMapper mapper)
        {
            this.logger = logger;
            this.azureSearchConfiguration = configuration.Value;
            this.logger.LogDebug("Creating Azure Search service client...", GetType());
            if (SearchServiceClient.IsNull())
            {
                SearchServiceClient = new SearchServiceClient(azureSearchConfiguration.SearchServiceName,
                                  new SearchCredentials(azureSearchConfiguration.SearchServiceAdminKey));
            }
            this.dataSourceConnectionString = azureSearchConfiguration.DataSourceConnectionString;
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.searchPropertiesConfigs = new List<SearchPropertiesConfiguration>
            {
                azureSearchConfiguration.BuilderSearchConfig,
                azureSearchConfiguration.PlanSearchConfig,
                azureSearchConfiguration.VersionSearchConfig,
                azureSearchConfiguration.EndUserSearchConfig,
                azureSearchConfiguration.UserSearchConfig,
                azureSearchConfiguration.ProjectSearchConfig
        };
        }


        /// <summary>
        ///    Creates Azure Search index
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreateOrUpdateIndexAsync<T>(string indexName)
        {
            logger.LogDebug("AzureSearchManagementService called CreateOrUpdateIndexAsync");

            if (deleteAndRecreate)
            {
                var isDeleted = DeleteIndexes(new List<string> { indexName });
                if (!isDeleted)
                {
                    logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexAsync -> return False");

                    return false;
                }
            }

            this.logger.LogDebug("Creating Azure Search index {index}...", indexName);

            logger.LogDebug("AzureSearchManagementService CreateOrUpdateIndexAsync call Indexes.CreateOrUpdateAsync");

            var index = await SearchServiceClient.Indexes.CreateOrUpdateAsync(
                new Microsoft.Azure.Search.Models.Index()
                {
                    Name = indexName,
                    Fields = FieldBuilder.BuildForType<T>()
                });

            logger.LogDebug("AzureSearchManagementService CreateOrUpdateIndexAsync end call Indexes.CreateOrUpdateAsync");

            logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexAsync -> return Call index.IsNotNull");

            return index.IsNotNull();
        }

        /// <summary>
        /// Creates the Azure Search indexes
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreateOrUpdateIndexesAsync()
        {
            logger.LogDebug("AzureSearchManagementService called CreateOrUpdateIndexesAsync");

            // Builder
            var isBuilderIndexCreated = await CreateOrUpdateIndexAsync<OmniSearchBuilderIndexModel>(azureSearchConfiguration.BuilderSearchConfig.IndexName);
            if (!isBuilderIndexCreated)
            {
                this.logger.LogDebug("Failed to create or update Azure Search index {index}...", azureSearchConfiguration.BuilderSearchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return False Builder");

                return false;
            }

            // Plan
            var isPlanIndexCreated = await CreateOrUpdateIndexAsync<OmniSearchPlanIndexModel>(azureSearchConfiguration.PlanSearchConfig.IndexName);
            if (!isPlanIndexCreated)
            {
                this.logger.LogDebug("Failed to create or update Azure Search index {index}...", azureSearchConfiguration.PlanSearchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return False Plan");

                return false;
            }

            // Version
            var isVersionIndexCreated = await CreateOrUpdateIndexAsync<OmniSearchVersionIndexModel>(azureSearchConfiguration.VersionSearchConfig.IndexName);
            if (!isVersionIndexCreated)
            {
                this.logger.LogDebug("Failed to create or update Azure Search index {index}...", azureSearchConfiguration.VersionSearchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return False Version");

                return false;
            }

            // End User
            var isEndUserIndexCreated = await CreateOrUpdateIndexAsync<OmniSearchEndUserIndexModel>(azureSearchConfiguration.EndUserSearchConfig.IndexName);
            if (!isEndUserIndexCreated)
            {
                this.logger.LogDebug("Failed to create or update Azure Search index {index}...", azureSearchConfiguration.EndUserSearchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return False EndUser");

                return false;
            }

            // User
            var isUserIndexCreated = await CreateOrUpdateIndexAsync<OmniSearchUserIndexModel>(azureSearchConfiguration.UserSearchConfig.IndexName);
            if (!isUserIndexCreated)
            {
                this.logger.LogDebug("Failed to create or update Azure Search index {index}...", azureSearchConfiguration.UserSearchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return False User");

                return false;
            }

            logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return True");

            // Project
            var isProjectIndexCreated = await CreateOrUpdateIndexAsync<OmniSearchProjectIndexModel>(azureSearchConfiguration.ProjectSearchConfig.IndexName);
            if (!isProjectIndexCreated)
            {
                this.logger.LogDebug("Failed to create or update Azure Search index {index}...", azureSearchConfiguration.ProjectSearchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return False Project");

                return false;
            }

            logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexesAsync -> return True");

            return true;
        }

        /// <summary>
        /// Deletes Azure Search indexes
        /// </summary>
        /// <returns></returns>
        private bool DeleteIndexes(List<string> indexNames)
        {
            logger.LogDebug("AzureSearchManagementService called DeleteIndexes");

            var tasks = new List<Task>();
            foreach (var indexName in indexNames)
            {
                if (!SearchServiceClient.Indexes.Exists(indexName))
                {
                    this.logger.LogDebug("Azure Search index {name} does not exist...", indexName);
                    continue;
                }
                this.logger.LogDebug("Deleting Azure Search index {index}...", indexName);

                logger.LogDebug("AzureSearchManagementService DeleteIndexes call Indexes.DeleteAsync");

                tasks.Add(SearchServiceClient.Indexes.DeleteAsync(indexName));

                logger.LogDebug("AzureSearchManagementService DeleteIndexes end call Indexes.DeleteAsync");
            }
            Task.WaitAll(tasks.ToArray());

            logger.LogDebug("AzureSearchManagementService end call DeleteIndexes -> return True");

            return true;
        }


        /// <summary>
        ///     Creates Azure Search Indexers
        /// </summary>
        /// <returns></returns>
        private bool CreateOrUpdateIndexers(IEnumerable<SearchPropertiesConfiguration> searchSetConfigs)
        {
            logger.LogDebug("AzureSearchManagementService called CreateOrUpdateIndexers");

            if (deleteAndRecreate)
            {
                var isDeleted = DeleteIndexers(searchSetConfigs.Select(e => e.IndexerName).ToList());
                if (!isDeleted)
                {
                    logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexers -> return False");

                    return false;
                }
            }

            var tasks = new List<Task>();
            foreach (var searchSetConfig in searchSetConfigs)
            {
                logger.LogDebug("AzureSearchManagementService CreateOrUpdateIndexers call Indexes.Exists");

                if (!SearchServiceClient.Indexes.Exists(searchSetConfig.IndexName))
                {
                    this.logger.LogDebug("Cannot create indexer {indexer}. The index {index} does not exist...", searchSetConfig.IndexerName, searchSetConfig.IndexName);

                    logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexers -> return False");

                    return false;
                }

                logger.LogDebug("AzureSearchManagementService CreateOrUpdateIndexers call Indexes.Exists");

                if (!SearchServiceClient.DataSources.Exists(searchSetConfig.DataSourceName))
                {
                    this.logger.LogDebug("Cannot create indexer {indexer}. The data source {dataSource} does not exist...", searchSetConfig.IndexerName, searchSetConfig.DataSourceName);

                    logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexers -> return False");

                    return false;
                }

                this.logger.LogDebug("Creating Azure Search indexer {indexer}...", searchSetConfig.IndexerName);

                logger.LogDebug("AzureSearchManagementService CreateOrUpdateIndexers call Indexers.CreateOrUpdateAsync");

                tasks.Add(SearchServiceClient.Indexers.CreateOrUpdateAsync(
                    new Indexer(
                    name: searchSetConfig.IndexerName,
                    dataSourceName: searchSetConfig.DataSourceName,
                    targetIndexName: searchSetConfig.IndexName,
                    schedule: new IndexingSchedule(new TimeSpan(1, 0, 0))
                    )));

                logger.LogDebug("AzureSearchManagementService CreateOrUpdateIndexers end call Indexers.CreateOrUpdateAsync");
            }
            Task.WaitAll(tasks.ToArray());

            logger.LogDebug("AzureSearchManagementService end call CreateOrUpdateIndexers -> return True");

            return true;
        }

        /// <summary>
        /// Deletes Azure Search indexers
        /// </summary>
        /// <returns></returns>
        private bool DeleteIndexers(List<string> indexerNames)
        {
            logger.LogDebug("AzureSearchManagementService called DeleteIndexers");

            var tasks = new List<Task>();
            foreach (var indexerName in indexerNames)
            {
                logger.LogDebug("AzureSearchManagementService DeleteIndexers call Indexers.Exists");

                if (!SearchServiceClient.Indexers.Exists(indexerName))
                {
                    this.logger.LogDebug("Azure Search indexer {indexer} does not exist...", indexerName);
                    continue;
                }
                this.logger.LogDebug("Deleting Azure Search indexer {indexer}...", indexerName);

                logger.LogDebug("AzureSearchManagementService DeleteIndexers call Indexers.DeleteAsync");

                tasks.Add(SearchServiceClient.Indexers.DeleteAsync(indexerName));

                logger.LogDebug("AzureSearchManagementService DeleteIndexers end call Indexers.DeleteAsync");
            }
            Task.WaitAll(tasks.ToArray());

            logger.LogDebug("AzureSearchManagementService end call DeleteIndexers -> return True");

            return true;
        }


        /// <summary>
        ///     Creates Azure Search data sources
        /// </summary>
        /// <returns></returns>
        private bool CreateOrUpdateDataSources(IEnumerable<SearchPropertiesConfiguration> searchPropertiesConfigs)
        {
            logger.LogDebug("AzureSearchManagementService called CreateOrUpdateDataSource");

            if (deleteAndRecreate)
            {
                var isDeleted = DeleteDataSources(searchPropertiesConfigs.Select(e => e.DataSourceName).ToList());
                if (!isDeleted) return false;
            }

            var tasks = new List<Task>();
            foreach (var searchPropertiesConfig in searchPropertiesConfigs)
            {
                this.logger.LogDebug("Creating Azure Search data source {dataSource}...", searchPropertiesConfig.DataSourceName);

                logger.LogDebug("AzureSearchManagementService CreateOrUpdateDataSources call DataSources.CreateOrUpdateAsync");

                tasks.Add(SearchServiceClient.DataSources.CreateOrUpdateAsync(
                        DataSource.AzureSql(
                           name: searchPropertiesConfig.DataSourceName,
                           sqlConnectionString: dataSourceConnectionString,
                           tableOrViewName: searchPropertiesConfig.DataSourceTableOrView)));

                logger.LogDebug("AzureSearchManagementService CreateOrUpdateDataSources end call DataSources.CreateOrUpdateAsync");

            }
            Task.WaitAll(tasks.ToArray());
            return true;
        }

        /// <summary>
        ///     Deletes Azure Search data source
        /// </summary>
        /// <returns></returns>
        private bool DeleteDataSources(List<string> dataSourceNames)
        {
            logger.LogDebug("AzureSearchManagementService called DeleteDataSources");

            var tasks = new List<Task>();
            foreach (var dataSourceName in dataSourceNames)
            {
                logger.LogDebug("AzureSearchManagementService DeleteDataSources call DataSources.Exists");

                if (!SearchServiceClient.DataSources.Exists(dataSourceName))
                {
                    this.logger.LogDebug("Azure Search data source {dataSource} does not exist...", dataSourceName);

                    logger.LogDebug("AzureSearchManagementService end call DeleteDataSource -> return True");

                    return true;
                }
                this.logger.LogDebug("Deleting Azure Search data source {dataSource}...", dataSourceName);

                logger.LogDebug("AzureSearchManagementService DeleteDataSource call DataSources.DeleteAsync");

                tasks.Add(SearchServiceClient.DataSources.DeleteAsync(dataSourceName));

                logger.LogDebug("AzureSearchManagementService DeleteDataSource call DataSources.DeleteAsync");

            }
            Task.WaitAll(tasks.ToArray());

            logger.LogDebug("AzureSearchManagementService end call DeleteDataSource -> return True");

            return true;
        }


        /// <summary>
        /// Ensures all of the components of Azure Search are created
        /// </summary>
        /// <returns></returns>
        public async Task<bool> EnsureCreatedAsync(bool deleteAndRecreate)
        {
            logger.LogDebug("AzureSearchManagementService called EnsureCreatedAsync");

            this.deleteAndRecreate = deleteAndRecreate;

            var indexesCreated = await CreateOrUpdateIndexesAsync();
            if (!indexesCreated)
            {
                logger.LogDebug("AzureSearchManagementService end call EnsureCreatedAsync -> return False");

                return false;
            }

            if (azureSearchConfiguration.UseIndexer)
            {
                var dataSourcesCreated = CreateOrUpdateDataSources(searchPropertiesConfigs);
                if (!dataSourcesCreated)
                {
                    logger.LogDebug("AzureSearchManagementService end call EnsureCreatedAsync -> return False");

                    return false;
                }

                var indexersCreated = CreateOrUpdateIndexers(searchPropertiesConfigs);
                if (!indexersCreated)
                {
                    logger.LogDebug("AzureSearchManagementService end call EnsureCreatedAsync -> return False");

                    return false;
                }
            }

            logger.LogDebug("AzureSearchManagementService end call EnsureCreatedAsync -> return True");

            return true;
        }
        /// <summary>
        /// Deletes all of the Azure Search components
        /// </summary>
        /// <returns></returns>
        public bool DeleteAzureSearch()
        {
            logger.LogDebug("AzureSearchManagementService called DeleteAzureSearch");

            var indexesDeleted = DeleteIndexes(searchPropertiesConfigs.Select(e => e.IndexName).ToList());
            if (!indexesDeleted)
            {
                logger.LogDebug("AzureSearchManagementService end call DeleteAzureSearch -> return False");

                return false;
            }

            if (azureSearchConfiguration.UseIndexer)
            {
                var dataSourceDeleted = DeleteDataSources(searchPropertiesConfigs.Select(e => e.DataSourceName).ToList());
                if (!dataSourceDeleted)
                {
                    logger.LogDebug("AzureSearchManagementService end call DeleteAzureSearch -> return False");

                    return false;
                }

                var indexersDeleted = DeleteIndexers(searchPropertiesConfigs.Select(e => e.IndexerName).ToList());
                if (!indexersDeleted)
                {
                    logger.LogDebug("AzureSearchManagementService end call DeleteAzureSearch -> return False");

                    return false;
                }
            }

            logger.LogDebug("AzureSearchManagementService end call DeleteAzureSearch -> return True");

            return true;
        }


        /// <summary>
        /// Merges/uploads documents to Azure Search index
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public void MergeOrUploadDocuments<T>(IEnumerable<T> documents)
        {
            logger.LogDebug("AzureSearchManagementService called MergeOrUploadDocuments (documents)");

            if (documents.Any())
            {
                var searchConfig = GetSearchConfigByType(typeof(T));

                logger.LogDebug("AzureSearchManagementService MergeOrUploadDocuments (documents) call Indexes.GetClient");

                var indexClient = SearchServiceClient.Indexes.GetClient(searchConfig.IndexName);

                logger.LogDebug("AzureSearchManagementService MergeOrUploadDocuments (documents) end call Indexes.GetClient");

                logger.LogDebug("AzureSearchManagementService MergeOrUploadDocuments (documents) call MergeOrUpload");

                var indexBatch = IndexBatch.MergeOrUpload(documents);

                logger.LogDebug("AzureSearchManagementService MergeOrUploadDocuments (documents) end call MergeOrUpload");

                logger.LogDebug("AzureSearchManagementService MergeOrUploadDocuments (documents) call Documents.IndexAsync");

                indexClient.Documents.Index(indexBatch);

                logger.LogDebug("AzureSearchManagementService MergeOrUploadDocuments (documents) end call Documents.IndexAsync");

                this.logger.LogDebug("Successfully merged/uploaded documents to index {index}...", searchConfig.IndexName);
            }

            logger.LogDebug("AzureSearchManagementService end call MergeOrUploadDocuments (documents)");
        }


        public Dictionary<Type, IEnumerable<int>> MergeOrUploadDocuments(List<EntityEntry> entityEntries)
        {
            Dictionary<Type, IEnumerable<int>> typesAndIds = new Dictionary<Type, IEnumerable<int>>();
            logger.LogDebug("AzureSearchManagementService called MergeOrUploadDocuments (entityEntries)");

            var builders = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Builder)).Select(ee => ((Builder)ee.Entity)).ToList();
            builders.AddRange(entityEntries.Where(ee => ee.Entity.GetType() == typeof(BuilderEducationerAiep)).Select(ee => ((BuilderEducationerAiep)ee.Entity).Builder).ToList());
            if (builders.Any())
            {
                MergeOrUploadDocuments(GetDocuments<OmniSearchBuilderIndexModel, Builder>(builders));
            }

            var plans = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Plan)).Select(ee => ((Plan)ee.Entity)).ToList();
            if (plans.Any())
            {
                MergeOrUploadDocuments(GetDocuments<OmniSearchPlanIndexModel, Plan>(plans));
            }

            var versions = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Domain.Entity.Version)).Select(ee => ((Domain.Entity.Version)ee.Entity)).ToList();

            if (versions.Any())
            {
                MergeOrUploadDocuments(GetDocuments<OmniSearchVersionIndexModel, Domain.Entity.Version>(versions));
            }

            var endUsers = entityEntries.Where(ee => ee.Entity.GetType() == typeof(EndUser)).Select(ee => ((EndUser)ee.Entity)).ToList();

            if (endUsers.Any())
            {
                MergeOrUploadDocuments(GetDocuments<OmniSearchEndUserIndexModel, EndUser>(endUsers));
            }

            var users = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Domain.Entity.User)).Select(ee => ((Domain.Entity.User)ee.Entity)).ToList();
            if (users.Any())
            {
                MergeOrUploadDocuments(GetDocuments<OmniSearchUserIndexModel, Domain.Entity.User>(users));
                typesAndIds.Add(typeof(OmniSearchUserIndexModel), users.Select(u => u.Id).ToList());
            }
            
            var projects = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Domain.Entity.Project)).Select(ee => ((Domain.Entity.Project)ee.Entity)).ToList();
            if (projects.Any())
            {
                MergeOrUploadDocuments(GetDocuments<OmniSearchProjectIndexModel, Domain.Entity.Project>(projects));
                typesAndIds.Add(typeof(OmniSearchProjectIndexModel), users.Select(u => u.Id).ToList());
            }

            logger.LogDebug("AzureSearchManagementService end call MergeOrUploadDocuments (entityEntries)");
            return typesAndIds;
        }

        public void MergeOrUploadISearchableDocumentsByUser(DataContext dbContext, Dictionary<Type, List<int>> documentIdsWithAclsToUpdate)
        {
            logger.LogDebug("AzureSearchManagementService called MergeOrUploadAclsInDocuments");

            if (documentIdsWithAclsToUpdate.ContainsKey(typeof(OmniSearchBuilderIndexModel)))
            {
                var builderEntities = dbContext.Set<Builder>().Where(b => documentIdsWithAclsToUpdate[typeof(OmniSearchBuilderIndexModel)].Contains(b.Id)).ToList();
                MergeOrUploadDocuments(GetDocuments<OmniSearchBuilderIndexModel, Builder>(builderEntities));
            }

            if (documentIdsWithAclsToUpdate.ContainsKey(typeof(OmniSearchPlanIndexModel)))
            {
                var planEntities = dbContext.Set<Plan>().Where(p => documentIdsWithAclsToUpdate[typeof(OmniSearchPlanIndexModel)].Contains(p.Id)).ToList();
                MergeOrUploadDocuments(GetDocuments<OmniSearchPlanIndexModel, Plan>(planEntities));
            }

            if (documentIdsWithAclsToUpdate.ContainsKey(typeof(OmniSearchVersionIndexModel)))
            {
                var versionEntities = dbContext.Set<Domain.Entity.Version>().Where(v => documentIdsWithAclsToUpdate[typeof(OmniSearchVersionIndexModel)].Contains(v.Id)).ToList();
                MergeOrUploadDocuments(GetDocuments<OmniSearchVersionIndexModel, Domain.Entity.Version>(versionEntities));
            }

            if (documentIdsWithAclsToUpdate.ContainsKey(typeof(OmniSearchEndUserIndexModel)))
            {
                var endUserEntities = dbContext.Set<EndUser>().Where(eu => documentIdsWithAclsToUpdate[typeof(OmniSearchEndUserIndexModel)].Contains(eu.Id)).ToList();
                MergeOrUploadDocuments(GetDocuments<OmniSearchEndUserIndexModel, EndUser>(endUserEntities));
            }
            
            if (documentIdsWithAclsToUpdate.ContainsKey(typeof(OmniSearchProjectIndexModel)))
            {
                var projectEntities = dbContext.Set<Project>().Where(p => documentIdsWithAclsToUpdate[typeof(OmniSearchProjectIndexModel)].Contains(p.Id)).ToList();
                MergeOrUploadDocuments(GetDocuments<OmniSearchProjectIndexModel, Project>(projectEntities));
            }
            logger.LogDebug("AzureSearchManagementService end call MergeOrUploadAclsInDocuments");
        }


        public IEnumerable<TModel> GetDocuments<TModel, TEntity>(List<TEntity> entities) where TModel : SearchBaseIndexModel where TEntity : ISearchable<int>
        {
            logger.LogDebug("AzureSearchManagementService called GetDocuments"); 
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var dbContext = currentServiceProvider.GetRequiredService<DataContext>();
            var entityIds = entities.Select(b => b.Id).ToList();
            var entityAcls = new Dictionary<string, IEnumerable<int>>();
            var AiepIdsByBuilderId = new List<Tuple<int, IEnumerable<int>>>();
            var AiepIdsByPlanId = new List<Tuple<int, IEnumerable<int>>>();
            var AiepIdsByVersionId = new List<Tuple<int, IEnumerable<int>>>();
            var AiepIdsByEndUserId = new List<Tuple<int, IEnumerable<int>>>();
            var AiepIdsByProjectId = new List<Tuple<int, IEnumerable<int>>>();

            if (typeof(TEntity) != typeof(Domain.Entity.User))
            {
                entityAcls = dbContext.Set<Acl>().Where(acl => acl.EntityType == typeof(TEntity).Name && entityIds.Contains(acl.EntityId)).AsEnumerable()
                                             .GroupBy(acl => acl.EntityId).ToDictionary(g => g.Key.ToString(), g => g.Select(a => a.UserId));
            }


            if (typeof(TEntity) == typeof(Builder))
            {
                var allBuilderAiepIds = dbContext.Set<BuilderEducationerAiep>().IgnoreQueryFilters()
                                                                       .Where(bdd => entityIds.Contains(bdd.BuilderId))
                                                                       .Select(bdd => Tuple.Create(bdd.BuilderId, bdd.AiepId)).ToList();

                AiepIdsByBuilderId = allBuilderAiepIds.GroupBy(bd => bd.Item1).Select(g => Tuple.Create(g.Key, g.Select(ids => ids.Item2))).ToList();
            }

            if (typeof(TEntity) == typeof(Plan))
            {
                var allPlanAiepIds = dbContext.Set<Plan>().IgnoreQueryFilters()
                                                    .Where(p => entityIds.Contains(p.Id))
                                                    .Select(p => Tuple.Create(p.Id, p.Project.AiepId)).ToList();

                AiepIdsByPlanId = allPlanAiepIds.GroupBy(bd => bd.Item1).Select(g => Tuple.Create(g.Key, g.Select(ids => ids.Item2))).ToList();
            }


            if (typeof(TEntity) == typeof(Domain.Entity.Version))
            {
                var allVersionAiepIds = dbContext.Set<Domain.Entity.Version>().IgnoreQueryFilters()
                                                                        .Where(v => entityIds.Contains(v.Id))
                                                                        .Select(v => Tuple.Create(v.Id, v.Plan.Project.AiepId)).ToList();

                AiepIdsByVersionId = allVersionAiepIds.GroupBy(bd => bd.Item1).Select(g => Tuple.Create(g.Key, g.Select(ids => ids.Item2))).ToList();
            }

            if (typeof(TEntity) == typeof(EndUser))
            {
                var allEndUserAiepIds = dbContext.Set<EndUserAiep>().IgnoreQueryFilters()
                                                               .Where(eud => entityIds.Contains(eud.EndUserId))
                                                               .Select(eud => Tuple.Create(eud.EndUserId, eud.AiepId)).ToList();

                AiepIdsByEndUserId = allEndUserAiepIds.GroupBy(bd => bd.Item1).Select(g => Tuple.Create(g.Key, g.Select(ids => ids.Item2))).ToList();
            }
            
            if (typeof(TEntity) == typeof(Project))
            {
                var allProjectAiepIds = dbContext.Set<Project>().IgnoreQueryFilters()
                                                               .Where(p => entityIds.Contains(p.Id))
                                                               .Select(p => Tuple.Create(p.Id, p.AiepId)).ToList();

                AiepIdsByProjectId = allProjectAiepIds.GroupBy(bd => bd.Item1).Select(g => Tuple.Create(g.Key, g.Select(ids => ids.Item2))).ToList();
            }


            var omniSearchIndexModels = this.mapper.Map(entities, new List<TModel>());

            foreach (var model in omniSearchIndexModels)
            {
                if (entityAcls.ContainsKey(model.Id))
                {
                    model.Acls = entityAcls[model.Id];
                }
                if (model.GetType() == typeof(OmniSearchBuilderIndexModel))
                {
                    var builderAiepIds = AiepIdsByBuilderId.FirstOrDefault(bd => bd.Item1 == int.Parse(model.Id));
                    if (builderAiepIds.IsNotNull())
                    {
                        model.AiepIds = builderAiepIds.Item2;
                    }
                }
                if (model.GetType() == typeof(OmniSearchPlanIndexModel))
                {
                    var planAiepIds = AiepIdsByPlanId.FirstOrDefault(pd => pd.Item1 == int.Parse(model.Id));
                    if (planAiepIds.IsNotNull())
                    {
                        model.AiepIds = planAiepIds.Item2;
                    }
                }
                if (model.GetType() == typeof(OmniSearchVersionIndexModel))
                {
                    var versionAiepIds = AiepIdsByVersionId.FirstOrDefault(vd => vd.Item1 == int.Parse(model.Id));
                    if (versionAiepIds.IsNotNull())
                    {
                        model.AiepIds = versionAiepIds.Item2;
                    }
                }
                if (model.GetType() == typeof(OmniSearchEndUserIndexModel))
                {
                    var endUserAiepIds = AiepIdsByEndUserId.FirstOrDefault(eud => eud.Item1 == int.Parse(model.Id));
                    if (endUserAiepIds.IsNotNull())
                    {
                        model.AiepIds = endUserAiepIds.Item2;
                    }
                }
                if (model.GetType() == typeof(OmniSearchProjectIndexModel))
                {
                    var projectAiepIds = AiepIdsByProjectId.FirstOrDefault(eud => eud.Item1 == int.Parse(model.Id));
                    if (projectAiepIds.IsNotNull())
                    {
                        model.AiepIds = projectAiepIds.Item2;
                    }
                }
            }

            logger.LogDebug("AzureSearchManagementService end call GetDocuments -> List of TModel");

            return omniSearchIndexModels;
        }


        public void DeleteDocuments(Dictionary<Type, IEnumerable<int>> documentTypesAndIds)
        {
            logger.LogDebug("AzureSearchManagementService called DeleteDocuments");

            foreach (var typeAndIds in documentTypesAndIds)
            {
                if (typeAndIds.Value.Any())
                {
                    var searchBaseIndexModel = new SearchBaseIndexModel();
                    var config = GetSearchConfigByType(typeAndIds.Key);

                    var indexClient = SearchServiceClient.Indexes.GetClient(config.IndexName);
                    logger.LogDebug("AzureSearchManagementService DeleteDocuments end call Indexes.GetClient");

                    var indexBatch = IndexBatch.Delete(nameof(searchBaseIndexModel.Id), typeAndIds.Value.Select(id => id.ToString()));
                    logger.LogDebug("AzureSearchManagementService DeleteDocuments end call IndexBatch.Delete");

                    indexClient.Documents.IndexAsync(indexBatch);
                    logger.LogDebug("AzureSearchManagementService DeleteDocuments end call Documents.IndexAsync");
                }
            }
            logger.LogDebug("AzureSearchManagementService end call DeleteDocuments");
        }

        public Dictionary<Type, List<int>> GetISearchableDocumentsByUserToBeUpdated(DataContext dbContext, Dictionary<Type, IEnumerable<int>> typesAndIds)
        {
            logger.LogDebug("AzureSearchManagementService called GetDocumentIdsWithAclsToBeUpdated");
            Dictionary<Type, List<int>> documentTypesAndIds = new Dictionary<Type, List<int>>();
            if (typesAndIds.ContainsKey(typeof(OmniSearchUserIndexModel)))
            {
                var aclEntities = dbContext.Set<Acl>().Where(acl => typesAndIds[typeof(OmniSearchUserIndexModel)].Contains(acl.UserId)).AsEnumerable()
                .GroupBy(acl => acl.EntityType).ToDictionary(g => g.Key.ToString(), g => g.Select(a => a.EntityId));

                if (aclEntities.ContainsKey(nameof(Builder)))
                    documentTypesAndIds.Add(typeof(OmniSearchBuilderIndexModel), aclEntities[nameof(Builder)].ToList());
                if (aclEntities.ContainsKey(nameof(Plan)))
                    documentTypesAndIds.Add(typeof(OmniSearchPlanIndexModel), aclEntities[nameof(Plan)].ToList());
                if (aclEntities.ContainsKey(nameof(Domain.Entity.Version)))
                    documentTypesAndIds.Add(typeof(OmniSearchVersionIndexModel), aclEntities[nameof(Domain.Entity.Version)].ToList());
                if (aclEntities.ContainsKey(nameof(EndUser)))
                    documentTypesAndIds.Add(typeof(OmniSearchEndUserIndexModel), aclEntities[nameof(EndUser)].ToList());
                if (aclEntities.ContainsKey(nameof(Project)))
                    documentTypesAndIds.Add(typeof(OmniSearchProjectIndexModel), aclEntities[nameof(Project)].ToList());
            }
            logger.LogDebug("AzureSearchManagementService end call GetDocumentIdsWithAclsToBeUpdated");
            return documentTypesAndIds;
        }

        private SearchPropertiesConfiguration GetSearchConfigByType(Type type)
        {
            logger.LogDebug("AzureSearchManagementService called GetSearchConfigByType");

            switch (type.Name)
            {
                case nameof(OmniSearchBuilderIndexModel):

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return Builder search config");

                    return azureSearchConfiguration.BuilderSearchConfig;
                case nameof(OmniSearchPlanIndexModel):

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return Plan search config");

                    return azureSearchConfiguration.PlanSearchConfig;
                case nameof(OmniSearchVersionIndexModel):

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return Version search config");

                    return azureSearchConfiguration.VersionSearchConfig;
                case nameof(OmniSearchEndUserIndexModel):

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return EndUser search config");

                    return azureSearchConfiguration.EndUserSearchConfig;
                case nameof(OmniSearchUserIndexModel):

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return User search config");

                    return azureSearchConfiguration.UserSearchConfig;
                case nameof(OmniSearchProjectIndexModel):

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return Project search config");

                    return azureSearchConfiguration.ProjectSearchConfig;
                default:

                    logger.LogDebug("AzureSearchManagementService end call GetSearchConfigByType -> return Null");

                    return null;
            }
        }

        public Dictionary<Type, IEnumerable<int>> GetDeletedOrUpdatedDocumentsIds(DataContext dbContext, List<EntityEntry> entityEntries)
        {
            logger.LogDebug("AzureSearchManagementService called GetDeletedDocumentsIds");

            Dictionary<Type, IEnumerable<int>> typesAndIds = new Dictionary<Type, IEnumerable<int>>();
            var buildersIds = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Builder)).Select(ee => ((Builder)ee.Entity).Id).ToList();
            var plansIds = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Plan)).Select(ee => ((Plan)ee.Entity).Id).ToList();
            var versionsIds = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Domain.Entity.Version)).Select(ee => ((Domain.Entity.Version)ee.Entity).Id).ToList();
            var endUsersIds = entityEntries.Where(ee => ee.Entity.GetType() == typeof(EndUser)).Select(ee => ((EndUser)ee.Entity).Id).ToList();
            var usersIds = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Domain.Entity.User)).Select(ee => ((Domain.Entity.User)ee.Entity).Id).ToList();
            var projectsIds = entityEntries.Where(ee => ee.Entity.GetType() == typeof(Domain.Entity.Project)).Select(ee => ((Domain.Entity.Project)ee.Entity).Id).ToList();

            if (buildersIds.Any())
            {
                typesAndIds.Add(typeof(OmniSearchBuilderIndexModel), dbContext.Set<Builder>().Where(bi => buildersIds.Contains(bi.Id)).Select(b => b.Id).ToList());
            }

            if (plansIds.Any())
            {
                typesAndIds.Add(typeof(OmniSearchPlanIndexModel), dbContext.Set<Plan>().Where(bi => plansIds.Contains(bi.Id)).Select(b => b.Id).ToList());
            }

            if (versionsIds.Any())
            {
                typesAndIds.Add(typeof(OmniSearchVersionIndexModel), dbContext.Set<Domain.Entity.Version>().Where(bi => versionsIds.Contains(bi.Id)).Select(b => b.Id).ToList());
            }

            if (endUsersIds.Any())
            {
                typesAndIds.Add(typeof(OmniSearchEndUserIndexModel), dbContext.Set<EndUser>().Where(bi => endUsersIds.Contains(bi.Id)).Select(b => b.Id).ToList());
            }

            if (usersIds.Any())
            {
                typesAndIds.Add(typeof(OmniSearchUserIndexModel), dbContext.Set<Domain.Entity.User>().Where(bi => usersIds.Contains(bi.Id)).Select(b => b.Id));
            }
            
            if (usersIds.Any())
            {
                typesAndIds.Add(typeof(OmniSearchProjectIndexModel), dbContext.Set<Domain.Entity.Project>().Where(bi => usersIds.Contains(bi.Id)).Select(b => b.Id));
            }

            logger.LogDebug("AzureSearchManagementService end call GetDeletedDocumentsIds -> return Dictionary<Type, List of Int>");

            return typesAndIds;
        }
    }
}



