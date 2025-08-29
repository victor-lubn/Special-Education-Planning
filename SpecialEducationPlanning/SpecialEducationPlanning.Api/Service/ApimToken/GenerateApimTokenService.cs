using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.Base;
using SpecialEducationPlanning
.Api.Service.DistributedCache;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.ApimToken
{
    public class GenerateApimTokenService : IGenerateApimTokenService
    {
        private readonly ILogger<GenerateApimTokenService> logger;
        private readonly IUserDistributedCacheService cacheService;
        private readonly HttpClient httpClient;

        public GenerateApimTokenService(
            ILogger<GenerateApimTokenService> logger,
            IUserDistributedCacheService cacheService,
            HttpClient httpClient) 
        {
            this.logger = logger;
            this.cacheService = cacheService;
            this.httpClient = httpClient;
        }

        public async Task<RepositoryResponse<string>> GenerateApimTokenAsync(ApimConfigurationBase apimConfiguration)
        {
            var token = await cacheService.ApimTokenGetAsync();
            if (token.IsNotNull())
            {
                return new RepositoryResponse<string>(content: token);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, apimConfiguration.TokenProviderUrl);
            
            var collection = new List<KeyValuePair<string, string>>
            {
                new("client_id", apimConfiguration.ClientId),
                new("client_secret", apimConfiguration.ClientSecret),
                new("grant_type", apimConfiguration.GrandType),
                new("scope", apimConfiguration.Scope)
            };
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var httpResponse = await httpClient.SendAsync(request);

            if (httpResponse.IsSuccessStatusCode)
            {
                logger.LogDebug("HttpResponse is success");
                var jsonString = await httpResponse.Content.ReadAsStringAsync();                

                JsonNode tokenNode = JsonNode.Parse(jsonString)!;
                int expireTime = tokenNode["expires_in"]!.GetValue<int>();
                token = tokenNode["access_token"]!.GetValue<string>();

                await cacheService.ApimTokenSetAsync(expireTime, token);

                return new RepositoryResponse<string>(content: token);
            }
            else
            {
                var response = new RepositoryResponse<string>();
                logger.LogError($"HTTP POST ERROR - StatusCode: {httpResponse.StatusCode}. ReasonPhrase: {httpResponse.ReasonPhrase}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, $"StatusCode: {httpResponse.StatusCode}. ReasonPhrase: {httpResponse.ReasonPhrase}");
                return response;
            }
        }
    }
}
