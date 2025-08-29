using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
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
using System.Data.Entity.Core.Mapping;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Educationer controller
    /// </summary>
    [Route("api/[controller]")]
    public class EducationerController : Controller
    {
        private readonly IBuilderRepository builderRepository;

        private readonly IAiepRepository AiepRepository;

        private readonly IEducationerRepository repository;

        private readonly IUserService userService;

        private readonly ILogger<EducationerController> logger;

        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="builderRepository"></param>
        /// <param name="AiepRepository"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        public EducationerController(
            IEducationerRepository repository,
            IBuilderRepository builderRepository,
            IAiepRepository AiepRepository,
            IUserService userService,
            ILogger<EducationerController> logger,
            IObjectMapper mapper)
        {
            this.repository = repository;
            this.builderRepository = builderRepository;
            this.AiepRepository = AiepRepository;
            this.userService = userService;
            this.logger = logger;
            this.mapper = mapper;
        }

        #region Public Methods

        /// <summary>
        ///     Deletes a Educationer
        /// </summary>
        /// <param name="id">Educationer ID</param>
        /// <returns>OkResult if the Educationer is deleted.  In case the Educationer doesn't exist, a NotFoundResult</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("EducationerController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<User>(id);
            var model = mapper.Map<User, UserModel>(entity);    

            if (model == null)
            {
                logger.LogDebug("No Educationer found");

                logger.LogDebug("EducationerController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("EducationerController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get Educationer by ID
        /// </summary>
        /// <param name="id">Educationer ID</param>
        /// <returns>If the ID is found returns Educationer Model, if not, it returns a NotFoundResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("EducationerController called Get");

            var entity = await repository.FindOneAsync<User>(id);
            var model = mapper.Map<User, UserModel>(entity);
            var repositoryResponse = new RepositoryResponse<UserModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No Educationer found");

                logger.LogDebug("EducationerController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("EducationerController end call Get -> return Educationer");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all Educationers
        /// </summary>
        /// <returns>Educationers list</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("EducationerController called GetAll");
            IEnumerable<User> userEntity = await repository.GetAllAsync<User>();
            IEnumerable<UserModel> userModel = mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(userEntity);
            var repositoryResponse = new RepositoryResponse<IEnumerable<UserModel>>
            {
                Content = userModel
            };

            logger.LogDebug("EducationerController end call GetAll -> return All Educationers");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieves the release info Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CurrentReleaseInfoId")]
        public async Task<IActionResult> GetReleaseInfo()
        {
            logger.LogDebug("EducationerController called GetReleaseInfo");

            var webUser = (ClaimsIdentity)User.Identity;
            var EducationerEntity = await repository.FindOneAsync<User>(userService.GetUserId(webUser));
            var EducationerModel = mapper.Map<User, UserModel>(EducationerEntity);    
            logger.LogDebug("EducationerController end call GetReleaseInfo -> return Ok");

            return Ok(EducationerModel?.ReleaseInfoId);
        }

        /// <summary>
        ///     Creates a Educationer
        /// </summary>
        /// <param name="value">Educationer model</param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Post([FromBody] UserModel value)
        {
            logger.LogDebug("EducationerController called Post -> Create Educationer");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("EducationerController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            User userEntity = mapper.Map<UserModel,User>(value);

            User userApplyChanges = await repository.Add(userEntity);

            UserModel userModel = mapper.Map<User,UserModel>(userApplyChanges);

            var repositoryResponse = new RepositoryResponse<UserModel>
            {
                Content = userModel
            };

            logger.LogDebug("EducationerController end call Post -> return Educationer created");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates a Educationer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] UserModel value)
        {
            logger.LogDebug("EducationerController called Put -> Update Educationer");

            // TODO Refactor specification
            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("EducationerController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            Domain.Entity.User user = await this.repository.FindOneAsync<Domain.Entity.User>(id);

            if (user.IsNull())
            {
                logger.LogDebug("No Educationer found");

                logger.LogDebug("EducationerController end call Put -> return Not found");

                return NotFound();
            }

            user = mapper.Map<UserModel, User>(value, user);
            User userApplyChanges = await repository.ApplyChangesAsync(user);
            UserModel userModel = mapper.Map<User, UserModel>(userApplyChanges);
            var repositoryResponse = new RepositoryResponse<UserModel>
            {
                Content = userModel
            };

            logger.LogDebug("EducationerController end call Put -> return Updated Educationer");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates the release info for the Educationer
        /// </summary>
        /// <param name="releaseInfoId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReleaseInfoId")]
        public async Task<IActionResult> SetReleaseInfoId(int releaseInfoId)
        {
            logger.LogDebug("EducationerController called SetReleaseInfoId");

            var webUser = (ClaimsIdentity)User.Identity;
            var EducationerEntity = await repository.FindOneAsync<User>(userService.GetUserId(webUser));
            
            if (EducationerEntity.IsNull())
            {
                logger.LogDebug("No Educationer found");

                logger.LogDebug("EducationerController end call SetReleaseInfoId -> return Not found");

                return NotFound();
            }

            EducationerEntity.ReleaseInfoId = releaseInfoId;

            User userApplyChanges = await repository.ApplyChangesAsync(EducationerEntity);
            UserModel userModel = mapper.Map<User, UserModel>(userApplyChanges);

            var repositoryResponse = new RepositoryResponse<UserModel> { Content = userModel };

            logger.LogDebug("EducationerController end call SetReleaseInfoId -> return Educationer updated");

            return repositoryResponse.GetHttpResponse();
        }

        #endregion
    }

}

