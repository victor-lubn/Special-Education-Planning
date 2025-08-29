using System;
using System.Collections.Generic;
using System.Text;

namespace SpecialEducationPlanning
.Api.Configuration.HttpClient
{
    class HttpClientConfiguration
    {
        public const string SectionName = "HttpClient";
        public int MessageHandlerLifetime { get; set; } 
        public RetryPolicy RetryPolicy { get; set; }
    }
}
