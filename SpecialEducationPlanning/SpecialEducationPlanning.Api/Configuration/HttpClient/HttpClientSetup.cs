using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Polly.Contrib.WaitAndRetry;

namespace SpecialEducationPlanning
.Api.Configuration.HttpClient
{
    public static class HttpClientSetup
    {
        public static void ConfigurePolicies(this IHttpClientBuilder builder, IConfiguration configuration) 
        {
            var httpClientConfig = configuration.GetSection(HttpClientConfiguration.SectionName).Get<HttpClientConfiguration>();

            builder.SetHandlerLifetime(TimeSpan.FromMinutes(httpClientConfig.MessageHandlerLifetime))
                   .AddPolicyHandler(GetRetryPolicy(httpClientConfig.RetryPolicy.RetryCount));
        }


        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), retryCount));
        }
    }
}
