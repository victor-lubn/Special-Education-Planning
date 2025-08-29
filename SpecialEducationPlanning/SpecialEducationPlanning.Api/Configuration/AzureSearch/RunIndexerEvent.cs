using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Api.Configuration.HangfireCfg;

namespace SpecialEducationPlanning
.Api.Configuration.AzureSearch
{
    public record RunIndexerEvent(DateTime? UpdateDate, int? PageSize, int? IndexerDelay, int? IndexerWindowInDays) : INotification
    {
    }

    public record RunIndexerEventHandler : INotificationHandler<RunIndexerEvent>
    {
        private readonly ILogger<RunIndexerEventHandler> _logger;

        public RunIndexerEventHandler(ILogger<RunIndexerEventHandler> logger)
        {
            this._logger = logger;
        }

        public async Task Handle(RunIndexerEvent notification, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("RunIndexerEventHandler Handle -> Hit");

            await HangfireSearchJobs.RunCustomIndexer(notification.UpdateDate, notification.PageSize, notification.IndexerDelay, notification.IndexerWindowInDays);

            this._logger.LogInformation("RunIndexerEventHandler Handle -> Exit");
        }
    }
}
