using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Koa.Hosting.AspNetCore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.UserInfoModel;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpecialEducationPlanning
.Api.Controllers
{

    /// <summary>
    ///     User Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly IAiepRepository AiepRepository;

        private readonly IEducationerRepository EducationerRepository;

        private readonly ILogger<UserController> logger;

        private readonly string url;

        private readonly IUserRepository userRepository;

        private readonly IUserService userService;

        private readonly IObjectMapper mapper;

        private readonly IFeatureManagementService featureManagementService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userRepository"></param>
        /// <param name="EducationerRepository"></param>
        public UserController(
            IUserService userService,
            IUserRepository userRepository,
            IAiepRepository AiepRepository,
            IEducationerRepository EducationerRepository,
            IOptions<UserInfoUrlConfiguration> options,
            ILogger<UserController> logger,
            IObjectMapper mapper,
            IFeatureManagementService featureManagementService)
        {
            this.userService = userService;
            this.userRepository = userRepository;
            this.EducationerRepository = EducationerRepository;
            this.AiepRepository = AiepRepository;
            url = options.Value.Url;
            this.logger = logger;
            this.mapper = mapper;
            this.featureManagementService = featureManagementService;
        }

        #region Methods Public

        /// <summary>
        ///     Deletes a User
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>OkResult if the User is deleted.  In case the User is not, a BadRequest</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.User_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("UserController called Delete");

            var deleted = await userRepository.DeleteUserAsync(id);

            if (deleted.ErrorList.Any() || !deleted.Content)
            {
                logger.LogDebug("UserController end call Delete -> return Bad request");

                return BadRequest();
            }

            logger.LogDebug("UserController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get a single User by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("UserController called Get");

            var repositoryResponse = await userRepository.GetUserByIdAsync(id);

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No user found");

                logger.LogDebug("UserController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("UserController end call Get -> return User");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Gets all the users that have a given role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns></returns>
        [HttpGet("GetAllUsersByRoleId/{roleId}")]
        public async Task<IActionResult> GetAllUsersByRoleId(int roleId)
        {
            logger.LogDebug("UserController called GetAllUsersByRoleId");

            var result = await userRepository.GetAllUsersByRoleId(roleId);

            logger.LogDebug("UserController end call GetAllUsersByRoleId -> return List of users");

            return result.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve all users with roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUsersWithRoles")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            logger.LogDebug("UserController called GetAllUsersWithRoles");

            var result = await userRepository.GetAllUsersWithRolesAsync();

            logger.LogDebug("UserController end call GetAllUsersWithRoles -> return List of users");

            return result.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve all users with roles and permissions
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUsersWithRolesAndPermissions")]
        public async Task<IActionResult> GetAllUsersWithRolesAndPermissions()
        {
            logger.LogDebug("UserController called GetAllUsersWithRolesAndPermissions");

            var result = await userRepository.GetAllUsersWithRolesAndPermissionsAsync();

            logger.LogDebug("UserController end call GetAllUsersWithRolesAndPermissions -> return List of users");

            return result.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve the curent user's info
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentUserInfo")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            logger.LogDebug("UserController called GetCurrentUserInfo");

            var webUser = (ClaimsIdentity)User.Identity;
            var userRepositoryResponse = await userRepository.GetUserByIdAsync(userService.GetUserId(webUser));

            if (userRepositoryResponse.HasError())
            {
                logger.LogDebug("Errors getting current user info {errors}",
                    userRepositoryResponse.ErrorList.Join("/"));

                logger.LogDebug("UserController end call GetCurrentUserInfo -> return Errors");

                return userRepositoryResponse.GetHttpResponse();
            }

            var releaseInfoResponse = await EducationerRepository.GetPendingReleaseInfo(userRepositoryResponse.Content.Id);

            if (releaseInfoResponse.HasError())
            {
                logger.LogDebug("Errors getting current user info {errors}", releaseInfoResponse.ErrorList.Join("/"));
                releaseInfoResponse.Content = null;
            }

            var assignedAiepId = userService.GetUserAssignedAiepId(webUser);
            var currentAiepId = userService.GetUserCurrentAiepId(webUser);
            var AiepEntity = await AiepRepository.FindOneAsync<Aiep>(assignedAiepId);
            var Aiep = mapper.Map<Aiep, AiepModel>(AiepEntity);
            if (assignedAiepId != -1 && Aiep == null)
            {
                logger.LogError("Errors getting current user info {errors}", "No access to assigned Aiep");

                logger.LogDebug("UserController end call GetCurrentUserInfo -> return Bad request");

                return new BadRequestObjectResult("No access to assigned Aiep");
            }

            AiepModel currentAiep = null;
            if (currentAiepId != -1)
            {
                var entity = await AiepRepository.FindOneAsync<Aiep>(currentAiepId);
                currentAiep = mapper.Map<Aiep, AiepModel>(entity);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.ProductVersion;

            var info = new UserInfoModel(
                Aiep?.AiepCode,
                assignedAiepId,
                currentAiepId,
                currentAiep?.AiepCode,
                userRepositoryResponse.Content,
                userService.GetUserRoles(webUser).Select(c => c.Value),
                userService.GetUserPermissions(webUser).Select(c => c.Value),
                releaseInfoResponse.Content,
                url,
                version,
                await featureManagementService.GetFeatureFlagAsync(FeatureManagementFlagNames.dvProToolEnabled, webUser));

            logger.LogDebug("UserController end call GetCurrentUserInfo -> return Ok");

            return Ok(info);
        }

        [HttpPost]
        [Route("GetUsersWithRolesFiltered")]
        public async Task<IActionResult> GetUsersWithRolesFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("UserController called GetUsersWithRolesFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("UserController end call GetUsersWithRolesFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await userRepository.GetUsersWithRolesFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("UserController end call GetUsersWithRolesFiltered -> return Error");

                return models.GetHttpResponse();
            }

            logger.LogDebug("UserController end call GetUsersWithRolesFiltered -> return Paged users");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Get a single User by ID and its roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUserWithRoles/{id}")]
        public async Task<IActionResult> GetUserWithRoles(int id)
        {
            logger.LogDebug("UserController called GetUserWithRoles");

            var repositoryResponse = await userRepository.GetUserWithRolesAsync(id);

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No user found");

                logger.LogDebug("UserController end call GetUserWithRoles -> return Not found");

                return NotFound();
            }

            logger.LogDebug("UserController end call GetUserWithRoles -> return User with role");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Create a new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.User_Management)]
        public async Task<IActionResult> Post([FromBody] UserModel userModel, int roleId)
        {
            logger.LogDebug("UserController called Post");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("UserController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = await userRepository.CreateUserWithRole(userModel, roleId);
            await userService.ResetClaimsCacheAsync((ClaimsIdentity)User.Identity);

            logger.LogDebug("UserController called Post -> return Created user");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Csv")]
        public async Task<IActionResult> PostUsersFromCsv([FromBody] MultiUploadedFileModel fileUpload)
        {
            logger.LogDebug("UserController called PostUsersFromCsv");

            IEnumerable<UserCsvModel> records;

            if (fileUpload.IsNull())
            {
                logger.LogDebug("UserController end call PostUsersFromCsv -> return Bad request");

                return BadRequest();
            }

            var file = fileUpload.Files.FirstOrDefault();

            using (var stream = file.FileStream)
            {
                records = await userService.ReadCsvFile(stream);
            }

            var userResponse = await userRepository.CreateUserFromCsvModel(records);

            await userService.ResetClaimsCacheAsync();

            logger.LogDebug("UserController end call PostUsersFromCsv -> return Response");

            return userResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Update a user
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="id"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.User_Management)]
        public async Task<IActionResult> Put([FromBody] UserModel userModel, int id, int roleId)
        {
            logger.LogDebug("UserController called Put");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("UserController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var oldUserEntity = await userRepository.FindOneAsync<User>(id);
            var oldUser = mapper.Map<User, UserModel>(oldUserEntity);

            if (!await userRepository.CheckIfExistsAsync(id) || oldUser.IsNull())
            {
                logger.LogDebug("No user found");

                logger.LogDebug("UserController end call Put -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await userRepository.UpdateUserWithRole(userModel, roleId);

            if (oldUser.AiepId.Value != userModel.AiepId.Value)
            {
                var refreshAcl = await userRepository.UpdateEducationerAclAsync(id);

                if (refreshAcl.HasError())
                {
                    logger.LogDebug("UserController end call Put -> return Error");

                    return refreshAcl.GetHttpResponse();
                }
            }
            await userService.ResetClaimsCacheAsync(id);

            logger.LogDebug("UserController end call Put -> return Updated user");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     SetCurrentUserAiepId
        /// </summary>
        /// <param name="AiepId"></param>
        /// <returns></returns>
        [HttpPut("CurrentUserAiepId")]
        [AuthorizeTdpFilter(PermissionType.User_Management)]
        public async Task<IActionResult> SetCurrentUserAiepId([FromBody] int? AiepId)
        {
            logger.LogDebug("UserController called SetCurrentUserAiepId");

            if (AiepId.HasValue && !ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("UserController end call SetCurrentUserAiepId -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var userId = userService.GetUserId(User);
            await userService.ResetClaimsCacheAsync(userId);
            var userResponse = await userRepository.SetUserCurrentAiepId(userId, AiepId);

            logger.LogDebug("UserController end call SetCurrentUserAiepId -> return Response");

            return userResponse.GetHttpResponse();
        }

        [HttpPost("ResetCachePermissionsForUser")]
        [AuthorizeTdpFilter(PermissionType.User_Management)]
        public async Task<IActionResult> ResetCachePermissionsForUser([FromBody] int? userId)
        {
            logger.LogDebug("UserController called ResetCachePermissionsForUser");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("UserController end call ResetCachePermissionsForUser -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            if (userId.IsNull() || userId <= 0)
            {
                userId = userService.GetUserId(User);   //take current user               
            }
            await userService.ResetClaimsCacheAsync(userId ?? 0);
            
            logger.LogDebug("UserController end call ResetCachePermissionsForUser -> return Response");

            return Ok(true);
        }

        /// <summary>
        ///     Gets all the users that have a given Aiep
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns></returns>
        [HttpGet("GetAllUsersByAiepId/AiepId/{AiepId}/userIdEdited/{userIdEdited}")]
        public async Task<IActionResult> GetAllUsersByAiepId(int AiepId, int userIdEdited)
        {
            logger.LogDebug("UserController called GetAllUsersByAiepId");

            var result = await userRepository.GetAllUsersByAiepId(AiepId, userIdEdited);

            logger.LogDebug("UserController end call GetAllUsersByAiepId -> return List of users");

            return result.GetHttpResponse();
        }

        /// <summary>
        /// Deletes the Leavers
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AutomaticDeleteLeavers")]
        [AuthorizeTdpFilter(PermissionType.User_Management)]
        public async Task<IActionResult> AutomaticDeleteLeavers()
        {
            logger.LogDebug("UserController called AutomaticDeleteLeavers");
            var numberOfLeavers = await userRepository.GetNumberofUsersMarkedForDeletion();
            if (numberOfLeavers > 0)
            {
                await userRepository.AutomaticDeleteLeavers();
            }
            logger.LogDebug("UserController end call AutomaticDeleteLeavers -> return Ok");

            return Ok();
        }

        #endregion

    }

}

