using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Configuration.Strategy;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.Publish;
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
using Version = SpecialEducationPlanning
.Domain.Entity.Version;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SpecialEducationPlanning
.Business.Extensions;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Publish controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PublishController : Controller
    {
        private readonly IPublishService _publishService;
        private readonly IPlanRepository _planRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly IActionRepository _actionRepository;
        private readonly ILogger<PublishController> _logger;
        private readonly IUserService _userService;
        private readonly IFeatureManagementService _featureManagementService;
        private readonly CountryConfiguration _countryConfiguration;
        private readonly IObjectMapper mapper;
        private readonly IPublishProjectService publishProjectService;
        private readonly IProjectRepository projectRepo;
        private readonly IPublish3DcService _publish3DcService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="planRepository"></param>
        /// <param name="versionRepository"></param>
        /// <param name="actionRepository"></param>
        /// <param name="logger">Controller logger</param>
        /// <param name="userService"></param>
        /// <param name="featureManagementFactory"></param>
        /// <param name="countryConfiguration"></param>
        public PublishController(
            IPublishService publishService,
            IPlanRepository planRepository,
            IVersionRepository versionRepository,
            IActionRepository actionRepository,
            ILogger<PublishController> logger,
            IUserService userService,
            IFeatureManagementService featureManagementService,
            IOptions<CountryConfiguration> countryConfiguration,
            IObjectMapper mapper,
            IPublishProjectService publishProjectService,
            IProjectRepository projectRepo,
            IPublish3DcService publish3DcService)
        {
            this._publishService = publishService; 
            this._planRepository = planRepository;
            this._versionRepository = versionRepository;
            this._actionRepository = actionRepository;
            this._logger = logger;
            this._userService = userService;
            this._featureManagementService = featureManagementService;
            this._countryConfiguration = countryConfiguration.Value;
            this.mapper = mapper;
            this.publishProjectService = publishProjectService;
            this.projectRepo = projectRepo;
            this._publish3DcService = publish3DcService;
        }

        /// <summary>
        /// Publish Plan by MasterVersion.  Given a Plan ID, extracts the Master Version Version ID from this plan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HealthCheck")]
        public async Task<IActionResult> HealthCheck()
        {
            this._logger.LogDebug("PublishController called HealthCheck");


            if (await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity))
            {
                var response = await _publishService.HealthCheck();

                return response.GetHttpResponse();
            }
            else
            {
                return Ok();
            }

        }

        /// <summary>
        ///     Publish All Plans in a Project by MasterVersion.  
        ///     Given a Project ID, for each plan in project extracts the Master Version Version ID from this plan
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns Status 200 OK in case it works or an error depending the nature of the error wether it's not found or bad request.</returns>
        [HttpPost("Project")]
        [AuthorizeTdpFilter(PermissionType.Plan_Publish)]
        public async Task<IActionResult> PublishProject([FromBody] PublishPlanPostModel request)
        {
            this._logger.LogDebug("PublishController called PublishProject");

            (bool isValid, IActionResult actionResult) = IsInputDataValid(ModelState, request, true);
            if (!isValid)
            {
                return actionResult;
            }                    

            var plans = await projectRepo.GetProjectPlans(request.ProjectId);
            if (plans.HasError())
            {
                this._logger.LogDebug("PublishController end call PublishProject -> return Bad request no project id");

                return BadRequest(plans.ErrorList.First());
            }
            bool isMasterVersionValid = await ValidateMasterVersion(plans.Content);
            
            if (!isMasterVersionValid)
            {
                return NotFound();
            }

            var tenderPackResponse = await publishProjectService.SendRomItemsToCreatioAsync(request.ProjectId);
            if (tenderPackResponse.HasError())
            {
                return BadRequest(tenderPackResponse.ErrorList.First());
            }

            var userFullName = this._userService.GetUserIdentifier((ClaimsIdentity)User.Identity);
            bool featureFlag = await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity);

            foreach (PlanModel plan in plans.Content)
            {
                var publishedPlanResult = await PublishPlanAsync((int)plan.MasterVersionId, plan, userFullName, request, featureFlag);

                if (publishedPlanResult is not OkResult)
                {
                    return publishedPlanResult;
                }
            }

            return Ok();
        }        

        /// <summary>
        ///     Publish Plan by MasterVersion.  Given a Plan ID, extracts the Master Version Version ID from this plan
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns Status 200 OK in case it works or an error depending the nature of the error wether it's not found or bad request.</returns>
        [HttpPost("Plan")]
        [AuthorizeTdpFilter(PermissionType.Plan_Publish)]
        public async Task<IActionResult> PublishPlan([FromBody] PublishPlanPostModel request)
        {
            _logger.LogDebug("PublishController called PublishPlan");

            (bool isValid, IActionResult actionResult) = IsInputDataValid(ModelState, request, false);
            if (!isValid)
            {
                return actionResult;
            }

            // Return the plan to extract the Master Version ID
            var planEntity = await this._planRepository.FindOneAsync<Plan>(request.PlanId);
            var planModel = mapper.Map<Plan, PlanModel>(planEntity);

            if (planModel.IsNull())
            {
                _logger.LogDebug("PublishController end call PublishPlan -> return Not found plan");

                return NotFound();
            }

            int masterVersionId = planModel.MasterVersionId ?? 0;

            if (masterVersionId == 0)
            {
                _logger.LogDebug("PublishController end call PublisPlan -> return Not found version");

                return NotFound();
            }

            var tenderPackResponse = await publishProjectService.SendRomItemsToCreatioAsync(request.ProjectId, request.PlanId);
            if (tenderPackResponse.HasError())
            {
                return BadRequest(tenderPackResponse.ErrorList.First());
            }

            var versionEntity = await this._versionRepository.FindOneAsync<Version> (masterVersionId);
            if (versionEntity.IsNull())
            {
                _logger.LogDebug("PublishController end call PublisPlan -> return Not found version");

                return NotFound();
            }

            var userFullName = this._userService.GetUserIdentifier((ClaimsIdentity)User.Identity);
            bool featureFlag = await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity);

            var publishedPlanResult = await PublishPlanAsync(masterVersionId, planModel, userFullName, request, featureFlag);

            return publishedPlanResult;
        }       

        /// <summary>
        /// Publish Plan By Version
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns Status 200 OK in case it works or an error depending the nature of the error wether it's not found or bad request.</returns>
        [HttpPost("Version")]
        [AuthorizeTdpFilter(PermissionType.Plan_Publish)]
        public async Task<IActionResult> PublishVersion([FromBody] PublishVersionModel request)
        {
            this._logger.LogDebug("PublishController called PublishVersion");

            if (!ModelState.IsValid)
            {
                this._logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                this._logger.LogDebug("PublishController end call PublishVersion -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            if (request.VersionId == 0)
            {
                this._logger.LogDebug("PublishController end call PublishVersion -> return Bad request No version id");

                return BadRequest("Version ID is required");
            }
            if (request.Comments.ContainsOpenHTMLElements())
            {
                this._logger.LogDebug("PublishController end call PublishVersion -> return Bad request contains suspected HTML markup");

                return BadRequest("Version comments contains suspected HTML markup");
            }

            var versionModel = await this._versionRepository.FindOneAsync<Domain.Entity.Version> (request.VersionId);
            if (versionModel.IsNull())
            {
                this._logger.LogDebug("PublishController end call PublishVersion -> return Not found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                this._logger.LogDebug("PublishController end call PublishVersion -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            if (!await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity))
            {
                return Ok();
            }

            var userFullName = this._userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            request.UserEmail = userFullName;
            request.VersionCode = versionModel.ExternalCode;
            request.Country = this._countryConfiguration.StrategyIdentifier;

            if (request.Comments.IsNullOrEmpty())
            {
                request.Comments = string.Empty;
            }
            request.IsCycles = false;
            request.Version = versionModel;

            //Check if user has requested and is configured for Cycles rendering
            var cyclesRequested = request.Comments.Contains("cycles", StringComparison.CurrentCultureIgnoreCase);

            if (cyclesRequested)
            {
                var cyclesEnabledForUser =
                    await _featureManagementService.GetFeatureFlagAsync(FeatureManagementFlagNames.dvCyclesEnabled, (ClaimsIdentity)User.Identity);

                if (cyclesEnabledForUser)
                {
                    request.IsCycles = true;
                    this._logger.LogInformation("PublishController will use Cycles for {userFullName}", userFullName);
                }
            }

            RepositoryResponse<string> response;
            var plan = await this._planRepository.FindOneAsync<Plan>(versionModel.PlanId);
            if (plan.EducationToolOrigin?.Name.Is3Dc() == true)
            {
                response = await _publish3DcService.PublishVersionAsync(request);
            }
            else 
            {
                response = await _publishService.PublishVersionAsync(request);
            }

            string actionComment = request.IsCycles ? "Cycles" : string.Empty;

            await this._actionRepository.CreateAction<PlanModel>(ActionType.PlanPublished, actionComment, versionModel.PlanId, userFullName);

            this._logger.LogDebug("PublishController end call PublishVersion");

            return response.GetHttpResponse();

        }

        /// <summary>
        /// Get Publish Status By VersionCode.  Given a version Code, gets the publish model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPublishJobsByVersionCode")]
        public async Task<IActionResult> GetPublishJobsByVersionCode(string versionCode)
        {
            this._logger.LogDebug("PublishController called GetPublishJobsByVersionCode");

            if (await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity))
            {
                var response = await _publishService.GetPublishJobsByVersionCodesAsync(new Collection<string> { versionCode });
                return response.GetHttpResponse();
            }
            else
            {
                return Ok();
            }

        }

        /// <summary>
        /// Get Publish Status By Job id.  Given a Job Id, gets the publish model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPublishJobByJobId")]
        public async Task<IActionResult> GetPublishJobByJobId(Guid jobId)
        {
            this._logger.LogDebug("PublishController called GetPublishJobByJobId");


            if (await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity))
            {
                var response = await _publishService.GetPublishJobByJobIdAsync(jobId);
                return response.GetHttpResponse();
            }
            else
            {
                return Ok();
            }
        }

        /// <summary>
        /// Get publish jobs By plan Id.  Given a Plan id, gets the publish job for each version
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPublishJobByPlanId")]
        public async Task<IActionResult> GetPublishJobsByPlanId(int planId)
        {
            this._logger.LogDebug("PublishController called GetPublishJobByPlanId");
            var response = new RepositoryResponse<IEnumerable<PublishJobModel>>();
            if (planId == 0)
            {
                this._logger.LogDebug("PublishController end call PublishVersion -> return Bad request No version id");
                return BadRequest("Version ID is required");
            }

            if (!await this._planRepository.CheckIfExistsAsync(planId))
            {
                this._logger.LogDebug($"The plan id {planId} doesn´t exist");
                return BadRequest($"The plan id doesn´t exist");
            }

            var versionModels = await this._versionRepository.GetVersionsByPlanId(planId);

            if(versionModels.IsNull() || !versionModels.Any())
            {
                this._logger.LogDebug($"The plan id {planId} doesn´t have any version");
                return response.GetHttpResponse();
            }

            if(await GetFeatureFlagString(FeatureManagementFlagNames.dvCADRenderingEnabled, (ClaimsIdentity)User.Identity))
            {
                response = await _publishService.GetPublishJobsByVersionCodesAsync(
                                             versionModels.Select(v => v.ExternalCode).ToList());
                return response.GetHttpResponse();
            }
            else
            {
                return Ok();
            }
        }

        private async Task<bool> GetFeatureFlagString(string flagName, ClaimsIdentity claimsIdentity)
        {
            this._logger.LogDebug("PublishController called GetFeatureFlagString for flag {flagName}", flagName);

            if (await this._featureManagementService.GetFeatureStringFlagAsync(flagName, claimsIdentity) == FeatureFlagEnum.On)
            {
                this._logger.LogDebug("GetFeatureFlagString -> Flag is ON");

                return true;
            }
            else
            {
                this._logger.LogDebug("GetFeatureFlagString -> Flag is OFF");

                return false;
            }
        }

        private async Task<PublishVersionModel> CreatePublishVersionModelAsync(int masterVersionId, PlanModel planModel, string userName, PublishPlanPostModel request)
        {
            Version version = await _versionRepository.GetVersionWithPlanProjectAiep(masterVersionId);
            Plan plan = await _planRepository.GetPlanWithHousingTypeHousingSpecs(version.PlanId);

            var isCHTPrequest = false;
            var destination = DestinationEnum.MY_KITCHEN;
            var crmProjectCode = version.Plan?.Project?.KeyName;
            var crmHousingType = plan?.HousingType?.Code;
            var crmHousingSpecificationCode = plan?.HousingType?.HousingSpecification?.Code;

            if (crmProjectCode.IsNotNull() ||
                crmHousingType.IsNotNull() ||
                crmHousingSpecificationCode.IsNotNull())
            {
                destination = DestinationEnum.CONTRACT_HUB;
                isCHTPrequest = true;
            }

            var publishVersion = new PublishVersionModel()
            {
                VersionId = planModel.MasterVersionId.Value,
                EducationerEmail = request.EducationerEmail ?? plan.CreationUser,
                ReceipientEmail1 = request.ReceipientEmail1,
                ReceipientEmail2 = request.ReceipientEmail2,
                Comments = request.Comments,
                SelectedMusic = request.SelectedMusic,
                UserEmail = userName,
                Country = this._countryConfiguration.StrategyIdentifier,
                VersionCode = version.ExternalCode,
                CrmProjectCode = crmProjectCode,
                CrmHousingType = crmHousingType,
                CrmHousingSpecificationCode = crmHousingSpecificationCode,
                Destination = destination,
                IsCHTPrequest = isCHTPrequest,
                Version = version,
                PlanCode = planModel.PlanCode,
            };
            return publishVersion;
        }

        private async Task<bool> ValidateMasterVersion(IEnumerable<PlanModel> plans)
        {
            bool isValid = true;

            if (plans.Count() == 0)
            {
                _logger.LogDebug($"PublishController end call PublishPlan -> return Not found any Plan to publish");
                isValid = false;
            }
            else
            {
                foreach (PlanModel plan in plans)
                {
                    int masterVersionId = plan.MasterVersionId ?? 0;

                    if (masterVersionId == 0 ||
                       (!await _versionRepository.CheckIfExistsAsync(masterVersionId)))
                    {
                        _logger.LogDebug($"PublishController end call PublishPlan -> return Not found master version for planId = {plan.Id}");

                        isValid = false;
                        break;
                    }
                }
            }
            return isValid;
        }

        private async Task<IActionResult> PublishPlanAsync(int masterVersionId, PlanModel planModel, string userFullName, PublishPlanPostModel request, bool featureFlag)
        {
            PublishVersionModel publishVersion = await CreatePublishVersionModelAsync(masterVersionId, planModel, userFullName, request);

            if ((publishVersion.IsCHTPrequest && request.CadPublishingSelected) ||
                 !publishVersion.IsCHTPrequest)
            {
                if (featureFlag)
                {
                    var response = await _publishService.PublishVersionAsync(publishVersion);

                    this._logger.LogDebug("PublishController PublishPlan publish service response -> {response}", response.GetHttpResponse());

                    await this._actionRepository.CreateAction<PlanModel>(ActionType.PlanPublished, string.Empty, planModel.Id, userFullName);

                    this._logger.LogDebug("PublishController end call PublishPlan");

                    return response.GetHttpResponse();
                }
            }

            return Ok();
        }

        private (bool, IActionResult) IsInputDataValid(ModelStateDictionary modelState, PublishPlanPostModel request, bool isProject)
        {
            var publishType = isProject ? "PublishProject" : "PublishPlan";

            if (!ModelState.IsValid)
            {
                _logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));
                _logger.LogDebug($"PublishController end call {publishType} -> return Bad request");

                return (false, BadRequest(ModelState.GetErrorMessages()));
            }
            if (isProject && request.ProjectId == 0)
            {
                this._logger.LogDebug($"PublishController end call {publishType} -> return Bad request no project id");

                return (false, BadRequest("Plan ID is required"));
            }
            if (!isProject && request.PlanId == 0)
            {
                this._logger.LogDebug($"PublishController end call {publishType} -> return Bad request no plan id");

                return (false, BadRequest("Plan ID is required"));
            }
            if (request.Comments.ContainsOpenHTMLElements())
            {
                this._logger.LogDebug($"PublishController end call {publishType} -> return Bad request contains suspected HTML markup");

                return (false, BadRequest("Plan comments contains suspected HTML markup"));
            }
            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                this._logger.LogDebug($"PublishController end call {publishType} -> return Bad request Undefined User");

                return (false, new UnauthorizedResult());
            }
            return (true, null);
        }
    }
}

