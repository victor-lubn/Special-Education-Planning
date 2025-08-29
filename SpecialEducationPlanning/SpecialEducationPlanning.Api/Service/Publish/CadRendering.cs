using Koa.Persistence.EntityRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using sun.invoke.empty;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
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
.Business.Model.FileStorageModel;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using OracleConfiguration = SpecialEducationPlanning
.Api.Model.PublishServiceModel.OracleConfiguration;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Api.Service.Publish
{

    /// <summary>
    ///     Publish a Plan into a Oracle database
    /// </summary>
    public class CadRendering : IPublishService
    {

        private readonly IActionRepository actionRepository;

        private readonly OracleConfiguration configuration;

        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;
        private readonly IFileStorageService<AzureStorageTenderPackConfiguration> fileTenderPackService;
        private readonly ILogger<PublishController> logger;

        private readonly IPlanRepository planRepository;

        private readonly IEntityRepository<int> entityRepository;

        private readonly IVersionRepository versionRepository;

        private readonly IBuilderRepository builderRepository;

        private const string NO_MUSIC = "00";
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
        public CadRendering(IOptions<OracleConfiguration> options
            , IEntityRepository<int> entityRepository
            , IVersionRepository versionRepository
            , IPlanRepository planRepository
            , IActionRepository actionRepository
            , IFileStorageService<AzureStorageConfiguration> fileStorageService
            , IFileStorageService<AzureStorageTenderPackConfiguration> fileTenderPackService
            , ILogger<PublishController> logger
            , IBuilderRepository builderRepository)
        {
            this.configuration = options.Value;
            this.entityRepository = entityRepository;
            this.versionRepository = versionRepository;
            this.planRepository = planRepository;
            this.actionRepository = actionRepository;
            this.fileStorageService = fileStorageService;
            this.fileTenderPackService = fileTenderPackService;
            this.logger = logger;
            this.builderRepository = builderRepository;
        }

        #region Methods IPublishService

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<Stream> CreateLegacyZipPlanFromStream(Stream stream)
        {
            var outStream = new MemoryStream();

            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                var fileInArchive = archive.CreateEntry("PLANIT/Plan.rom", CompressionLevel.Fastest);

                using (var entryStream = fileInArchive.Open())
                using (var fileToCompressStream = new MemoryStream(stream.ToByteArray()))
                {
                    await fileToCompressStream.CopyToAsync(entryStream);
                }
            }

            return outStream;
        }

        /// <summary>
        ///     Post a file into the Publish database
        /// </summary>
        /// <param name="publishPlan"></param>
        /// <param name="stream"></param>
        public async Task<RepositoryResponse<string>> PublishPlanFileAsync(PublishVersionModel publishPlan, Stream stream)
        {
            logger.LogDebug("CadRendering called PublishPlanFileAsync");

            PlanPublicationType publicationType;
            char isHQ;

            Version version = publishPlan.Version;

            version.Plan = await planRepository.FindOneAsync<Plan>(version.PlanId);

            if (publishPlan.IsCHTPrequest)
            {
                (publicationType, isHQ) = (PlanPublicationType.StillsHQ, 'N');
            }
            else
            {
                (publicationType, isHQ) = DetermineNonChtpValues(publishPlan);
            }

            var random = new Random();
            var subject = string.Empty;

            if (version.Plan != null)
            {
                subject = "Plan: " + version.Plan.PlanCode;

                var builder =
                    await builderRepository.FindOneAsync<Builder>(version.Plan.BuilderId ?? -1);

                if (builder.IsNotNull())
                {
                    subject += " Builder Name: " + builder.TradingName;
                }
            }

            var request = new PublishPlanRequest
            {
                AiepNo = version.AiepCode,
                BuilderId = version.AiepCode.Substring(1) + "000000" + version.ExternalCode.Substring(3, 4),
                QuoteNMBR = version.AiepCode.Substring(1) + "0000" + version.ExternalCode.Substring(7, 4),
                Name = subject,
                HdVersion = isHQ,
                SoundTrack = publishPlan.SelectedMusic.IsNullOrEmpty() ? NO_MUSIC : publishPlan.SelectedMusic,
                PlanType = (int)publicationType,
                PublishDate = DateTime.UtcNow,
                PublishTime = DateTime.UtcNow.ToString("HHmmss", CultureInfo.InvariantCulture),
                SenderEmail = publishPlan.EducationerEmail,
                RecipientEmail1 = publishPlan.ReceipientEmail1,
                RecipientEmail2 = publishPlan.ReceipientEmail2,
                Id = Convert.ToInt32(DateTime.UtcNow.ToString("mmssfff")) * 100 + random.Next(0, 99),
                Comments = publishPlan.Comments,
                CrmProjectCode = publishPlan.CrmProjectCode,
                CrmHousingType = publishPlan.CrmHousingType,
                CrmHousingSpecificationCode = publishPlan.CrmHousingSpecificationCode,
                Destination = publishPlan.Destination
            };

            logger.LogDebug("PublishPlanRequest: {requestJson}", JsonConvert.SerializeObject(request));
            var connection = CreateOracleConnection();

            var query =
                "insert into CADFILE_QUOTES_TMP" +
                "       (ID, AiepNO, CUSTOMERID, QUOTENMBR, PLAN, NAME, HDVERSION, SOUNDTRACK, PLAN_TYPE, PUBLISH_DATE, PUBLISH_TIME, SENDER_EMAIL, RECIPIENT_EMAIL_1, RECIPIENT_EMAIL_2, COMMENTS, DESTINATION, CRMPROJECTCODE, CRMHOUSINGSPECIFICATIONCODE, CRMHOUSINGTYPE)" +
                " values (:Id, :AiepNo, : CustomerId, :QuoteNMBR, :Plan, :KeyName, :HdVersion, :Sountrack, : PlanType, :PublishDate, :PublishTime, :SenderEmail, :RecipientEmail1, :RecipientEmail2, :Comments, :Destination, :CrmProjectCode, :CrmHousingSpecificationCode, :CrmHousingType)";

            using (var cmd = new OracleCommand(query, connection))
            {
                cmd.Parameters.AddRange(CreateSqlParameters(request, stream));

                try
                {
                    logger.LogDebug("PublishService: {step}", "OpenAsync");
                    await connection.OpenAsync();
                    logger.LogDebug("PublishService: {step}", "OpenAsync finished");
                    logger.LogDebug("PublishService: {step}", "QueryAsync");
                    await cmd.ExecuteNonQueryAsync();
                    logger.LogDebug("PublishService: {step}", "QueryAsync finished");
                }
                catch (Exception ex)
                {
                    throw new CommunicationException("Publish Service: Not able to connect with Publish system. QueryAsync finished", ex);
                }
                finally
                {
                    logger.LogDebug("PublishService: {step}", "Close connection & Dispose");
                    connection.Close();
                    connection.Dispose();
                    logger.LogDebug("PublishService: {step}", "Close connection & Dispose Finsihed");
                }
            }

            logger.LogDebug("CadRendering end call PublishPlanFileAsync");
            return new RepositoryResponse<string>();
        }       

        /// <summary>
        ///     This function receives an ID, searches the path for this Version ID.
        ///     Once the path is returned it creates a ZIP file and Publishes it by inserting in
        ///     the Oracle database the Plan.
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<string>> PublishVersionAsync(PublishVersionModel plan)
        {
            logger.LogDebug("CadRendering called PublishVersionAsync");
            // Return the path of the Master Version
            var path = (await versionRepository.GetRomPathById(plan.VersionId)).Content;

            var version = (await versionRepository.GetRomAndPreviewInfo(plan.VersionId)).Content;

            if (path.IsNull())
            {
                logger.LogDebug("Path not found");

                return new RepositoryResponse<string>("Path not found");
            }

            var previewImage = await fileStorageService.DownloadAsync(version.PreviewPath); //download from DV Azure Storage
            if ((previewImage?.Length ?? 0) > 0 && !string.IsNullOrEmpty(plan.CrmHousingType))   //only CHTP plan
            {
                var pathPreviewImage = await fileTenderPackService.UploadCreatioAsync(previewImage, plan.CrmProjectCode, plan.PlanCode); //upload to Creatio Azure Storage
            }           

            var planZipStream = await CreateLegacyZipPlanFromStream(await fileStorageService.DownloadAsync(path));

            logger.LogDebug("CadRendering end call PublishVersionAsync");
            return await PublishPlanFileAsync(plan, planZipStream);
        }

        public async Task<RepositoryResponse<PublishJobModel>> GetPublishJobByJobIdAsync(Guid versionCode)
        {
            return new RepositoryResponse<PublishJobModel>(new PublishJobModel());
        }

        public async Task<RepositoryResponse<IEnumerable<PublishJobModel>>> GetPublishJobsByVersionCodesAsync(IEnumerable<string> versionCodes)
        {
            return new RepositoryResponse<IEnumerable<PublishJobModel>>(Enumerable.Empty<PublishJobModel>());
        }

        #endregion

        #region Methods Public

        public async Task<RepositoryResponse<string>> HealthCheck()
        {
            var connection = CreateOracleConnection();
            var response = new RepositoryResponse<string>();
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                logger.LogDebug($"{nameof(CadRendering)} HealthCheck -> Error: {ex.Message}");
                logger.LogDebug($"{nameof(CadRendering)} HealthCheck -> Error: {ex.InnerException}");
                response.AddError(ErrorCode.UnsuccessfulHttpCallError, ex.Message);
                return response;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return response;
        }

        #endregion

        #region Methods Private

        private OracleConnection CreateOracleConnection()
        {
            logger.LogDebug($"OracleConnection.ConnectionString {configuration.ConnectionString}");
            return new OracleConnection(configuration.ConnectionString);
        }

        private static OracleParameter[] CreateSqlParameters(PublishPlanRequest request, Stream stream)
        {
            return new[]
            {
                new OracleParameter("id", request.Id),
                new OracleParameter("AiepNo", request.AiepNo),
                new OracleParameter("CustomerId", request.BuilderId),
                new OracleParameter("QuoteNMBR", request.QuoteNMBR),
                new OracleParameter("Plan", stream.ToByteArray()),
                new OracleParameter("KeyName", request.Name),
                new OracleParameter("HdVersion", request.HdVersion),
                new OracleParameter("Sountrack", request.SoundTrack),
                new OracleParameter("PlanType", request.PlanType),
                new OracleParameter("PublishDate", new OracleDate(request.PublishDate.Year,request.PublishDate.Month,request.PublishDate.Day)),
                new OracleParameter("PublishTime", request.PublishTime),
                new OracleParameter("SenderEmail", request.SenderEmail),
                new OracleParameter("ReceipientEmail1", request.RecipientEmail1),
                new OracleParameter("ReceipientEmail2", request.RecipientEmail2),
                new OracleParameter("Comments", request.Comments),
                new OracleParameter("Destination", (int)request.Destination),
                new OracleParameter("CrmProjectCode", request.CrmProjectCode),
                new OracleParameter("CrmHousingSpecificationCode", request.CrmHousingSpecificationCode),
                new OracleParameter("CrmHousingType", request.CrmHousingType)
            };
        }

        private (PlanPublicationType publicationType, char isHQ) DetermineNonChtpValues(PublishVersionModel publishPlan)
        {
            var publicationType = PlanPublicationType.StillsHQ;
            var isHQ = 'N';

            if (!publishPlan.RequiredVideo)
            {
                if (publishPlan.RequiredHd)
                {
                    publicationType = PlanPublicationType.StillsHQ;
                }
                else
                {
                    publicationType = PlanPublicationType.StillsOnly;
                }
            }
            else
            {
                publicationType = PlanPublicationType.Video;

                if (publishPlan.RequiredVirtualShowRoom)
                {
                    isHQ = 'Y';
                }
            }
            if (publishPlan.IsCycles)
            {
                publicationType = PlanPublicationType.StillsCY;
            }
            return (publicationType, isHQ);
        }

        #endregion

    }

}

