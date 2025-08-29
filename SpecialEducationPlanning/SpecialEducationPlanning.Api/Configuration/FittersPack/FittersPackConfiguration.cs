using System.Collections.Generic;
using SpecialEducationPlanning
.Api.Configuration.HangfireCfg;

namespace SpecialEducationPlanning
.Api.Configuration.FittersPack
{
    public class FittersPackConfiguration
    {
        public bool Enabled { get; set; }

        public HangfireRetryConfiguration HangfireRetry { get; set; }

        public IList<string> OverdueRetryStatuses { get; set; }
    }
}
