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
using Flurl;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Api.Service.Publish
{

    /// <summary>
    ///     Publish a Plan into a Oracle database
    /// </summary>
    public class PublishService : IPublishService
    {
        private readonly IOptions<PublishServiceConfiguration> _options;
        private readonly ILogger<PublishController> _logger;
        private readonly IEntityRepository<int> _versionEntityRepository;
        private readonly IBuilderRepository _builderRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly IAiepRepository _AiepRepository;
        private readonly IFileStorageService<AzureStorageConfiguration> _fileStorageService;
        private readonly HttpClient _httpClient;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="versionEntityRepository"></param>
        /// <param name="versionRepository"></param>
        /// <param name="planRepository"></param>
        /// <param name="actionRepository"></param>
        /// <param name="fileStorageService"></param>
        /// <param name="logger">Controller logger</param>
        /// <param name="builderRepository"></param>
        public PublishService(IOptions<PublishServiceConfiguration> options,
            ILogger<PublishController> logger,
            IEntityRepository<int> versionEntityRepository,
            IBuilderRepository builderRepository,
            IVersionRepository versionRepository,
            IAiepRepository AiepRepository,
            IFileStorageService<AzureStorageConfiguration> fileStorageService,
            HttpClient httpClient)
        {
            this._options = options;
            this._logger = logger;
            this._versionEntityRepository = versionEntityRepository;
            this._builderRepository = builderRepository;
            this._versionRepository = versionRepository;
            this._AiepRepository = AiepRepository;
            this._fileStorageService = fileStorageService;
            this._httpClient = httpClient;

            this._httpClient.BaseAddress = new Uri(this._options.Value.BaseUrl);
            this._httpClient.DefaultRequestHeaders.Add("basic-token", this._options.Value.Token);
        }

        #region Methods IPublishService

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<Stream> CreateLegacyZipPlanFromStream(Stream stream)
        {
            return null;
        }

        /// <summary>
        ///     Post a file into the Publish database
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fileStream"></param>
        public async Task<RepositoryResponse<string>> PublishPlanFileAsync(PublishVersionModel request, Stream fileStream)
        {
            var response = new RepositoryResponse<string>();
            Domain.Entity.Version version = null;
            RepositoryResponse<AiepModel> Aiep = null;
            if (request.VersionId != 0)
            {
                version = await _versionRepository.GetVersionWithPlanProjectAiep(request.VersionId);
            }

            if (version == null)
            {
                _logger.LogDebug($"PublishService: Version {request.VersionId} not found.");

                response.AddError(ErrorCode.EntityNotFound, $"Version {request.VersionId} not found");
                return response;
            }

            if (version.AiepCode != null)
            {
                Aiep = await _AiepRepository.GetAiepByCode(version.AiepCode);
            }

            PlanPublishType publishType;
            if (!request.RequiredVideo)
            {
                if (request.RequiredHd)
                {
                    publishType = PlanPublishType.ImageHighQuality;
                }
                else
                {
                    publishType = PlanPublishType.ImageStandardQuality;
                }
            }
            else
            {
                if (request.RequiredHd)
                {
                    publishType = PlanPublishType.ImageAndVideoHighQuality;
                }
                else
                {
                    publishType = PlanPublishType.ImageAndVideoStandardQuality;
                }
            }
            if (request.IsCycles)
            {
                publishType = PlanPublishType.CyclesImage;
            }

            var random = new Random();
            var subject = string.Empty;
            var builderId = string.Empty;

            if (version.Plan != null)
            {
                subject = "Plan: " + version.Plan.PlanCode;

                var builder = await _builderRepository.FindOneAsync<Builder>(version.Plan.BuilderId ?? -1);

                if (builder.IsNotNull())
                {
                    subject += " Builder Name: " + builder.TradingName;
                    builderId = builder.Id.ToString();
                }
            }

            var publishServiceModel = new PublishPlanRequest
            {
                AiepNo = version.AiepCode,
                BuilderId = builderId,
                QuoteNMBR = version.QuoteOrderNumber,
                Name = subject,
                HdVersion = request.RequiredHd ? '1' : '0',
                PlanType = (int)publishType,
                SenderEmail = request.EducationerEmail,
                RecipientEmail1 = request.ReceipientEmail1,
                RecipientEmail2 = request.ReceipientEmail2,
                Comments = request.Comments,
                AiepEmail = Aiep?.Content?.Email,
                UserEmail = request.UserEmail,
                VersionCode = request.VersionCode,
                Country = request.Country
            };

            _logger.LogDebug("Starting serialize object...");

            var publishServiceModelSerialised = JsonConvert.SerializeObject(publishServiceModel);

            var multipartFormDataContent = new MultipartFormDataContent($"--------Boundary{Guid.NewGuid()}");
            multipartFormDataContent.Headers.ContentType.MediaType = "multipart/form-data";
            var modelContent = new StringContent(publishServiceModelSerialised, Encoding.UTF8, "application/json");
            multipartFormDataContent.Add(modelContent, "model");
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            multipartFormDataContent.Add(fileContent, "file", "File");

            _logger.LogDebug("Starting the http POST");
            var httpResponse = await _httpClient.PostAsync(_options.Value.PublishJobUrl, multipartFormDataContent);

            if (httpResponse.IsSuccessStatusCode)
            {
                _logger.LogDebug("HttpResponse is success");
                return response;
            }
            else
            {
                _logger.LogError($"HTTP POST ERROR - StatusCode: { httpResponse.StatusCode}. ReasonPhrase: { httpResponse.ReasonPhrase}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, $"StatusCode: { httpResponse.StatusCode}. ReasonPhrase: { httpResponse.ReasonPhrase}");
                return response;
            }
        }

        /// <summary>
        ///     This function receives an ID, searches the path for this Version ID.
        ///     Once the path is returned it creates a ZIP file and Publishes it by inserting in
        ///     the Oracle database the Plan.
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        [Obsolete("Not used any more", false)]
        public async Task<RepositoryResponse<string>> PublishVersionAsync(PublishVersionModel plan)
        {
            // Return the path of the Master Version
            var path = (await _versionRepository.GetRomPathById(plan.VersionId)).Content;

            if (path.IsNull())
            {
                _logger.LogDebug($"Path not found for version ID {plan.VersionId}");
                var response = new RepositoryResponse<string>();
                response.AddError(ErrorCode.EntityNotFound, $"Path not found for version ID {plan.VersionId}");
                return response;
            }
            var fileStream = await _fileStorageService.DownloadAsync(path);

            return await PublishPlanFileAsync(plan, fileStream);
        }

        public async Task<RepositoryResponse<PublishJobModel>> GetPublishJobByJobIdAsync(Guid jobId)
        {
            this._logger.LogDebug("PublishService called GetPublishJobByJobIdAsync");

            var response = new RepositoryResponse<PublishJobModel>();
            var url = new Url(_options.Value.GetPublishJobsByJobIdUrl).AppendPathSegment(jobId);
            var httpResponse = await _httpClient.GetAsync(url);

            this._logger.LogDebug($"PublishService -> GetPublishJobByJobIdAsync - URL: {httpResponse.RequestMessage.RequestUri}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var stringModel = await httpResponse.Content.ReadAsStringAsync();
                var publishModel = JsonConvert.DeserializeObject<PublishJobModel>(stringModel);
                response.Content = publishModel;
            }
            else
            {
                _logger.LogError($"HTTP GET ERROR - StatusCode: { httpResponse.StatusCode}. ReasonPhrase: { httpResponse.ReasonPhrase}. JobId: {jobId}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, $"StatusCode: { httpResponse.StatusCode}. ReasonPhrase: { httpResponse.ReasonPhrase}");
            }
            this._logger.LogDebug("PublishService end call GetPublishJobByJobIdAsync");
            return response;
        }

        public async Task<RepositoryResponse<IEnumerable<PublishJobModel>>> GetPublishJobsByVersionCodesAsync(IEnumerable<string> versionCodes)
        {
            this._logger.LogDebug("PublishService called GetPublishJobsByVersionCodesAsync");

            var response = new RepositoryResponse<IEnumerable<PublishJobModel>>();

            var versionCodesSerialised = JsonConvert.SerializeObject(versionCodes);
            var url = new Url(_options.Value.GetPublishJobsByJobIdUrl).AppendPathSegment(versionCodesSerialised);
            var httpResponse = await _httpClient.GetAsync(url);

            this._logger.LogDebug($"PublishService -> GetPublishJobsByVersionCodesAsync - URL: {httpResponse.RequestMessage.RequestUri}");
            
            if (httpResponse.IsSuccessStatusCode)
            {
                var stringModel = await httpResponse.Content.ReadAsStringAsync();
                var publishModel = JsonConvert.DeserializeObject<IEnumerable<PublishJobModel>>(stringModel);
                response.Content = publishModel;
            }
            else
            {
                _logger.LogError($"HTTP GET ERROR - StatusCode: { httpResponse.StatusCode}. ReasonPhrase: { httpResponse.ReasonPhrase}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, $"StatusCode: { httpResponse.StatusCode}. ReasonPhrase: { httpResponse.ReasonPhrase}");
            }
            this._logger.LogDebug("PublishService end call GetPublishJobsByVersionCodesAsync");
            return response;
        }

        public async Task<RepositoryResponse<string>> HealthCheck()
        {
            this._logger.LogDebug("PublishService called HealthCheck");
            var response = new RepositoryResponse<string>();
            var url = new Url(_options.Value.HealthCheckUrl);
            var httpResponse = await _httpClient.GetAsync(url);

            this._logger.LogDebug($"PublishService -> HealthCheck - URL: {httpResponse.RequestMessage.RequestUri}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var httpResponseContent = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogError($"PublishService HealthCheck error - {httpResponseContent}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, httpResponseContent);
            }
            response.Content = httpResponse.StatusCode.ToString();
            this._logger.LogDebug("PublishService end call HealthCheck");
            return response;
        }

        #endregion

        #region Methods Public

        #endregion

        #region Methods Private


        #endregion

    }

}

