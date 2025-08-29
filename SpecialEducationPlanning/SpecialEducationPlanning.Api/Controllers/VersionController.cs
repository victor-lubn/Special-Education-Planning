using Koa.Hosting.AspNetCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.DtoModel.VersionDto;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Service.FittersPack;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Extensions;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Mapper;
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


namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Version controller
    /// </summary>
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        private readonly IVersionRepository versionRepository;
        private readonly IActionRepository actionRepository;
        private readonly IPlanRepository planRepository;
        private readonly IRomItemRepository romItemRepository;
        private readonly ICatalogRepository catalogRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;
        private readonly IFileStorageService<AzureStorageFittersPackConfiguration> fittersPackStorageService;
        private readonly ILogger logger;
        private readonly IUserRepository userRepository;
        private readonly IUserService userService;
        private readonly IObjectMapper mapper;
        private readonly IFittersPackService _fittersPackService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="versionRepository"></param>
        /// <param name="actionRepository"></param>
        /// <param name="planRepository"></param>
        /// <param name="romItemRepository"></param>
        /// <param name="catalogRepository"></param>
        /// <param name="commentRepository"></param>
        /// <param name="fileStorageService"></param>
        /// <param name="logger"></param>
        /// <param name="userRepository"></param>
        /// <param name="userService"></param>
        public VersionController(
                IVersionRepository versionRepository,
                IActionRepository actionRepository,
                IPlanRepository planRepository,
                IRomItemRepository romItemRepository,
                ICatalogRepository catalogRepository,
                ICommentRepository commentRepository,
                IFileStorageService<AzureStorageConfiguration> fileStorageService,
                IFileStorageService<AzureStorageFittersPackConfiguration> fittersPackStorageService,
                ILogger<VersionController> logger,
                IUserRepository userRepository, IUserService userService,
                IObjectMapper mapper,
                IFittersPackService fittersPackService
            )
        {
            this.versionRepository = versionRepository;
            this.actionRepository = actionRepository;
            this.planRepository = planRepository;
            this.romItemRepository = romItemRepository;
            this.catalogRepository = catalogRepository;
            this.commentRepository = commentRepository;
            this.fileStorageService = fileStorageService;
            this.userRepository = userRepository;
            this.userService = userService;
            this.logger = logger;
            this.mapper = mapper;
            _fittersPackService = fittersPackService;
            this.fittersPackStorageService = fittersPackStorageService;
        }

        /// <summary>
        ///     Delete a version by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]

        [AuthorizeTdpFilter(PermissionType.Plan_Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("VersionController called Delete");

            //TODO Refactor specification
            var entity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var model = mapper.Map<Domain.Entity.Version, VersionModel>(entity);

            if (model == null)
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call Delete -> return Not found");

                return NotFound();
            }
            versionRepository.Remove(id);

            logger.LogDebug("VersionController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get Educationer by id
        /// </summary>
        /// <param name="id">Educationer id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var model = mapper.Map<Domain.Entity.Version, VersionModel>(entity);
            var repositoryResponse = new RepositoryResponse<VersionModel>
            {
                Content = model
            };
            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("Educationer not found");
                return NotFound();
            }
            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all
        /// </summary>
        /// <returns>Educationers list</returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("VersionController called GetAll");
            var list = await versionRepository.GetAllAsync<Domain.Entity.Version>();
            var models = mapper.Map<IEnumerable<Domain.Entity.Version>, IEnumerable<VersionModel>>(list);

            var repositoryResponse = new RepositoryResponse<IEnumerable<VersionModel>>
            {
                Content = models
            };

            logger.LogDebug("VersionController end call GetAll -> return All versions");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates a version
        /// </summary>
        /// <param name="value">Version model</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post([FromBody] VersionModel value)
        {
            logger.LogDebug("VersionController called Post -> Create version");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));
                logger.LogDebug("VersionController end call Post -> return Bad request");
                return BadRequest(ModelState.GetErrorMessages());
            }

            var createdVersion = await versionRepository.CreateVersionAsync(value);

            foreach (var romItem in value.RomItems)
            {
                romItem.VersionId = createdVersion.Id;
                var romItemEntity = mapper.Map<RomItemModel, RomItem>(romItem);
                await romItemRepository.Add(romItemEntity);
            }

            var planEntity = await planRepository.GetWithNavigationsAsync<Plan>(value.PlanId, new List<string> { nameof(EducationToolOrigin) });

            SetMasterVersion(planEntity, createdVersion);

            planEntity.UpdatedDate = createdVersion.UpdatedDate;
            await planRepository.ApplyChangesAsync(planEntity);

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");
                logger.LogDebug("VersionController end call Post -> return Bad request Undefined user");
                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            await actionRepository.CreateAction<VersionModel>(ActionType.Create, createdVersion.VersionNumber.ToString(), createdVersion.Id, userFullName);

            await GenerateFitterPackAsync(planEntity, createdVersion);

            var versionEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(createdVersion.Id);
            var response = mapper.Map<Domain.Entity.Version, VersionModel>(versionEntity);

            logger.LogDebug("VersionController end call Post -> return Created version");

            return new OkObjectResult(response);
        }

        private async Task GenerateFitterPackAsync(Plan planEntity, VersionModel createdVersion)
        {
            if (planEntity.EducationToolOrigin?.Name.Is3Dc() == true && createdVersion.EducationTool3DCVersionId > 0)
            {
                await _fittersPackService.GenerateFitterPackAsync(createdVersion.Id,
                    createdVersion.EducationTool3DCVersionId.Value);
            }
        }

        private void SetMasterVersion(Plan planEntity, VersionModel createdVersion)
        {
            planEntity.MasterVersionId =
                planEntity.EducationToolOrigin?.Name.Is3Dc() == true  && planEntity.MasterVersionId > 0
                    ? planEntity.MasterVersionId
                    : createdVersion.Id;
        }

        /// <summary>
        ///     Updates a Educationer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> Put(int id, [FromBody] VersionModel value)
        {
            logger.LogDebug("VersionController called Put -> Update version");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("VersionController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var planEntity = await planRepository.FindOneAsync<Plan>(value.PlanId);

            if (planEntity.IsNull())
            {
                logger.LogDebug("Plan not found");

                logger.LogDebug("VersionController end call Put -> return Plan not found");

                return NotFound();
            }

            var originalVersionEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            
            if (originalVersionEntity.IsNull())
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call Put -> return Version not found");

                return NotFound();
            }

            originalVersionEntity.QuoteOrderNumber = value.QuoteOrderNumber;

            originalVersionEntity.EducationTool3DCVersionId = value.EducationTool3DCVersionId;
            originalVersionEntity.EducationTool3DCPlanId = value.EducationTool3DCPlanId;
            originalVersionEntity.CatalogId = value.CatalogId;
            originalVersionEntity.VersionNumber = value.VersionNumber;
            
            originalVersionEntity.LastKnown3DCVersion = value.LastKnown3DCVersion;
            originalVersionEntity.LastKnownCatalogId = value.LastKnownCatalogId;
            originalVersionEntity.LastKnownPreviewPath = value.LastKnownPreviewPath;
            originalVersionEntity.LastKnownRomPath = value.LastKnownRomPath;

            if (!string.IsNullOrEmpty(value.PreviewPath))
            {
                originalVersionEntity.PreviewPath = value.PreviewPath;
            }

            if (!string.IsNullOrEmpty(value.RomPath))
            {
                originalVersionEntity.RomPath = value.RomPath;
            }
            
            await versionRepository.ApplyChangesAsync(originalVersionEntity);
            
            foreach (var romItem in value.RomItems)
            {
                RomItem item = await romItemRepository.FindOneAsync<RomItem>(romItem.Id);

                if (item.IsNull())
                {
                    romItem.VersionId = id;
                    item = mapper.Map<RomItemModel, RomItem>(romItem);
                    await this.romItemRepository.Add(item);
                }
                else
                {
                    item.VersionId = id;
                    item = mapper.Map<RomItemModel, RomItem>(romItem, item);
                    await romItemRepository.ApplyChangesAsync(item);
                }
            }

            originalVersionEntity.Id = id;
            var repositoryResponse = await versionRepository.UpdateVersionAsync(originalVersionEntity);
            if (repositoryResponse.Content == null)
            {
                logger.LogDebug("VersionController end call Put -> return Version not found");

                return NotFound();
            }

            planEntity.UpdatedDate = repositoryResponse.Content.UpdatedDate;
            await planRepository.ApplyChangesAsync(planEntity);

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("VersionController end call Put -> return Bad request undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            await actionRepository.CreateAction<VersionModel>(ActionType.Update, repositoryResponse.Content.VersionNumber.ToString(), repositoryResponse.Content.Id, userFullName);
            
            await GenerateFitterPackAsync(planEntity, repositoryResponse);

            var versionEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(repositoryResponse.Content.Id);
            var response = mapper.Map<Domain.Entity.Version, VersionModel>(versionEntity);

            logger.LogDebug("VersionController end call Put -> return Version");

            return new OkObjectResult(response);
        }

        private async Task GenerateFitterPackAsync(Plan planEntity, RepositoryResponse<VersionModel> repositoryResponse)
        {
            if (planEntity.EducationToolOrigin?.Name.Is3Dc() == true && repositoryResponse.Content.EducationTool3DCVersionId > 0 &&
                repositoryResponse.Content.LastKnown3DCVersion == repositoryResponse.Content.EducationTool3DCVersionId)
            {
                await _fittersPackService.GenerateFitterPackAsync(repositoryResponse.Content.Id,
                    repositoryResponse.Content.EducationTool3DCVersionId.Value);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{planId}/SaveVersion")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]
        public async Task<IActionResult> SaveVersion(int planId, [FromBody] MultiUploadedFileModel<VersionInfoModel> value)
        {
            var a = this.HttpContext.Request;
            var dict = a.Form.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
            var m = dict["model"];
            var buffer = Encoding.UTF8.GetBytes(m);
            using (var stream = new MemoryStream(buffer))
            {
                value.Model = await JsonSerializer.DeserializeAsync<VersionInfoModel>(stream, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }

            logger.LogDebug("VersionController called SaveVersion");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("VersionController end call SaveVersion -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            logger.LogDebug("Downloading rom");
            var romFileStream = Request.Form.Files.FirstOrDefault(x => x.ContentType.ToLower().Equals("application/octet-stream"));
            logger.LogDebug("Downloading preview");
            var previewFileStream = Request.Form.Files.FirstOrDefault(x => x.ContentType.ToLower().Equals("image/jpeg"));

            if (romFileStream == null)
            {
                logger.LogError("No files on the Request");

                logger.LogDebug("VersionController end call SaveVersion -> return Bad request Entity not found");

                return BadRequest(ErrorCode.EntityNotFound.GetDescription());
            }

            var versionModel = await versionRepository.SaveVersion(planId, value.Model.Id, value.Model);
            if (versionModel.Content == null || versionModel.ErrorList.Any())
            {
                logger.LogError("Error creating the version");

                logger.LogDebug("VersionController end call SaveVersion -> return Error version");

                return versionModel.GetHttpResponse();
            }

            var planEntity = await planRepository.FindOneAsync<Plan>(versionModel.Content.PlanId);
            var planModel = mapper.Map<Plan, PlanModel>(planEntity);
            versionModel.Content.Plan = planModel;

            if (!value.Model.IsSync)
            {
                await planRepository.ApplyChangesPlanAsync(planModel);
            }

            //Uploads romFile
            var romUploaded = await versionRepository.UploadRomFile(romFileStream.OpenReadStream(), romFileStream.FileName, versionModel.Content);
            if (romUploaded.ErrorList.Any())
            {
                logger.LogError("Error creating the version");

                logger.LogDebug("VersionController end call SaveVersion -> return Errors rom upload");

                return romUploaded.GetHttpResponse();
            }
            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("VersionController end call SaveVersion -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }
            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);
            if (value.Model.Id == 0)
            {
                await actionRepository.CreateAction<VersionModel>(ActionType.FileCreate, string.Empty, versionModel.Content.Id, userFullName);
            }
            else
            {
                await actionRepository.CreateAction<PlanModel>(ActionType.Update, string.Empty, planId, userFullName);
            }

            if (previewFileStream != null)
            {
               await versionRepository.UploadPreviewFile(previewFileStream.OpenReadStream(), previewFileStream.FileName, versionModel.Content.Id);
            }

            logger.LogDebug("finish");

            logger.LogDebug("VersionController end call SaveVersion -> return Version saved");

            return versionModel.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Rom")]
        public async Task<IActionResult> GetRom(int id)
        {
            logger.LogDebug("VersionController called GetRom");

            var getRomEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var getRomModel = mapper.Map<Domain.Entity.Version, VersionModel>(getRomEntity);
            if (getRomModel.IsNull())
            {
                logger.LogDebug("Rom not found");

                logger.LogDebug("VersionController end call GetRom -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await versionRepository.GetRomAndPreviewInfo(id);
            if (repositoryResponse.ErrorList.Any())
            {
                logger.LogDebug("VersionController end call GetRom -> return Errors");

                return repositoryResponse.GetHttpResponse();
            }

            var fileStream = await fileStorageService.DownloadAsync(repositoryResponse.Content.RomPath);

            if (fileStream.IsNull())
            {
                logger.LogDebug("VersionController end call GetRom -> return No content");

                return NoContent();
            }

            logger.LogDebug("VersionController end call GetRom -> return Rom");

            return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = repositoryResponse.Content?.Rom == null ? "plan.Rom" : repositoryResponse.Content.Rom };
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/FittersPackPdf")]
        public async Task<IActionResult> GetFittersPackPdf(int id)
        {
            logger.LogDebug("VersionController called GetFittersPackPdf");

            var getFittersPackEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var getFittersPackModel = mapper.Map<Domain.Entity.Version, VersionModel>(getFittersPackEntity);
            if (getFittersPackModel.IsNull())
            {
                logger.LogDebug("FittersPack not found");
                logger.LogDebug("VersionController end call GetFittersPackPdf -> return Not found");
                return NotFound();
            }

            var fileStream = await fittersPackStorageService.DownloadAsync(getFittersPackModel.FittersPackPath);

            if (fileStream.IsNull())
            {
                logger.LogDebug("VersionController end call GetFittersPackPdf -> return No content");
                return NoContent();
            }

            logger.LogDebug("VersionController end call GetFittersPackPdf -> return FittersPackPdf");
            return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = $"{getFittersPackModel.ExternalCode}_fitters_pack.pdf" };
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{id}/Rom")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> PostRom(int id, IFormFile file)
        {
            logger.LogDebug("VersionController called PostRom");

            var postRomEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var postRomModel = mapper.Map<Domain.Entity.Version, VersionModel>(postRomEntity);
            if (postRomModel.IsNull())
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call PostRom -> return Not found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("VersionController end call PostRom -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            var repositoryResponse = await versionRepository.GetRomAndPreviewInfo(id);
            if (!repositoryResponse.ErrorList.Any() && repositoryResponse.Content.Rom.IsNull())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var path = await fileStorageService.UploadAsync<Domain.Entity.Version>(fileStream);
                    await actionRepository.CreateAction<VersionModel>(ActionType.FileCreate, string.Empty, id, userFullName);

                    logger.LogDebug("VersionController end call PostRom -> return SetVersionRom call");

                    return (await versionRepository.SetVersionRom(id, path, file.FileName, string.Empty)).GetHttpResponse();
                }
            }

            logger.LogDebug("VersionController end call PostRom -> return Version");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPut("{id}/Rom")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> PutRom(int id, IFormFile file)
        {
            logger.LogDebug("VersionController called PutRom");

            var putRomEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var putRomModel = mapper.Map<Domain.Entity.Version, VersionModel>(putRomEntity);
            if (putRomModel.IsNull())
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call PutRom -> return Not found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("VersionController end call PutRom -> return Bad reques Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            var repositoryResponse = await versionRepository.GetRomAndPreviewInfo(id);
            if (!repositoryResponse.ErrorList.Any() && repositoryResponse.Content.Rom.IsNotNull())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var path = await fileStorageService.UploadAsync<Domain.Entity.Version>(fileStream);
                    await fileStorageService.DeleteAsync(repositoryResponse.Content.RomPath);
                    await actionRepository.CreateAction<VersionModel>(ActionType.FileUpdate, string.Empty, id, userFullName);

                    logger.LogDebug("VersionController end call PutRom -> return SetVersionRom call");

                    return (await versionRepository.SetVersionRom(id, path, file.FileName, string.Empty)).GetHttpResponse();
                }
            }

            logger.LogDebug("VersionController end call PutRom -> return Version");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get a ROM preview for a specific version
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Preview")]

        public async Task<IActionResult> GetPreview(int id)
        {
            logger.LogDebug("VersionController called GetPreview");

            var getPreviewEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var getPreviewModel = mapper.Map<Domain.Entity.Version, VersionModel>(getPreviewEntity);

            if (getPreviewModel.IsNull())
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call GetPreview -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await versionRepository.GetRomAndPreviewInfo(id);
            if (repositoryResponse.ErrorList.Any()) { return repositoryResponse.GetHttpResponse(); }
            var fileStream = await fileStorageService.DownloadAsync(repositoryResponse.Content.PreviewPath);
            if (fileStream.IsNull())
            {
                logger.LogDebug("VersionController end call GetPreview -> return No content");

                return NoContent();
            }

            logger.LogDebug("VersionController end call GetPreview -> return Document");

            return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = repositoryResponse.Content.Preview };
        }

        /// <summary>
        ///     Upload a ROM file to file storage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{id}/Preview")]

        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> PostPreview(int id, IFormFile file)
        {
            logger.LogDebug("VersionController called PostPreview");


            var postPreviewEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var postPreviewModel = mapper.Map<Domain.Entity.Version, VersionModel>(postPreviewEntity);

            if (postPreviewModel.IsNull())
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call PostPreview -> return Not found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("VersionController end call PostPreview -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            var repositoryResponse = await versionRepository.GetRomAndPreviewInfo(id);
            if (!repositoryResponse.ErrorList.Any() && repositoryResponse.Content.Preview.IsNull())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var path = await fileStorageService.UploadAsync<Domain.Entity.Version>(fileStream);
                    await actionRepository.CreateAction<VersionModel>(ActionType.FileCreate, string.Empty, id, userFullName);

                    logger.LogDebug("VersionController end call PostPreview -> return SetVersionPreview call");

                    return (await versionRepository.SetVersionPreview(id, path, file.FileName)).GetHttpResponse();
                }
            }

            logger.LogDebug("VersionController end call PostPreview -> return Version");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPut("{id}/Preview")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> PutPreview(int id, IFormFile file)
        {
            logger.LogDebug("VersionController called PutPreview");


            var putPreviewEntity = await versionRepository.FindOneAsync<Domain.Entity.Version>(id);
            var putPreviewModel = mapper.Map<Domain.Entity.Version, VersionModel>(putPreviewEntity);

            if (putPreviewModel.IsNull())
            {
                logger.LogDebug("Version not found");

                logger.LogDebug("VersionController end call PutPreview -> return Not found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("VersionController end call PutPreview -> return Bad reques Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            var repositoryResponse = await versionRepository.GetRomAndPreviewInfo(id);
            if (!repositoryResponse.ErrorList.Any() && repositoryResponse.Content.Preview.IsNotNull())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var path = await fileStorageService.UploadAsync<Domain.Entity.Version>(fileStream);
                    await fileStorageService.DeleteAsync(repositoryResponse.Content.PreviewPath);
                    await actionRepository.CreateAction<VersionModel>(ActionType.FileUpdate, string.Empty, id, userFullName);

                    logger.LogDebug("VersionController end call PutPreview -> return SetVersionPreview call");

                    return (await versionRepository.SetVersionPreview(id, path, file.FileName)).GetHttpResponse();
                }
            }

            logger.LogDebug("VersionController end call PutPreview -> return Version");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Comments")]

        [AuthorizeTdpFilter(PermissionType.Plan_Comment)]
        public async Task<IActionResult> GetComments(int id)
        {
            logger.LogDebug("VersionController called GetComments");

            var repositoryResponse = await commentRepository.GetModelComments<VersionModel>(id);

            logger.LogDebug("VersionController end call GetComments -> return List of comments");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/Comments")]

        [AuthorizeTdpFilter(PermissionType.Plan_Comment)]
        public async Task<IActionResult> PostCommentPlan(int id, [FromBody] CommentModel comment)
        {
            logger.LogDebug("VersionController called PostCommentPlan");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("VersionController end call PostCommentPlan -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            logger.LogDebug("VersionController end call PostCommentPlan -> return Ok call Create comment");

            return Ok((await commentRepository.CreateComment<VersionModel>(comment, id, userFullName)).Content);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Actions")]
        public async Task<IActionResult> GetActions(int id)
        {
            logger.LogDebug("VersionController called GetActions");

            var repositoryResponse = await actionRepository.GetModelActions<VersionModel>(id);

            logger.LogDebug("VersionController end call GetActions -> return Actions");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Catalogs")]
        public async Task<IActionResult> GetCatalogs(int id)
        {
            logger.LogDebug("VersionController called GetCatalogs");

            var repositoryResponse = await versionRepository.GetAvailableCatalogsAsync(id);

            logger.LogDebug("VersionController end call GetCatalogs -> return Int");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="versionNotes"></param>
        /// <returns></returns>
        [HttpPut("{id}/ModifyVersionNotes")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        [Obsolete("This endpoint is obsolete. Use ModifyVersionNotesAndQuote instead.", false)]
        public async Task<IActionResult> ModifyVersionNotes(int id, [FromBody] string versionNotes)
        {
            logger.LogDebug("VersionController called ModifyVersionNotes");

            var response = await versionRepository.ModifyVersionNotes(id, versionNotes);

            logger.LogDebug("VersionController end call ModifyVersionNotes -> return Version");

            return response.GetHttpResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="versionNotesQuote"></param>
        /// <param name="versionNotes"></param>
        /// <param name="quoteOrderNumber"></param>
        /// <returns></returns>
        [HttpPut("{id}/ModifyVersionNotesAndQuote")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> ModifyVersionNotesAndQuote(int id, [FromBody] VersionNotesQuoteDto versionNotesQuote)
        {
            logger.LogDebug("VersionController called ModifyVersionNotesAndQuote");

            var response = await versionRepository.ModifyVersionNotesAndQuote(id, versionNotesQuote.VersionNotes, versionNotesQuote.QuoteOrderNumber);

            logger.LogDebug("VersionController end call ModifyVersionNotesAndQuote -> return Version");

            return response.GetHttpResponse();
        }
    }
}
