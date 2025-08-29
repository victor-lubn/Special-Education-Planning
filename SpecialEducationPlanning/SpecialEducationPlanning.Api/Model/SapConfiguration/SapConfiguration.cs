using System;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Model.SapConfiguration
{
    public class SapConfiguration
    {
        public SapConnectionCredentials SapConnectionCredentials { get; set; }
        public TimeSpan SapConnectionTimeout { get; set; }
        public int SapTake { get; set; }
        public IEnumerable<string> Companies { get; set; }
    }
}
