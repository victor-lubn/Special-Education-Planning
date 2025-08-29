using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{

    public static class HangfireSearchJobs
    {

        private static int indexerDelay;

        private static TimeSpan indexerSchedule;

        private static ILogger logger;

        private static IServiceProvider serviceProvider;
        private static bool customerIndexerEnabled;

        private const string customIndexerJobName = "CustomAzureSearchIndexer";

        private const int pageSizeConstant = 1000;

        #region Methods Public
        public static async Task UpdateJobs(IApplicationBuilder app, ILogger loggerStartup, int delay, TimeSpan schedule, int indexerWindowInDays, bool indexerEnabled)
        {
            serviceProvider = app.ApplicationServices;
            logger = loggerStartup;
            indexerDelay = delay;
            indexerSchedule = schedule;
            customerIndexerEnabled = indexerEnabled;

            CreateRecurringIndexer(indexerWindowInDays);
        }

        public static async Task RunCustomIndexer(DateTime? updateDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays, bool forceRun = false)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;

            // Fail-safe. Logger should ALWAYS be set before this due to it being set in this static instance via api startup.
            // This is here purely as a fail-safe when being executed via AdminController.
            logger ??= currentServiceProvider.GetRequiredService<ILogger>();

            logger.LogInformation("HangfireSearchJobs -> RunCustomIndexer Hit.");

            var configuration = currentServiceProvider.GetRequiredService<IConfiguration>();
            var azureSearchEnabled = configuration.GetValue("AzureSearch:Enabled", false);

            if (azureSearchEnabled && (forceRun || customerIndexerEnabled))
            {
                logger.LogInformation("HangfireSearchJobs -> Indexer Enabled. Running now.");
                await PlanAutomaticIndexing(updateDate, pageSize, indexerDelay, indexerWindowInDays);
                await BuilderAutomaticIndexing(updateDate, pageSize, indexerDelay, indexerWindowInDays);
                await VersionAutomaticIndexing(updateDate, pageSize, indexerDelay, indexerWindowInDays);
                await EndUserAutomaticIndexing(updateDate, pageSize, indexerDelay, indexerWindowInDays);
                await UserAutomaticIndexing(updateDate, pageSize, indexerDelay, indexerWindowInDays);
                await ProjectAutomaticIndexing(updateDate, pageSize, indexerDelay, indexerWindowInDays);
            }

            logger.LogInformation("HangfireSearchJobs -> RunCustomIndexer Exit.");
        }

        private static void CreateRecurringIndexer(int indexerWindowInDays)
        {
            var currentJobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            var existing = currentJobs.Any(j => j.Id == customIndexerJobName);

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var configuration = currentServiceProvider.GetRequiredService<IConfiguration>();
            var azureSearchEnabled = configuration.GetValue("AzureSearch:Enabled", false);

            if (!existing && customerIndexerEnabled && azureSearchEnabled)
            {
                logger.LogInformation($"Created {customIndexerJobName} job");
                RecurringJob.AddOrUpdate(customIndexerJobName, () => RunCustomIndexer(null, null, null, indexerWindowInDays, false),
                Cron.Daily(indexerSchedule.Hours, indexerSchedule.Minutes));
            }
        }


        private static async Task BuilderAutomaticIndexing(DateTime? updatedDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            logger.LogInformation("HangfireSearchJobs BuilderAutomaticIndexing -> Hit");

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var builderRepository = currentServiceProvider.GetRequiredService<IBuilderRepository>();
            var count = await builderRepository.CountNoFiltersAsync();
            var configuration = currentServiceProvider.GetRequiredService<IConfiguration>();
            int take = pageSize ?? configuration.GetValue("AzureSearch:HangFireAzurePageSize", pageSizeConstant);
            var timesLooping = count / take;
            var rest = count % take;
            var skip = 0;

            logger.LogInformation("HangfireSearchJobs BuilderAutomaticIndexing -> Take Value is {take}. Looping {loops} with {left} remaining", take, timesLooping, rest);

            IBackgroundJobClient client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            try
            {
                for (var i = 0; i < timesLooping; i++)
                {
                    client.Enqueue(() => BuilderAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                    skip += take;
                }

                if (rest != 0)
                {
                    client.Enqueue(() => BuilderAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HangfireSearchJobs BuilderAutomaticIndexing -> Error {ex}", ex.Message);
            }

            logger.LogInformation("HangfireSearchJobs BuilderAutomaticIndexing -> Exit");
        }

        // MUST BE PUBLIC FOR HANGFIRE TO FIND IT.
        public static async Task BuilderAutomaticIndexingPage(int take, int skip, DateTime? updatedDate, int? customIndexerDelay, int? indexerWindowInDays)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var builderRepository = currentServiceProvider.GetRequiredService<IBuilderRepository>();
            await builderRepository.CallIndexerAsync(take, skip, updatedDate, indexerWindowInDays);
        }


        private static async Task EndUserAutomaticIndexing(DateTime? updatedDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            logger.LogInformation("HangfireSearchJobs EndUserAutomaticIndexing -> Hit");

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var endUserRepository = currentServiceProvider.GetRequiredService<IEndUserRepository>();
            var count = await endUserRepository.CountNoFiltersAsync();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            int take = pageSize ?? configuration.GetValue("AzureSearch:HangFireAzurePageSize", pageSizeConstant);
            var timesLooping = count / take;
            var rest = count % take;
            var skip = 0;

            logger.LogInformation("HangfireSearchJobs EndUserAutomaticIndexing -> Take Value is {take}. Looping {loops} with {left} remaining", take, timesLooping, rest);

            IBackgroundJobClient client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            try
            {
                for (var i = 0; i < timesLooping; i++)
                {
                    client.Enqueue(() => EndUserAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                    skip += take;
                }

                if (rest != 0)
                {
                    client.Enqueue(() => EndUserAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HangfireSearchJobs EndUserAutomaticIndexing -> Error {ex}", ex.Message);
            }

            logger.LogInformation("HangfireSearchJobs EndUserAutomaticIndexing -> Exit");
        }

        // MUST BE PUBLIC FOR HANGFIRE TO FIND IT.
        public static async Task EndUserAutomaticIndexingPage(int take, int skip, DateTime? updatedDate, int? customIndexerDelay, int? indexerWindowInDays)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var endUserRepository = currentServiceProvider.GetRequiredService<IEndUserRepository>();
            await endUserRepository.CallIndexerAsync(take, skip, updatedDate, indexerWindowInDays);
        }


        private static async Task PlanAutomaticIndexing(DateTime? updatedDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            logger.LogInformation("HangfireSearchJobs PlanAutomaticIndexing -> Hit");

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var planRepository = currentServiceProvider.GetRequiredService<IPlanRepository>();
            var count = await planRepository.CountNoFiltersAsync();
            var configuration = currentServiceProvider.GetRequiredService<IConfiguration>();
            int take = pageSize ?? configuration.GetValue("AzureSearch:HangFireAzurePageSize", pageSizeConstant);
            var timesLooping = count / take;
            var rest = count % take;
            var skip = 0;

            logger.LogInformation("HangfireSearchJobs PlanAutomaticIndexing -> Take Value is {take}. Looping {loops} with {left} remaining", take, timesLooping, rest);

            IBackgroundJobClient client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            try
            {
                for (var i = 0; i < timesLooping; i++)
                {
                    client.Enqueue(() => PlanAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                    skip += take;
                }

                if (rest != 0)
                {
                    client.Enqueue(() => PlanAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HangfireSearchJobs PlanAutomaticIndexing -> Error {ex}", ex.Message);
            }

            logger.LogInformation("HangfireSearchJobs PlanAutomaticIndexing -> Exit");
        }

        // MUST BE PUBLIC FOR HANGFIRE TO FIND IT.
        public static async Task PlanAutomaticIndexingPage(int take, int skip, DateTime? updatedDate, int? customIndexerDelay, int? indexerWindowInDays)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var planRepository = currentServiceProvider.GetRequiredService<IPlanRepository>(); 
            await planRepository.CallIndexerAsync(take, skip, updatedDate, indexerWindowInDays);
        }

        private static async Task ProjectAutomaticIndexing(DateTime? updatedDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            logger.LogInformation("HangfireSearchJobs ProjectAutomaticIndexing -> Hit");

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var projectRepository = currentServiceProvider.GetRequiredService<IProjectRepository>();
            var count = await projectRepository.CountNoFiltersAsync();
            var configuration = currentServiceProvider.GetRequiredService<IConfiguration>();
            int take = pageSize ?? configuration.GetValue("AzureSearch:HangFireAzurePageSize", pageSizeConstant);
            var timesLooping = count / take;
            var rest = count % take;
            var skip = 0;

            logger.LogInformation("HangfireSearchJobs ProjectAutomaticIndexing -> Take Value is {take}. Looping {loops} with {left} remaining", take, timesLooping, rest);

            IBackgroundJobClient client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            try
            {
                for (var i = 0; i < timesLooping; i++)
                {
                    client.Enqueue(() => ProjectAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                    skip += take;
                }

                if (rest != 0)
                {
                    client.Enqueue(() => ProjectAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HangfireSearchJobs ProjectAutomaticIndexing -> Error {ex}", ex.Message);
            }

            logger.LogInformation("HangfireSearchJobs ProjectAutomaticIndexing -> Exit");
        }

        // MUST BE PUBLIC FOR HANGFIRE TO FIND IT.
        public static async Task ProjectAutomaticIndexingPage(int take, int skip, DateTime? updatedDate, int? customIndexerDelay, int? indexerWindowInDays)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var projectRepository = currentServiceProvider.GetRequiredService<IProjectRepository>();
            await projectRepository.CallIndexerAsync(take, skip, updatedDate, indexerWindowInDays);
        }

        private static async Task UserAutomaticIndexing(DateTime? updatedDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            logger.LogInformation("HangfireSearchJobs UserAutomaticIndexing -> Hit");

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var userRepository = currentServiceProvider.GetRequiredService<IUserRepository>();
            var count = await userRepository.CountNoFiltersAsync();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            int take = pageSize ?? configuration.GetValue("AzureSearch:HangFireAzurePageSize", pageSizeConstant);
            var timesLooping = count / take;
            var rest = count % take;
            var skip = 0;

            logger.LogInformation("HangfireSearchJobs UserAutomaticIndexing -> Take Value is {take}. Looping {loops} with {left} remaining", take, timesLooping, rest);

            IBackgroundJobClient client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            try
            {
                for (var i = 0; i < timesLooping; i++)
                {
                    client.Enqueue(() => UserAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                    skip += take;
                }

                if (rest != 0)
                {
                    client.Enqueue(() => UserAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HangfireSearchJobs UserAutomaticIndexing -> Error {ex}", ex.Message);
            }

            logger.LogInformation("HangfireSearchJobs UserAutomaticIndexing -> Exit");
        }

        // MUST BE PUBLIC FOR HANGFIRE TO FIND IT.
        public static async Task UserAutomaticIndexingPage(int take, int skip, DateTime? updatedDate, int? customIndexerDelay, int? indexerWindowInDays)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var userRepository = currentServiceProvider.GetRequiredService<IUserRepository>();
            await userRepository.CallIndexerAsync(take, skip, updatedDate, indexerWindowInDays);
        }


        private static async Task VersionAutomaticIndexing(DateTime? updatedDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            logger.LogInformation("HangfireSearchJobs VersionAutomaticIndexing -> Hit");

            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var versionRepository = currentServiceProvider.GetRequiredService<IVersionRepository>();
            var count = await versionRepository.CountNoFiltersAsync();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            int take = pageSize ?? configuration.GetValue("AzureSearch:HangFireAzurePageSize", pageSizeConstant);
            var timesLooping = count / take;
            var rest = count % take;
            var skip = 0;

            logger.LogInformation("HangfireSearchJobs VersionAutomaticIndexing -> Take Value is {take}. Looping {loops} with {left} remaining", take, timesLooping, rest);

            IBackgroundJobClient client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            try
            {
                for (var i = 0; i < timesLooping; i++)
                {
                    client.Enqueue(() => VersionAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                    skip += take;
                }

                if (rest != 0)
                {
                    client.Enqueue(() => VersionAutomaticIndexingPage(take, skip, updatedDate, indexerDelay, indexerWindowInDays));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "HangfireSearchJobs VersionAutomaticIndexing -> Error {ex}", ex.Message);
            }

            logger.LogInformation("HangfireSearchJobs VersionAutomaticIndexing -> Exit");
        }

        // MUST BE PUBLIC FOR HANGFIRE TO FIND IT.
        public static async Task VersionAutomaticIndexingPage(int take, int skip, DateTime? updatedDate, int? customIndexerDelay, int? indexerWindowInDays)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var versionRepository = currentServiceProvider.GetRequiredService<IVersionRepository>();
            await versionRepository.CallIndexerAsync(take, skip, updatedDate, indexerWindowInDays);
        }
        
        #endregion
    }

}