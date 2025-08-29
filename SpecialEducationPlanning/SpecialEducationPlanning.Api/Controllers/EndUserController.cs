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
.Business.IService;
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

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     End User controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EndUserController : Controller
    {
        private readonly IBuilderRepository builderRepository;
        private readonly IPlanRepository planRepository;
        private readonly IEndUserRepository repository;
        private readonly IActionRepository actionRepository;
        private readonly IUserService userService;
        private readonly ILogger<EndUserController> logger;
        private readonly IPostCodeServiceFactory postCodeServiceFactory;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository">Builder repository</param>
        /// <param name="builderRepository"></param>
        /// <param name="planRepository"></param>
        /// <param name="actionRepository"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        /// <param name="postCodeServiceFactory"></param>
        public EndUserController(IEndUserRepository repository,
            IBuilderRepository builderRepository,
            IPlanRepository planRepository,
            IActionRepository actionRepository,
            IUserService userService,
            ILogger<EndUserController> logger,
            IPostCodeServiceFactory postCodeServiceFactory,
            IObjectMapper mapper)
        {
            this.repository = repository;
            this.builderRepository = builderRepository;
            this.planRepository = planRepository;
            this.actionRepository = actionRepository;
            this.userService = userService;
            this.logger = logger;
            this.postCodeServiceFactory = postCodeServiceFactory;
            this.mapper=mapper;
        }

        /// <summary>
        ///     Deletes a builder
        /// </summary>
        /// <param name="id">Builder id to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("EndUserController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<EndUser>(id);

            var model = mapper.Map<EndUser,EndUserModel>(entity);

            if (model == null)
            {
                logger.LogDebug("No user found");

                logger.LogDebug("EndUserController end call Delete -> return Not found");

                return NotFound();
            }
            repository.Remove(id);

            logger.LogDebug("EndUserController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get a end user by id
        /// </summary>
        /// <param name="id">The end user id</param>
        /// <returns>A single end user</returns>
        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("EndUserController called Get");

            var repositoryResponse = await repository.GetEndUserById(id);

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("EndUserController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("EndUserController end call Get -> return End user");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all builders
        /// </summary>
        /// <returns>Builders list</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("EndUserController called GetAll");

            var repositoryResponse = new RepositoryResponse<IEnumerable<EndUserModel>>();
            var endUsers = await repository.GetAllAsync<EndUser>();
            var endUserModels = mapper.Map<IEnumerable<EndUser>, IEnumerable<EndUserModel>>(endUsers);
            repositoryResponse.Content = endUserModels;

            logger.LogDebug("EndUserController end call GetAll -> return All End users");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates a builder
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post([FromBody] EndUserModel value)
        {
            logger.LogDebug("EndUserController called Post -> Create End user");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("EndUserController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var responsePostCode = postCodeServiceFactory.GetService(null).GetPostCode(value.Postcode);
            if (responsePostCode.HasError() || responsePostCode.Content.IsNull())
            {
                logger.LogError("EndUserController: Error bad PostCode");
                responsePostCode.AddError(ErrorCode.GenericBusinessError, "Bad PostCode");

                logger.LogDebug("EndUserController end call Post -> return Bad postcode");

                return responsePostCode.GetHttpResponse();
            }

            value.Postcode = responsePostCode.Content;

            var endUserEntity = mapper.Map<EndUserModel,EndUser>(value);
            var endUserApply = await repository.Add(endUserEntity);
            var endUserModel = mapper.Map<EndUser,EndUserModel>(endUserApply);
            var repositoryResponse = new RepositoryResponse<EndUserModel>
            {
                Content = endUserModel
            };

            logger.LogDebug("EndUserController end call Post -> return Created End user");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates a builder
        /// </summary>
        /// <param name="id">Builder id</param>
        /// <param name="value">Updated builder model</param>
        /// <returns></returns>
        [HttpPut("{id}")]

        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> Put(int id, [FromBody] EndUserModel value)
        {
            logger.LogDebug("EndUserController called Put -> Update builder");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("EndUserController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            var oldEndUserEntity = await this.repository.FindOneAsync<EndUser>(value.Id);
            var oldEndUser = mapper.Map<EndUser,EndUserModel>(oldEndUserEntity);
            if (oldEndUserEntity.IsNull())
            {
                logger.LogDebug("No user found");

                logger.LogDebug("EndUserController end call Put -> return Not found");

                return NotFound();
            }

            var responsePostCode = postCodeServiceFactory.GetService(null).GetPostCode(value.Postcode);
            if (responsePostCode.HasError() || responsePostCode.Content.IsNull())
            {
                logger.LogError("EndUserController: Error bad PostCode");
                responsePostCode.AddError(ErrorCode.GenericBusinessError, "Bad PostCode");

                logger.LogDebug("EndUserController end call Put -> return Bad postcode");

                return responsePostCode.GetHttpResponse();
            }

            value.Postcode = responsePostCode.Content;
            var endUserEntity = mapper.Map<EndUserModel,EndUser>(value, oldEndUserEntity);
            var endUserApply = await repository.ApplyChangesAsync(endUserEntity);
            var endUserModel = mapper.Map<EndUser,EndUserModel>(endUserApply);
            var repositoryResponse = new RepositoryResponse<EndUserModel>
            {
                Content = endUserModel
            };

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("EndUserController end call Put -> return Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            if (repositoryResponse.Content.IsNotNull())
            {
                this.repository.CompareEndUsers(oldEndUser, repositoryResponse.Content);
            }

            logger.LogDebug("EndUserController end call Put -> return Updated End user");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endUser"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("SearchExistingEndUser")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]
        public async Task<IActionResult> GetEndUserGetLatestAiep([FromBody] EndUserModel endUser)
        {
            logger.LogDebug("EndUserController called GetEndUserGetLatestAiep");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("EndUserController end call GetEndUserGetLatestAiep -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var AiepId = userService.GetUserAiepId((ClaimsIdentity)this.User.Identity);
            if (AiepId == 0)
            {
                logger.LogDebug(ErrorCode.UndefinedAiep.GetDescription());

                logger.LogDebug("EndUserController end call GetEndUserGetLatestAiep -> return Bad request undefined Aiep");

                return BadRequest(ErrorCode.UndefinedAiep.GetDescription());
            }

            var responsePostCode = postCodeServiceFactory.GetService(null).GetPostCode(endUser.Postcode);
            if (responsePostCode.HasError() || responsePostCode.Content.IsNull())
            {
                logger.LogError("EndUserController: Error bad PostCode");
                responsePostCode.AddError(ErrorCode.GenericBusinessError, "Bad PostCode");

                logger.LogDebug("EndUserController end call GetEndUserGetLatestAiep -> return Bad postcode");

                return responsePostCode.GetHttpResponse();
            }

            endUser.Postcode = responsePostCode.Content;

            var responseGetEndUser = await repository.GetEndUserByMandatoryFieldsAsync(endUser);
            AiepModel responseAiep = null;

            if (responseGetEndUser.Content == null)
            {
                logger.LogDebug("EndUserController end call GetEndUserGetLatestAiep -> return Ok Null End User");

                return Ok(new { EndUser = responseGetEndUser.Content, Aiep = responseAiep });
            }

            var responseOwnOrLastAiep = await repository.GetEndUserOwnOrLatestUserAiepAsync(responseGetEndUser.Content.Id, AiepId);

            if (responseOwnOrLastAiep.HasError())
            {
                logger.LogDebug("EndUserController end call GetEndUserGetLatestAiep -> return Errors");

                return responseOwnOrLastAiep.GetHttpResponse();
            }

            logger.LogDebug("EndUserController end call GetEndUserGetLatestAiep -> return Ok");

            return Ok(new { EndUser = responseGetEndUser.Content, Aiep = responseOwnOrLastAiep.Content });
        }

        /// <summary>
        ///     After 2 years it removes users within the TDP systems so that it is no longer visible to 
        ///     Educationers or users of the platform
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("EndUserCleanManagement")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> EndUserRightToBeForgottenTwoYearAfterAsync()
        {
            logger.LogDebug("EndUserController called EndUserRightToBeForgottenTwoYearAfterAsync");

            var repositoryResponse = await repository.EndUserCleanManagment();
            if (repositoryResponse.Content == null)
            {
                logger.LogDebug("Error anonymising EndUser");

                logger.LogDebug("EndUserController end call EndUserRightToBeForgottenTwoYearAfterAsync -> return Bad request");

                return BadRequest();
            }

            logger.LogDebug("EndUserController end call EndUserRightToBeForgottenTwoYearAfterAsync -> return Response generic End user clean");

            return repositoryResponse.GetHttpResponse();
        }
    }
}

