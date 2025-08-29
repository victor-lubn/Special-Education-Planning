using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Koa.Hosting.AspNetCore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Service.User;
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
.Business.Repository.UserReleaseInfoRepository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.EndUserSpecifications;

namespace SpecialEducationPlanning
.Api.Controllers
{

    /// <summary>
    ///     ReleaseInfo Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ReleaseInfoController : Controller
    {

        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;

        private readonly IUserService userService;

        private readonly IReleaseInfoRepository repository;

        private readonly IUserReleaseInfoRepository userReleaseInfoRepository;

        private readonly ILogger<ReleaseInfoController> logger;

        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="fileStorageService"></param>
        public ReleaseInfoController(IReleaseInfoRepository repository, IFileStorageService<AzureStorageConfiguration> fileStorageService, IUserService userService, IUserReleaseInfoRepository userReleaseInfoRepository, ILogger<ReleaseInfoController> logger, IObjectMapper mapper)
        {
            this.repository = repository;
            this.fileStorageService = fileStorageService;
            this.userService = userService;
            this.userReleaseInfoRepository = userReleaseInfoRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Delete a release
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("ReleaseInfoController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<ReleaseInfo>(id);
            if (entity == null)
            {
                logger.LogDebug("Not found");

                logger.LogDebug("ReleaseInfoController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("ReleaseInfoController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get a release by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("ReleaseInfoController called Get");

            var repositoryResponse = new RepositoryResponse<ReleaseInfoModel>();
            var entity = await repository.FindOneAsync<ReleaseInfo>(id);
            var mdoel = mapper.Map<ReleaseInfo, ReleaseInfoModel>(entity);
            repositoryResponse.Content = mdoel;

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("Not found");

                logger.LogDebug("ReleaseInfoController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("ReleaseInfoController end call Get -> return Release info");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all releases
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("ReleaseInfoController called GetAll");
            var entityList = await repository.GetAllAsync<ReleaseInfo>();   
            var modelList = mapper.Map<IEnumerable<ReleaseInfo>, IEnumerable<ReleaseInfoModel>>(entityList);
            var repositoryResponse = new RepositoryResponse<IEnumerable<ReleaseInfoModel>>
            {
                Content = modelList
            };

            logger.LogDebug("ReleaseInfoController end call GetAll -> return All Release info");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get a release info document by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Document")]
        public async Task<IActionResult> GetReleaseInfoDocument(int id)
        {
            logger.LogDebug("ReleaseInfoController called GetReleaseInfoDocument");

            var releaseNotesEntity = await repository.FindOneAsync<ReleaseInfo>(id);
            var releaseNotes = mapper.Map<ReleaseInfo, ReleaseInfoModel>(releaseNotesEntity);

            if (releaseNotes.IsNull())
            {
                logger.LogDebug("No document found");

                logger.LogDebug("ReleaseInfoController end call GetReleaseInfoDocument -> return Not found");

                return NotFound();
            }

            var fileStream = await fileStorageService.DownloadAsync(releaseNotes.DocumentPath);

            if (fileStream.IsNull())
            {
                logger.LogDebug("Document has no content");

                logger.LogDebug("ReleaseInfoController end call GetReleaseInfoDocument -> return No content");

                return NoContent();
            }

            logger.LogDebug("ReleaseInfoController end call GetReleaseInfoDocument -> return Document");

            return new FileStreamResult(fileStream, "application/octet-stream")
            { FileDownloadName = releaseNotes.Document };
        }

        /// <summary>
        ///     Get release inrfo by version
        /// </summary>
        /// <param name="version"></param>
        /// <param name="fusionVersion"></param>
        /// <param name="onDemand"></param>
        /// <returns></returns>
        [HttpGet("VersionsDocument")]
        public async Task<IActionResult> GetReleaseInfoByVersion(string version, string fusionVersion, bool onDemand)
        {
            logger.LogDebug("ReleaseInfoController called GetReleaseInfoByVersion");

            var userId = this.userService.GetUserId((ClaimsIdentity)this.User.Identity);

            var releaseInfo = await repository.GetReleaseInfoAsync(version, fusionVersion);

            if (releaseInfo.Content.IsNull() || releaseInfo.ErrorList.Any())
            {
                logger.LogDebug("No release not read");

                logger.LogDebug("ReleaseInfoController end call GetReleaseInfoByVersion -> return Ok No release");

                return Ok();
            }

            var existsUserReleaseInfo = await userReleaseInfoRepository.ExistsUserReleaseInfo(releaseInfo.Content.Id, userId);

            Stream fileStream = new MemoryStream();
            if (!existsUserReleaseInfo)
            {
                fileStream = await fileStorageService.DownloadAsync(releaseInfo.Content.DocumentPath);

                if (fileStream.IsNull())
                {
                    logger.LogDebug("Document has no content");

                    logger.LogDebug("ReleaseInfoController end call GetReleaseInfoByVersion -> return No content");

                    return NoContent();
                }

                if (!onDemand)
                {
                    var userReleaseInfo = await userReleaseInfoRepository.CreateUserReleaseInfoAsync(releaseInfo.Content.Id, userId);

                    if (userReleaseInfo.Content.IsNull() || userReleaseInfo.ErrorList.Any())
                    {
                        logger.LogDebug("No user release info created");

                        logger.LogDebug("ReleaseInfoController end call GetReleaseInfoByVersion -> return Ok No user release info created");

                        return Ok();
                    }
                }
            }
            else
            {
                if (onDemand)
                {
                    fileStream = await fileStorageService.DownloadAsync(releaseInfo.Content.DocumentPath);

                    if (fileStream.IsNull())
                    {
                        logger.LogDebug("Document has no content");

                        logger.LogDebug("ReleaseInfoController end call GetReleaseInfoByVersion -> return No content");

                        return NoContent();
                    }
                }
            }

            logger.LogDebug("ReleaseInfoController end call GetReleaseInfoByVersion -> return Document");

            return new FileStreamResult(fileStream, "application/octet-stream") { FileDownloadName = releaseInfo.Content.Document };
        }

        /// <summary>
        ///     Delete release info by version
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteUserReleaseInfoAsync")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> DeleteUserReleaseInfoAsync(string version, string fusionVersion)
        {
            logger.LogDebug("ReleaseInfoController called DeleteUserReleaseInfoAsync");

            var response = await userReleaseInfoRepository.DeleteUserReleaseInfoAsync(version, fusionVersion);

            if (response)
            {
                logger.LogDebug("ReleaseInfoController end call DeleteUserReleaseInfoAsync -> return Ok");

                return Ok();
            }

            logger.LogDebug("No user release info deleted");

            logger.LogDebug("ReleaseInfoController end call DeleteUserReleaseInfoAsync -> return Bad request");

            return BadRequest();
        }

        /// <summary>
        ///     Create a new release
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> Post([FromBody] ReleaseInfoModel value)
        {
            logger.LogDebug("ReleaseInfoController called Post -> Create release info");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("ReleaseInfoController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = new RepositoryResponse<ReleaseInfoModel>();
            try
            {
                var releaseNotes =  await repository.FindOneAsync<ReleaseInfo>(value.Id);
                if (releaseNotes.IsNull())
                {
                    releaseNotes = mapper.Map(value, releaseNotes);
                    await repository.Add(releaseNotes);
                }
                else
                {
                    releaseNotes = mapper.Map(value, releaseNotes);
                    await repository.ApplyChangesAsync(releaseNotes);
                }

                var releaseNotesModel = mapper.Map<ReleaseInfo,ReleaseInfoModel>(releaseNotes);
                repositoryResponse.Content = releaseNotesModel;
            }
            catch (Exception)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityAlreadyExist.GetDescription());
            }

            logger.LogDebug("ReleaseInfoController end call Post -> return Release info");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Upload file to file storage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{id}/Document")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> PostReleaseInfoDocument(int id, [FromBody] MultiUploadedFileModel fileUpload)
        {
            logger.LogDebug("ReleaseInfoController called PostReleaseInfoDocument");

            var infoDocEntity = await repository.FindOneAsync<ReleaseInfo>(id);
            var infoDoc = mapper.Map<ReleaseInfo, ReleaseInfoModel>(infoDocEntity);
            if (infoDoc.IsNull())
            {
                logger.LogDebug("No document found");

                logger.LogDebug("ReleaseInfoController end call PostReleaseInfoDocument -> return Not found");

                return NotFound();
            }

            //var file = Request.Form.Files.FirstOrDefault();
            var file = fileUpload.Files.FirstOrDefault();
            using (var stream = file.FileStream)
            {
                var path = await fileStorageService.UploadAsync<ReleaseInfo>(stream);

                logger.LogDebug("ReleaseInfoController end call PostReleaseInfoDocument -> return Release info document");

                return (await repository.SetReleaseInfoDocument(id, path, file.OriginalName)).GetHttpResponse();
            }
        }

        /// <summary>
        ///     Update release info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] ReleaseInfoModel value)
        {
            logger.LogDebug("ReleaseInfoController called Put");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("ReleaseInfoController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            ReleaseInfo releaseInfo = await repository.FindOneAsync<ReleaseInfo>(id);

            if (releaseInfo.IsNull())
            {
                logger.LogDebug("Not found");

                logger.LogDebug("ReleaseInfoController end call Put -> return Not found");

                return NotFound();
            }

            var entity = mapper.Map<ReleaseInfoModel, ReleaseInfo>(value, releaseInfo);
            var entityApply = await repository.ApplyChangesAsync(entity);
            var model = mapper.Map<ReleaseInfo, ReleaseInfoModel>(entityApply);

            var repositoryResponse = new RepositoryResponse<ReleaseInfoModel>
            {
                Content = model
            };

            logger.LogDebug("ReleaseInfoController end call Put -> return Release info");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///  Get Release Info by using Filters
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns>ReleaseInfo list</returns>
        [HttpPost]
        [Route("GetReleaseInfoFiltered")]
        public async Task<IActionResult> GetReleaseInfoFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("ReleaseInfoController called GetReleaseInfoFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("ReleaseInfoController end call GetReleaseInfoFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await repository.GetReleaseInfoFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("ReleaseInfoController end call GetReleaseInfoFiltered -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("ReleaseInfoController end call GetReleaseInfoFiltered -> return Paged Release info");

            return this.PagedJsonResult(models.Content, true);
        }
    }

}