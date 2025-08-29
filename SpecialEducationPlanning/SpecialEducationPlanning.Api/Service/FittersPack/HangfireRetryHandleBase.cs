using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.HangfireCfg;


namespace SpecialEducationPlanning
.Api.Service.FittersPack
{
    public abstract class HangfireRetryHandlerBase<TContext>
        where TContext : class
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger _logger;
        private readonly HangfireRetryConfiguration _hangfireRetryConfiguration;

        protected HangfireRetryHandlerBase(
            IBackgroundJobClient backgroundJobClient,
            ILogger logger,
            HangfireRetryConfiguration options)
        {
            _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hangfireRetryConfiguration = options;
        }

        public async Task ExecuteWithRetryAsync(TContext context, int attempt)
        {
            try
            {
                await ExecuteAsync(context);

                // Log success
                _logger.LogInformation($"Task executed successfully for context: {context} on attempt {attempt + 1}.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Task failed for context: {context} on attempt {attempt + 1}: {ex.Message}");

                if (attempt + 1 > _hangfireRetryConfiguration.MaxRetries)
                {
                    _logger.LogError($"All {_hangfireRetryConfiguration.MaxRetries} retries have been exhausted for context: {context}.");

                    await OnFailAsync(context);
                    return;
                }

                _logger.LogInformation($"Scheduling retry for context: {context}, attempt {attempt + 2}, in {_hangfireRetryConfiguration.RetryDelay} .");
                _backgroundJobClient.Schedule(() =>
                        ExecuteWithRetryAsync(context, attempt + 1),
                    _hangfireRetryConfiguration.RetryDelay);
            }
        }

        public abstract Task ExecuteAsync(TContext context);

        public abstract Task OnFailAsync(TContext context);
    }
}
