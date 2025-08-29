using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.Base;
using SpecialEducationPlanning
.Api.Service.ApimToken;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.Base
{
    public class ApimServiceBase : IApimServiceBase
    {
        private ApimConfigurationBase apimConfiguration;
        private ILogger<ApimServiceBase> logger;
        private readonly HttpClient httpClient;
        private readonly IGenerateApimTokenService apimTokenService;

        public ApimServiceBase(
            ApimConfigurationBase apimConfiguration,
            ILogger<ApimServiceBase> logger,
            HttpClient httpClient,
            IGenerateApimTokenService apimTokenService)
        {
            this.apimConfiguration = apimConfiguration;
            this.logger = logger;
            this.httpClient = httpClient;
            this.apimTokenService = apimTokenService;

            this.httpClient.BaseAddress = new Uri(this.apimConfiguration.BaseUrl);
        }

        public async Task<RepositoryResponse<T>> Post<T>(string requestUri, object content)
        {
            var response = new RepositoryResponse<T>();

            var json = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var token = await apimTokenService.GenerateApimTokenAsync(apimConfiguration);
            if (token.HasError())
            {
                response.ErrorList.AddRange(token.ErrorList);
                return response;
            }
            
            httpClient.DefaultRequestHeaders.Add("Authorization", token.Content);

            logger.LogDebug("Starting the http POST");
            var httpResponse = await httpClient.PostAsync(requestUri, json);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                try
                {
                    var data = JsonConvert.DeserializeObject<T>(responseContent);
                    response.Content = data;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Deserialization failed: {ex.Message}");
                    response.AddError(ErrorCode.DeserializationError, ex.Message);
                }

                return response;
            }
            else
            {
                logger.LogError($"HTTP POST ERROR - StatusCode: {httpResponse.StatusCode}. ReasonPhrase: {httpResponse.ReasonPhrase}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, $"StatusCode: {httpResponse.StatusCode}. ReasonPhrase: {httpResponse.ReasonPhrase}");
                return response;
            }
        }
    }
}
