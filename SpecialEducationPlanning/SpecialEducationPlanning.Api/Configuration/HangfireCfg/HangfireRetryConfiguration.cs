using System;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{
    public class HangfireRetryConfiguration
    {
        public int MaxRetries { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(5);
    }
}
