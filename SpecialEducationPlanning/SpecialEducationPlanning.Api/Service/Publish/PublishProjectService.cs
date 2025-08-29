using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Koa.Persistence.EntityRepository;
using SpecialEducationPlanning
.Api.Configuration.PublishSystemService;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

using Newtonsoft.Json;
using System.Linq;
using com.sun.security.ntlm;
using SpecialEducationPlanning
.Api.Configuration.PublishProjectService;
using Microsoft.AspNetCore.Http;
using SpecialEducationPlanning
.Api.Service.ApimToken;
using SpecialEducationPlanning
.Api.Service.Base;

namespace SpecialEducationPlanning
.Api.Service.Publish
{
    /// <summary>
    ///     Publish a Project to APIM from where request goes to Creation app
    /// </summary>
    public class PublishProjectService : IPublishProjectService
    {
        private readonly PublishProjectServiceConfiguration config;
        private ILogger<PublishProjectService> logger;
        private readonly IProjectRepository projectRepo;
        private readonly HttpClient httpClient;
        private readonly IGenerateApimTokenService apimTokenService;

        public PublishProjectService(IOptions<PublishProjectServiceConfiguration> options,
            ILogger<PublishProjectService> logger,
            IProjectRepository projectRepo,
            HttpClient httpClient,
            IGenerateApimTokenService apimTokenService)
        {
            this.config = options.Value;
            this.logger = logger;
            this.projectRepo = projectRepo;
            this.httpClient = httpClient;
            this.apimTokenService = apimTokenService;

            //TODO: ask Tung
            //this.httpClient.BaseAddress = new Uri(this.config.BaseUrl);
            //this.httpClient.DefaultRequestHeaders.Add("ApiVersion", this.config.ApiVersion);
        }

        public async Task<RepositoryResponse<string>> SendRomItemsToCreatioAsync(int projectId)
        {
            var response = new RepositoryResponse<string>();

            var projectDB = await projectRepo.GetProjectRomItemsAsync(projectId);

            var payloadCreatio = CreatePayloadForCreatio(projectDB.Content);

            var token = await apimTokenService.GenerateApimTokenAsync(config);

            response = await SendRequestToAPIM(payloadCreatio, token.Content);

            return response;
        }       

        public async Task<RepositoryResponse<string>> SendRomItemsToCreatioAsync(int projectId, int planId)
        {
            var response = new RepositoryResponse<string>();

            var projectDB = await projectRepo.GetPlanRomItemsAsync(projectId, planId);

            var payloadCreatio = CreatePayloadForCreatio(projectDB.Content);

            var token = await apimTokenService.GenerateApimTokenAsync(config);

            response = await SendRequestToAPIM(payloadCreatio, token.Content);

            return response;
        }

        private async Task<RepositoryResponse<string>> SendRequestToAPIM(PublishProjectModel payloadCreatio, string token)
        {
            var response = new RepositoryResponse<string>();

            logger.LogDebug("Starting serialize object...");

            var payloadCreatioSerialised = JsonConvert.SerializeObject(payloadCreatio);

            var content = new StringContent(payloadCreatioSerialised, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("Authorization", token);  

            logger.LogDebug("Starting the http POST");
            var httpResponse = await httpClient.PostAsync("Create", content);

            if (httpResponse.IsSuccessStatusCode)
            {
                logger.LogDebug("HttpResponse is success");
                return response;
            }
            else
            {
                logger.LogError($"HTTP POST ERROR - StatusCode: {httpResponse.StatusCode}. ReasonPhrase: {httpResponse.ReasonPhrase}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, $"StatusCode: {httpResponse.StatusCode}. ReasonPhrase: {httpResponse.ReasonPhrase}");
                return response;
            }
        }

        private PublishProjectModel CreatePayloadForCreatio(Project content)
        {
            var housingSpecs = content.HousingSpecifications.Select(x =>
            {
                var houseTypes = x.HousingTypes;
                var item = new HousingSpecificatioTenderPackModel()
                {
                    CRMHousingSpecificationId = x.Code,
                    HousingTypes = houseTypes.Select(y =>
                    {
                        List<PlanTenderPackModel> houseTypePlans = new();
                        var plans = y.Plans;

                        foreach (Plan plan in plans)
                        {
                            var masterVersion = plan.Versions.First();

                            var houseTypePlan = new PlanTenderPackModel()
                            {
                                PlanCode = plan.PlanCode,
                                Title = plan.Title,
                                UpdatedDate = plan.UpdatedDate,
                                UpdatedUser = plan.UpdateUser,
                                MasterVersionId = plan.MasterVersionId,
                                PreviewPath = masterVersion.PreviewPath,

                                RomItems = masterVersion.RomItems.Select(item =>
                                {
                                    var romItem = new RomItemTenderPackModel()
                                    {
                                        Id = item.Id,
                                        ItemName = item.ItemName,
                                        PosNumber = item.PosNumber,
                                        Annotation = item.Annotation,
                                        Description = item.Description,
                                        Handing = item.Handing,
                                        Quantity = item.Qty,
                                        CatalogId = item.CatalogId,
                                        SerialNumber = item.SerialNumber,
                                        Sku = item.Sku,
                                        Range = item.Range,
                                        Color = item.Colour,
                                        OrderCode = item.OrderCode
                                    };
                                    return romItem;
                                }).ToList()
                            };
                            houseTypePlans.Add(houseTypePlan);
                        }

                        var houseTypeItem = new HousingTypeTenderPackModel()
                        {
                            CRMHousingTypeId = y.Code,
                            Plans = houseTypePlans 
                        };
                        return houseTypeItem;
                    }).ToList()
                };
                return item;
            }).ToList();

            PublishProjectModel payload = new()
            {
                CRMProjectId = content.KeyName,
                HousingSpecifications = housingSpecs
            };

            return payload;
        }

        
    } 
}
