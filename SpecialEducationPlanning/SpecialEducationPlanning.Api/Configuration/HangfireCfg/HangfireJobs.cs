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

    public static class HangfireJobs
    {

        private static ILogger logger;

        private static IServiceProvider serviceProvider;

        #region Methods Public

        public static async Task PlanAutomaticArchiveAndDelete(int AiepId)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var planRepository = currentServiceProvider.GetRequiredService<IPlanRepository>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var archiveDays = configuration.GetValue("Archive:Archive", 100);
            await planRepository.AutomaticNonTenderPackArchiveAsync(DateTime.UtcNow, archiveDays, AiepId);
            await planRepository.AutomaticTenderPackArchiveAsync(DateTime.UtcNow, archiveDays, AiepId);
            var deleteDays = configuration.GetValue("Archive:Delete", 100);
            await planRepository.AutomaticDeletion(deleteDays, DateTime.UtcNow, AiepId);
        }

        public static async Task ProjectAutomaticArchive(int AiepId)
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var projectRepository = currentServiceProvider.GetRequiredService<IProjectRepository>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var archiveDays = configuration.GetValue("Archive:Archive", 100);
            await projectRepository.AutomaticArchive(DateTime.UtcNow, archiveDays, AiepId);
        }

        public static async Task ActionLogsAutomaticDelete()
        {
            using IServiceScope currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var actionRepository = currentServiceProvider.GetRequiredService<IActionLogsRepository>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var deleteDays = configuration.GetValue("ActionLogs:Delete", 100);
            await actionRepository.AutomaticRemoveOldItems(DateTime.UtcNow, deleteDays);
        }

        public static async Task SystemLogsAutomaticDelete()
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var systemRepository = currentServiceProvider.GetRequiredService<ILogRepository>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var deleteDays = configuration.GetValue("SystemLogs:Delete", 100);
            await systemRepository.AutomaticRemoveOldItems(DateTime.UtcNow, deleteDays);
        }

        public static async Task UpdateJobs(IApplicationBuilder app, ILogger loggerStartup)
        {
            serviceProvider = app.ApplicationServices;
            logger = loggerStartup;
            await AiepPlans();
            await ActionLogs();
            await SystemLogs();
            await DeleteLeaversAndUpdatePlans();
        }

        #endregion


        #region Methods Private

        private static async Task AiepPlans()
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var AiepRepository = currentServiceProvider.GetRequiredService<IAiepRepository>();
            var AiepIds = (await AiepRepository.GetAllIdsIgnoreAcl()).Content;
            var currentJobs = JobStorage.Current.GetConnection().GetRecurringJobs();

            foreach (var AiepId in AiepIds)
            {
                logger.LogDebug($"Job Aiep-{AiepId}-PlansArchiveDelete");
                var existingPlanJob = currentJobs.Any(j => j.Id == $"Aiep-{AiepId}-PlansArchiveDelete");

                if (!existingPlanJob)
                {
                    logger.LogInformation($"Created Job Aiep-{AiepId}-PlansArchiveDelete");

                    RecurringJob.AddOrUpdate($"Aiep-{AiepId}-PlansArchiveDelete",
                        () => PlanAutomaticArchiveAndDelete(AiepId),
                        Cron.Daily);
                }

                var existingProjectJob = currentJobs.Any(j => j.Id == $"Aiep-{AiepId}-ProjectsArchiveDelete");
                if (!existingProjectJob)
                {
                    logger.LogInformation($"Created Job Aiep-{AiepId}-ProjectsArchiveDelete");

                    RecurringJob.AddOrUpdate($"Aiep-{AiepId}-ProjectsArchiveDelete",
                        () => ProjectAutomaticArchive(AiepId),
                        Cron.Daily);
                }
            }
        }

        private static async Task DeleteLeaversAndUpdatePlans()
        {            
            var currentJobs = JobStorage.Current.GetConnection().GetRecurringJobs();          
                logger.LogDebug($"Job Delete Leavers");
                var existing = currentJobs.Any(j => j.Id == $"Job Delete Leavers");

                if (!existing)
                {
                    logger.LogInformation($"Created Job Delete Leavers");

                    RecurringJob.AddOrUpdate($"Job Delete Leavers",
                        () => AutomaticDeleteLeavers(),
                        Cron.Daily(1,00));
                }          
        }

        public static async Task AutomaticDeleteLeavers()
        {
            using var currentScope = serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;
            var userRepository = currentServiceProvider.GetRequiredService<IUserRepository>();           
            await userRepository.AutomaticDeleteLeavers();
            
        }

        private static async Task ActionLogs()
        {
            logger.LogInformation($"Created Job ActionLog-Delete");
            RecurringJob.AddOrUpdate($"ActionLog-Delete",
                        () => ActionLogsAutomaticDelete(),
                        Cron.Daily);
        }

        private static async Task SystemLogs()
        {
            logger.LogInformation($"Created Job SystemLog-Delete");
            RecurringJob.AddOrUpdate($"SystemLog-Delete",
                        () => SystemLogsAutomaticDelete(),
                        Cron.Daily);
        }

        #endregion

    }
}
