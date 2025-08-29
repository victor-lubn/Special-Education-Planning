using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Configuration.Base
{
    public class ApimConfigurationBase
    {
        public string BaseUrl { get; set; }
        public string ApiVersion { get; set; }
        public string TokenProviderUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrandType { get; set; }
        public string Scope { get; set; }
    }
}
