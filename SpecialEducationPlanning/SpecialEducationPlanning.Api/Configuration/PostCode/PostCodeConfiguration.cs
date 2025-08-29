using System;
using System.Collections.Generic;
using System.Text;

namespace SpecialEducationPlanning
.Api.Configuration.PostCode
{
    public class PostCodeConfiguration
    {
        /// <summary>
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SearchAddressEndpoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Dataset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Take { get; set; }
    }
}
