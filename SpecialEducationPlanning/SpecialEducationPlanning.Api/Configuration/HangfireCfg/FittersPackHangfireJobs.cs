using Hangfire;
using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.FittersPack;
using SpecialEducationPlanning
.Domain.Enum;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{
    public class FittersPackHangfireJobs
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public FittersPackHangfireJobs(
            IServiceProvider serviceProvider,
            ILogger<FittersPackHangfireJobs> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public  async Task AutomaticOverdueRetryFittersPackAsync()
        {
            var reGeneratePeriodFrom = _configuration.GetValue("FittersPack:OverdueRetryPeriodFrom", TimeSpan.FromMinutes(60));
            var reGeneratePeriodTo = _configuration.GetValue("FittersPack:OverdueRetryPeriodTo", TimeSpan.FromMinutes(30));
            var fromDate = DateTime.UtcNow.Add(-reGeneratePeriodFrom);
            var toDate = DateTime.UtcNow.Add(-reGeneratePeriodTo);
            var pageSize = _configuration.GetValue("FittersPack:OverdueRetryPageSize", 50);
            var statuses = _configuration.GetSection("FittersPack:OverdueRetryStatuses").Get<List<string>>() ?? new List<string>{ nameof(FittersPackStatusEnum.Queued) };
            long lastId = 0;

            _logger.LogInformation($"Starting Regenerating Fitters Pack with PageSize = {pageSize}, FromDate = {fromDate}, ToDate = {toDate}.");
            await ProcessRecordsUsingLastId(statuses, fromDate, toDate, lastId, pageSize);
        }

        public async Task ProcessRecordsUsingLastId(
            IList<string> statuses,
            DateTime fromDate,
            DateTime toDate,
            long lastId,
            int pageSize)
        {
            using var currentScope = _serviceProvider.CreateScope();
            var currentServiceProvider = currentScope.ServiceProvider;

            var entityRepository = currentServiceProvider.GetRequiredService<IEntityRepository>();
            var fittersPackService = currentServiceProvider.GetRequiredService<IFittersPackService>();
            var client = currentServiceProvider.GetRequiredService<IBackgroundJobClient>();

            var spec = new Specification<Version>(p =>
                statuses.Contains(p.FittersPackStatus.Name)
                && p.FittersPack3DCRequestTime >= fromDate
                && p.FittersPack3DCRequestTime <= toDate
                && p.EducationTool3DCVersionId > 0
                && p.Id > lastId);
            var versions = await entityRepository
                .Where(spec)
                .IgnoreQueryFilters()
                .OrderBy(p => p.Id)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation(
                $"{nameof(FittersPackHangfireJobs)}{nameof(ProcessRecordsUsingLastId)} Processing {versions.Count} records (LastId: {lastId}).");

            if (versions.Count == 0)
            {
                return;
            }

            foreach (var version in versions)
            {
                await fittersPackService.GenerateFitterPackAsync(version.Id, version.EducationTool3DCVersionId.Value, FitterPackProcessType.Overdue);
                _logger.LogDebug(
                    $"{nameof(FittersPackHangfireJobs)}{nameof(ProcessRecordsUsingLastId)} Successfully processed version {version.Id}");
            }

            var newLastId = versions.Last().Id;

            _logger.LogInformation(
                $"Successfully processed page for Regenerating Fitters Pack. Re-queuing next job with LastId = {newLastId}.");

            client.Enqueue(() => ProcessRecordsUsingLastId(statuses, fromDate, toDate, newLastId, pageSize));
        }
    }
}
