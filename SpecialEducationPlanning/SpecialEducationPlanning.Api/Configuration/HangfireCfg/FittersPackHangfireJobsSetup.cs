using System.Linq;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{
    public static class FittersPackHangfireJobsSetup
    {
        private const string DefaultHangfireOverdueRetrySchedule = "0 * * * *";

        public static void UpdateJobs(
            ILogger logger,
            IConfiguration configuration)
        {
            const string jobId = "Overdue Retry Fitters Pack";
            logger.LogDebug($"Setup Job {jobId}");

            var currentJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            var existing = currentJobs.Any(j => j.Id == jobId);

            logger.LogInformation(existing ? $"Updating job '{jobId}'." : $"Creating job '{jobId}'.");

            var cronExpression = configuration.GetValue(
                "FittersPack:HangfireOverdueRetrySchedule", DefaultHangfireOverdueRetrySchedule);

            RecurringJob.AddOrUpdate<FittersPackHangfireJobs>(
                jobId,
                job => job.AutomaticOverdueRetryFittersPackAsync(),
                cronExpression);

            logger.LogInformation($"Job '{jobId}' added or updated with CRON expression '{cronExpression}'.");
        }
    }
}
