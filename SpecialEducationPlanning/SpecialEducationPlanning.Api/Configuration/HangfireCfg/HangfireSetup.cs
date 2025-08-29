using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{

    public static class HangfireSetup
    {
        #region Methods Public

        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue("Hangfire:Enabled", false))
            {
                var connectionString = configuration.GetValue("Hangfire:DefaultConnectionString", "");

                services.AddHangfire(configurationHangfire => configurationHangfire
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString,
                        new SqlServerStorageOptions
                        {
                            //TODO move to config
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.FromMinutes(3),
                            UseRecommendedIsolationLevel = true,
                            UsePageLocksOnDequeue = true,
                            DisableGlobalLocks = true
                        }));
                //services.AddHangfireServer();
            }
            else
            {
                services.AddSingleton<IBackgroundJobClient, NullBackgroundJobClient>();
            }
        }

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration, ILogger logger)
        {
            if (configuration.GetValue("Hangfire:Enabled", false))
            {
                logger.LogDebug("Configuring Hangfire...");

                if (configuration.GetValue("Hangfire:DashboardEnabled", false))
                {
                    logger.LogDebug("Configuring Hangfire Dashboard...");

                    app.UseHangfireDashboard("/jobs", new DashboardOptions
                    {
                        Authorization = new[] { new HangfireAuthFilter() },
                        IsReadOnlyFunc = context => true,
                        AppPath = null,
                        IgnoreAntiforgeryToken = true
                    });
                }

                logger.LogDebug("Configuring Hangfire SetupJobs...");
                HangfireJobs.UpdateJobs(app, logger);
                FittersPackHangfireJobsSetup.UpdateJobs(logger, configuration);
                HangfireSearchJobs.UpdateJobs(app, logger,
                    configuration.GetValue("Hangfire:IndexerDelay", 1000),
                    configuration.GetValue("Hangfire:IndexerSchedule", new TimeSpan(1, 0, 0)),
                    configuration.GetValue("Hangfire:IndexerWindowInDays", 0),
                    configuration.GetValue("Hangfire:IndexerEnabled", false));

                if (configuration.GetValue("Hangfire:ServerEnabled", false))
                {
                    string workerCountAsString = configuration.GetValue("Hangfire:WorkerCount", "1");
                    bool validInt = int.TryParse(workerCountAsString, out int workerCount);
                    if (!validInt) workerCount = 1;

                    app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = workerCount });
                }
            }
        }

        //public static void MapHangfireDashboard(this IEndpointRouteBuilder endpoints, IConfiguration configuration, ILogger logger)
        //{
        //    if (configuration.GetValue("Hangfire:Enabled", false))
        //    {
        //        logger.LogDebug("Configuring Hangfire...");

        //        if (configuration.GetValue("Hangfire:DashboardEnabled", false))
        //        {
        //            logger.LogDebug("Configuring Hangfire Dashboard...");

        //            endpoints.MapHangfireDashboard("/jobs", new DashboardOptions
        //            {
        //                Authorization = new[] { new HangfireAuthFilter() },
        //                IsReadOnlyFunc = context => true,
        //                AppPath = null,
        //                IgnoreAntiforgeryToken = true
        //            });
        //        }
        //    }
        //}


        #endregion

    }

}