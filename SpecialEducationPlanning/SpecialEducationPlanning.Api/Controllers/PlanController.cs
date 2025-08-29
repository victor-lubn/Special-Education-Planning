using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Hosting.AspNetCore.Controller;
using Koa.Persistence.Abstractions.QueryResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.AutomaticArchiveModel;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.Search;
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
.Business.Model.Project;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpecialEducationPlanning
.Api.Controllers
{

    /// <summary>
    ///     Plan Controller
    /// </summary>
    [Route("api/[controller]")]
    public class PlanController : Controller
    {
        private readonly IActionRepository actionRepository;

        private readonly IBuilderEducationerAiepRepository builderEducationerAiepRepository;

        private readonly IBuilderRepository builderRepository;

        private readonly ICatalogRepository catalogRepository;

        private readonly ICommentRepository commentRepository;

        private readonly IAiepRepository AiepRepository;

        private readonly IEducationerRepository EducationerRepository;

        private readonly IEndUserRepository endUserRepository;

        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;

        private readonly IOptions<AutomaticArchiveConfiguration> options;
        private readonly IUserService userService;
        private readonly IUserRepository userRepository;
        private readonly IProjectRepository projectRepository;

        private readonly IPlanRepository repository;

        private readonly IRomItemRepository romItemRepository;

        private readonly IVersionRepository versionRepository;

        private readonly ILogger<PlanController> logger;
        private readonly IPostCodeServiceFactory postCodeServiceFactory;
        private readonly AzureSearchConfiguration azureSearchConfiguration;
        private readonly IAzureSearchService azureSearchService;
        private readonly IObjectMapper mapper;
        private readonly IFeatureManagementService _featureManagementService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EducationerRepository"></param>
        /// <param name="builderRepository"></param>
        /// <param name="projectRepository"></param>
        /// <param name="AiepRepository"></param>
        /// <param name="endUserRepository"></param>
        /// <param name="actionRepository"></param>
        /// <param name="commentRepository"></param>
        /// <param name="versionRepository"></param>
        /// <param name="catalogRepository"></param>
        /// <param name="romItemRepository"></param>
        /// <param name="builderEducationerAiepRepository"></param>
        /// <param name="fileStorageService"></param>
        /// <param name="options"></param>
        /// <param name="userService"></param>
        /// <param name="userRepository"></param>
        /// <param name="postCodeServiceFactory"></param>
        /// <param name="logger"></param>
        public PlanController(IPlanRepository repository,
            IEducationerRepository EducationerRepository,
            IBuilderRepository builderRepository,
            IProjectRepository projectRepository,
            IAiepRepository AiepRepository,
            IEndUserRepository endUserRepository,
            IActionRepository actionRepository,
            ICommentRepository commentRepository,
            IVersionRepository versionRepository,
            ICatalogRepository catalogRepository,
            IBuilderEducationerAiepRepository builderEducationerAiepRepository,
            IRomItemRepository romItemRepository,
            IFileStorageService<AzureStorageConfiguration> fileStorageService,
            IOptions<AutomaticArchiveConfiguration> options,
            IUserService userService,
            IUserRepository userRepository,
            IPostCodeServiceFactory postCodeServiceFactory,
            ILogger<PlanController> logger,
            IOptions<AzureSearchConfiguration> configuration,
            IAzureSearchService azureSearchService,
            IObjectMapper mapper,
            IFeatureManagementService featureManagementService)
        {
            this.repository = repository;
            this.EducationerRepository = EducationerRepository;
            this.builderRepository = builderRepository;
            this.projectRepository = projectRepository;
            this.AiepRepository = AiepRepository;
            this.endUserRepository = endUserRepository;
            this.actionRepository = actionRepository;
            this.commentRepository = commentRepository;
            this.versionRepository = versionRepository;
            this.catalogRepository = catalogRepository;
            this.romItemRepository = romItemRepository;
            this.builderEducationerAiepRepository = builderEducationerAiepRepository;
            this.fileStorageService = fileStorageService;
            this.options = options;
            this.userService = userService;
            this.userRepository = userRepository;
            this.logger = logger;
            this.postCodeServiceFactory = postCodeServiceFactory;
            this.azureSearchConfiguration = configuration.Value;
            this.azureSearchService = azureSearchService;
            this.mapper = mapper;
            this._featureManagementService = featureManagementService;
        }

        #region Methods Public

        /// <summary>
        ///     Assign a builder to a plan
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="builderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AssignBuilderToPlan")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> AssignBuilderToPlan(int planId, int builderId)
        {
            logger.LogDebug("PlanController called AssignBuilderToPlan");

            var planEntity = await repository.FindOneAsync<Plan>(planId);
            var plan = mapper.Map<Plan, PlanModel>(planEntity);

            //We check that the given plan exist and is unnasigned
            if (plan.IsNotNull())
            {
                var builderEntity = await builderRepository.FindOneAsync<Builder>(builderId);
                var builder = mapper.Map<Builder, BuilderModel>(builderEntity);

                if (builder.IsNotNull())
                {
                    if (((ClaimsIdentity)User.Identity).IsNull())
                    {
                        logger.LogDebug("Undefined User");

                        logger.LogDebug("PlanController end call AssignBuilderToPlan -> return Bad request Undefined user");

                        return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
                    }

                    var repositoryResponse = await repository.AssignBuilderToPlan(plan.Id, builder);

                    logger.LogDebug("PlanController end call AssignBuilderToPlan -> return Plan");

                    return repositoryResponse.GetHttpResponse();
                }

                //BuilderNotFound
                logger.LogDebug("No builder found");

                logger.LogDebug("PlanController end call AssignBuilderToPlan -> return Error Entity not found Builder");

                return new RepositoryResponse<PlanModel>
                { Content = null, ErrorList = new Collection<string> { ErrorCode.EntityNotFound.GetDescription() } }
                     .GetHttpResponse();
            }

            //PlanNotFound
            logger.LogDebug("No plan found");

            logger.LogDebug("PlanController end call AssignBuilderToPlan -> return Error Entity not found Plan");

            return new RepositoryResponse<PlanModel>
            { Content = null, ErrorList = new Collection<string> { ErrorCode.EntityNotFound.GetDescription() } }
                .GetHttpResponse();
        }

        /// <summary>
        ///     Unassign a builder from a plan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/UnassignBuilder")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> UnassignBuilderFromPlan(int id)
        {
            logger.LogDebug("PlanController called UnassignBuilderFromPlan");

            var planEntity = await repository.FindOneAsync<Plan>(id);
            var plan = mapper.Map<Plan, PlanModel>(planEntity);
            //We check that the given plan exist and is unnasigned
            if (plan.IsNotNull())
            {
                if (((ClaimsIdentity)User.Identity).IsNull())
                {
                    logger.LogDebug("Undefined User");

                    logger.LogDebug("PlanController end call UnassignBuilderFromPlan -> return Bad request Undefined user");

                    return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
                }

                var repositoryResponse = await repository.UnassignBuilderFromPlan(id);

                logger.LogDebug("PlanController end call UnassignBuilderFromPlan -> return Plan");

                return repositoryResponse.GetHttpResponse();
            }

            //PlanNotFound
            logger.LogDebug("No plan found");

            logger.LogDebug("PlanController end call UnassignBuilderFromPlan -> return Entity not found Plan");

            return new RepositoryResponse<PlanModel>
            { Content = null, ErrorList = new Collection<string> { ErrorCode.EntityNotFound.GetDescription() } }
                .GetHttpResponse();
        }

        /// <summary>
        ///     Automatically archive plans
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AutomaticArchive")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> AutomaticArchive()
        {
            logger.LogDebug("PlanController called AutomaticArchive");

            var archiveDays = int.Parse(options.Value.Archive, CultureInfo.InvariantCulture);
            var delete = double.Parse(options.Value.Delete, CultureInfo.InvariantCulture);
            await repository.AutomaticArchive(archiveDays);
            await repository.AutomaticDeletion(delete);

            logger.LogDebug("PlanController end call AutomaticArchive -> return Ok");

            return Ok();
        }


        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="planState"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePlanState/{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> ChangePlanState(int id, [FromBody] PlanState planState)
        {
            logger.LogDebug("PlanController called ChangePlanState");

            var planEntity = await repository.FindOneAsync<Plan>(id);
            var planModel = mapper.Map<Plan, PlanModel>(planEntity);    
            if (planModel.IsNull())
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call ChangePlanState -> return No found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("PlanController end call ChangePlanState -> return Bad request undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var repositoryResponse = await repository.ChangePlanStateAsync(planModel, planState);

            logger.LogDebug("PlanController end call ChangePlanState -> return Plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Copy a plan
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="builderId"></param>
        /// <param name="AiepId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{planId}/CopyAndAssignToBuilder/{builderId}/Aiep/{AiepId}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> CopyPlan(int planId, int builderId, int AiepId)
        {
            logger.LogDebug("PlanController called CopyPlan");

            var planModel = (await repository.GetPlanAsync(planId)).Content;

            if (planModel.IsNull())
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call CopyPlan -> return Not found Plan");

                return NotFound();
            }

            var findProjectEntity = await projectRepository.FindOneAsync<Project>(planModel.ProjectId);

            if (findProjectEntity.IsNull())
            {
                logger.LogDebug("No project found");

                logger.LogDebug("PlanController end call CopyPlan -> return Not found Project");

                return NotFound();
            }

            if (!await builderRepository.CheckIfExistsAsync(builderId))
            {
                logger.LogDebug("No builder found");

                logger.LogDebug("PlanController end call CopyPlan -> return Not found Builder");

                return NotFound();
            }

            if (!await AiepRepository.CheckIfExistsAsync(AiepId))
            {
                logger.LogDebug("No Aiep found");

                logger.LogDebug("PlanController end call CopyPlan -> return Not found Aiep");

                return NotFound();
            }

            var projectModel = mapper.Map<Project, ProjectModel>(findProjectEntity);

            var newProjectResponse = await projectRepository.CopyToAiep(projectModel, AiepId);
            var projectEntity =  await projectRepository.ApplyChangesAsync(mapper.Map<ProjectModel, Project>(newProjectResponse.Content));
            var newProjectModel = mapper.Map<Project, ProjectModel>(projectEntity);

            var newPlan =
                await repository.CreateOrUpdateAsync((await repository.CopyToProject(planModel, newProjectModel.Id))
                    .Content);

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("PlanController end call CopyPlan -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            if (!newPlan.ErrorList.Any())
            {
                var versions = await versionRepository.GetVersionsByPlanId(planId);


                if (versions.Any())
                {
                    foreach (var version in versions)
                    {
                        version.PlanId = newPlan.Content.Id;
                        var versionInfo = await versionRepository.GetRomAndPreviewInfo(version.Id);
                        var createdVersion = await versionRepository.CreateVersionAsync(version);

                        //Replicates Rom file
                        var romFile = await fileStorageService.DownloadAsync(versionInfo.Content.RomPath);

                        if (romFile.IsNotNull())
                        {
                            var romPath = await fileStorageService.UploadAsync<Version>(romFile);

                            await versionRepository.SetVersionRom(createdVersion.Id, romPath, versionInfo.Content.Rom,
                                string.Empty);

                            await actionRepository.CreateAction<VersionModel>(ActionType.FileCreate, string.Empty, createdVersion.Id, userFullName);
                        }

                        //Replicates Preview file
                        var previewFile = await fileStorageService.DownloadAsync(versionInfo.Content.PreviewPath);

                        if (previewFile.IsNotNull())
                        {
                            var previewPath = await fileStorageService.UploadAsync<Version>(previewFile);

                            await versionRepository.SetVersionPreview(createdVersion.Id, previewPath,
                                versionInfo.Content.Preview);

                            await actionRepository.CreateAction<VersionModel>(ActionType.FileCreate, string.Empty, createdVersion.Id, userFullName);
                        }

                        await actionRepository.CreateAction<VersionModel>(
                            ActionType.Create,
                            createdVersion.VersionNumber.ToString(),
                            createdVersion.Id,
                            userFullName);
                    }
                }
            }

            await actionRepository.CreateAction<PlanModel>(ActionType.Create, string.Empty, newPlan.Content.Id, userFullName);

            var repositoryResponse =
                new RepositoryResponse<PlanModel>(await repository.AssignBuilder(newPlan.Content.Id, builderId));

            if ((await builderEducationerAiepRepository.GetBuilderEducationerAiepModelByBuilderIdAiepId(builderId, AiepId)
                ).Content.IsNull())
            {
                await builderEducationerAiepRepository.CreateBuilderEducationerAiepModelRelation(builderId, AiepId);
            }

            logger.LogDebug("PlanController end call CopyPlan -> return Plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Delete a plan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("PlanController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<Plan>(id);
            var model = mapper.Map<Plan, PlanModel>(entity);
            if (model == null)
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("PlanController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Delete empty plans
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteEmptyPlans")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> DeleteEmptyPlans()
        {
            logger.LogDebug("PlanController called DeleteEmptyPlans");

            var response = await repository.DeleteEmptyPlans();

            if (response)
            {
                logger.LogDebug("PlanController end call DeleteEmptyPlans -> return Ok");

                return Ok();
            }
            logger.LogDebug("No plan found");

            logger.LogDebug("PlanController end call DeleteEmptyPlans -> return Bad request");

            return BadRequest();
        }

        /// <summary>
        ///     Generate a new plan Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("GeneratePlanCode")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]

        public async Task<IActionResult> GeneratePlanId()
        {
            logger.LogDebug("PlanController called GeneratePlanId");

            var repositoryReponse = await repository.GeneratePlanIdAsync();

            logger.LogDebug("PlanController end call GeneratePlanId -> return String Plan id");

            return repositoryReponse.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve plans by project Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetPlansForProject/{projectId}")]
        public async Task<IActionResult> GetPlansForProject(int projectId)
        {
            logger.LogDebug("PlanController called GetPlansForProject");

            var repositoryResponse = await repository.GetAllPlansForProjectAsync(projectId);

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No plans found");

                logger.LogDebug("PlanController end call GetPlansForProject -> return Not found");

                return NotFound();
            }

            logger.LogDebug("PlanController end call GetPlansForProject -> return Plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve a plan by its plan Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("PlanController called Get");

            var repositoryResponse = await repository.GetPlanAsync(id);

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("PlanController end call Get -> return Plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Gets plan actions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Actions")]
        public async Task<IActionResult> GetActions(int id)
        {
            logger.LogDebug("PlanController called GetActions");

            var planEntity = await repository.FindOneAsync<Plan>(id);
            var planModel = mapper.Map<Plan, PlanModel>(planEntity);

            if (planModel.IsNull())
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call GetActions -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await actionRepository.GetModelActions<PlanModel>(id);

            if (planModel.EndUserId.IsNotNull())
            {
                var endUserActionsRepositoryResponse =
                    await actionRepository.GetModelActions<EndUserModel>(planModel.EndUserId.Value);

                repositoryResponse.Content.AddRange(endUserActionsRepositoryResponse.Content);
            }

            logger.LogDebug("PlanController end call GetActions -> return Action");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all plans
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("PlanController called GetAll");

            var repositoryResponse = await repository.GetAllPlansWithoutArchivedPlansAsync();

            logger.LogDebug("PlanController end call GetAll -> return All Plans");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all archived plans
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Archived")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> GetAllWithArchived()
        {
            logger.LogDebug("PlanController called GetAllWithArchived");

            var repositoryResponse = await repository.GetAllPlansWithArchivedPlansAsync();

            logger.LogDebug("PlanController end call GetAllWithArchived -> return All plans including archived");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Returns all Archived Plans (Ignoring Query Filters)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAllArchivedPlans")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> GetAllArchivedPlans([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("PlanController called GetAllArchivedPlans");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("PlanController end call GetAllArchivedPlans -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await repository.GetAllArchivedPlansAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("PlanController end call GetAllArchivedPlan -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("PlanController end call GetAllArchivedPlans -> return Paged archived plans");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Gets plan and children actions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/PlanActions")]
        public async Task<IActionResult> GetPlanActions(int id)
        {
            logger.LogDebug("PlanController called GetPlanActions");

            var planEntity = await repository.FindOneAsync<Plan>(id);
            var planModel = mapper.Map<Plan, PlanModel>(planEntity);

            if (planModel.IsNull())
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call GetPlanActions -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await actionRepository.GetPlanActions<PlanModel, VersionModel>(id);

            if (planModel.EndUserId.IsNotNull())
            {
                var endUserActionsRepositoryResponse =
                    await actionRepository.GetModelActions<EndUserModel>(planModel.EndUserId.Value);

                repositoryResponse.Content.AddRange(endUserActionsRepositoryResponse.Content);
            }

            logger.LogDebug("PlanController end call GetPlanActions -> return Plan Actions");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get plans by using Filters
        /// </summary>
        /// <returns>A plan list. This action is not returning archived plans</returns>
        [HttpPost]
        [Route("GetPlansFiltered")]

        public async Task<IActionResult> GetPlansFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("PlanController called GetPlansFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("PlanController end call GetPlansFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var currentAiepId = userService.GetUserCurrentAiepId((ClaimsIdentity)this.User.Identity);

            if (azureSearchConfiguration.LefthandSearchEnabled && azureSearchService.GetType() == typeof(AzureSearchService))
            {
                var skipValue = searchModel.Skip ?? 0;
                var takeValue = searchModel.Take ?? 100;

                var idsFiltered = await azureSearchService.GetPlanIdsFilteredAsync(searchModel, currentAiepId);

                if (takeValue <= 0)
                {
                    var result = new PagedQueryResult<PlanModel>(new List<PlanModel>(), takeValue, skipValue, idsFiltered.TotalCount);
                    logger.LogDebug("PlanController end call GetPlansFiltered -> take <= 0. Return Paged plans Left hand search enabled");
                    return this.PagedJsonResult(result, true);
                }

                var typesAndIds = idsFiltered.PlanFilteredIds;
                var plans = await repository.GetPlansByIdsAndTypeAsync(typesAndIds, skipValue, takeValue, currentAiepId, idsFiltered.Sort, searchModel.Filters);
                var pagedQuery = new PagedQueryResult<PlanModel>(plans.Content, takeValue, skipValue, idsFiltered.TotalCount);

                logger.LogDebug("PlanController end call GetPlansFiltered -> return Paged plans Left hand search enabled");

                return this.PagedJsonResult(pagedQuery, true);
            }
            else
            {
                var models = await repository.GetPlansFilteredAsync(searchModel, currentAiepId);

                if (models.ErrorList.Any())
                {
                    logger.LogError("Error found: {error}", models.ErrorList.Join("/"));
                    return models.GetHttpResponse();
                }

                logger.LogDebug("PlanController end call GetPlansFiltered -> return Paged plans Left hand search disabled");

                return this.PagedJsonResult(models.Content, true);
            }
        }

        /// <summary>
        ///     Retrieve the versions for a plan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{planId}/Version")]
        public async Task<IActionResult> GetPlanVersions(int planId)
        {
            logger.LogDebug("PlanController called GetPlanVersions");

            var response = new RepositoryResponse<IEnumerable<VersionModel>>();

            if (!await repository.CheckIfExistsAsync(planId))
            {
                response.ErrorList.Add(ErrorCode.EntityNotFound.GetDescription());
                response.ErrorList.Add(ErrorCode.NoResults.GetDescription());
            }
            else
            {
                response.Content = await versionRepository.GetVersionsByPlanId(planId);
            }

            logger.LogDebug("PlanController end call GetPlanVersions -> return List of versions");

            return response.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="AiepId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{planId}/TransferSinglePlanBetweenAieps/{AiepId}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> TransferSinglePlanBetweenAieps(int planId, int AiepId)
        {
            logger.LogDebug("PlanController called TransferSinglePlanBetweenAieps");

            var response = new RepositoryResponse<PlanModel>();
            if (!await repository.CheckIfExistsAsync(planId))
            {
                logger.LogDebug("No plan found");
                response.ErrorList.Add("No plan found");

                logger.LogDebug("PlanController end call TransferSiglePlanBetweenAieps -> return Error Plan not found");

                return response.GetHttpResponse();
            }

            if (!await AiepRepository.CheckIfExistsAsync(AiepId))
            {
                logger.LogDebug("No Aiep found");
                response.ErrorList.Add("No Aiep found");

                logger.LogDebug("PlanController end call TransferSinglePlanBetweenAieps -> return Error Aiep not found");

                return response.GetHttpResponse();
            }

            response = await repository.TransferSinglePlanBetweenAieps(planId, AiepId);

            logger.LogDebug("PlanController end call TransferSinglePlanBetweenAieps -> return Plan");

            return response.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="builderId"></param>
        /// <param name="AiepId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{builderId}/TransferMultiplePlanBetweenAieps/{AiepId}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> TransferMultiplePlanBetweenAieps(int builderId, int AiepId)
        {
            logger.LogDebug("PlanController called TransferMultiplePlanBetweenAieps");

            var response = new RepositoryResponse<IEnumerable<PlanModel>>();
            if (!await builderRepository.CheckIfExistsAsync(builderId))
            {
                logger.LogDebug("No builder found");
                response.ErrorList.Add("No builder found");

                logger.LogDebug("PlanController end call TransferMultiplePlanBetweenAieps -> return Error Builder not found");

                return response.GetHttpResponse();
            }

            if (!await AiepRepository.CheckIfExistsAsync(AiepId))
            {
                logger.LogDebug("No Aiep found");
                response.ErrorList.Add("No Aiep found");

                logger.LogDebug("PlanController end call TransferMultiplePlanBetweenAieps -> return Error Aiep not found");

                return response.GetHttpResponse();
            }

            response = await repository.TransferMultiplePlanToUnassignedBuilder(builderId, AiepId);

            logger.LogDebug("PlanController end call TransferMultiplePlanBetweenAieps -> return List of plans");

            return response.GetHttpResponse();
        }

        [HttpPost]
        [Route("{projectId:int}/TransferProjectPlansToAiep/{AiepCode}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> TransferProjectPlansToAiep(int projectId, string AiepCode)
        {
            logger.LogDebug("PlanController called TransferProjectPlansToAiep");

            var response = new RepositoryResponse<bool>();

            response = await repository.TransferMultipleProjectPlansToAiep(projectId, AiepCode.ToUpper());

            logger.LogDebug("PlanController end call TransferMultiplePlanBetweenAieps -> return List of plans");

            return response.GetHttpResponse();
        }


        /// <summary>
        ///     Receives a Plan ID and returns all the comments for that Plan
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <returns>List of Comments</returns>
        [HttpGet("{id}/Comments")]
        [AuthorizeTdpFilter(PermissionType.Plan_Comment)]
        public async Task<IActionResult> GetComments(int id)
        {
            logger.LogDebug("PlanController called GetComments");

            var repositoryResponse = await commentRepository.GetModelComments<PlanModel>(id);

            logger.LogDebug("PlanController end call GetComments -> return All comments");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates a comment for a plan
        /// </summary>
        /// <param name="id"></param>
        /// <param name="planComment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/Comments")]
        [AuthorizeTdpFilter(PermissionType.Plan_Comment)]

        public async Task<IActionResult> PostCommentPlan(int id, [FromBody] CommentModel planComment)
        {
            logger.LogDebug("PlanController called PostCommentPlan");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("PlanController end call PostCommentPlan -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            var userFullName = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);

            logger.LogDebug("PlanController end call PostCommentPlan -> return Ok");

            return Ok((await commentRepository.CreateComment<PlanModel>(planComment, id, userFullName)).Content);
        }

        /// <summary>
        ///     Saves a plan
        /// </summary>
        /// <param name="planModel">Plan to be created</param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostSinglePlan")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> PostSinglePlan([FromBody] PlanModel planModel)
        {
            logger.LogDebug("PlanController called Post");

            var userIdentity = ((ClaimsIdentity)User.Identity);
            var userAiepId = userService.GetUserAiepId(userIdentity);

            if (planModel == null)
            {
                logger.LogDebug("Undefined planModel");

                logger.LogDebug("PlanController end call PostSinglePlan -> return Bad request Undefined planModel");

                return new BadRequestObjectResult(ErrorCode.NullOrWhitespace.GetDescription());
            }

            var validationResult = await ValidatePassedData(ModelState, planModel, userIdentity, userAiepId);

            if (((StatusCodeResult)validationResult).StatusCode == StatusCodes.Status400BadRequest) 
            {
                return validationResult;
            }

            var dvProToolEnabledForUser = await _featureManagementService.GetFeatureFlagAsync(FeatureManagementFlagNames.dvProToolEnabled, userIdentity);
            planModel.EducationOrigin = dvProToolEnabledForUser ? EducationOriginType.ThreeDc.GetDescription() : EducationOriginType.Fusion.GetDescription();

            var userId = userService.GetUserId(userIdentity);

            RepositoryResponse<PlanModel> repositoryResponse;

            repositoryResponse = await repository.CreatePlan(planModel, userAiepId, userId);

            logger.LogDebug("PlanController end call Post -> return Created plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Saves a plan for Tender Pack project
        /// </summary>
        /// <param name="planModel">Plan to be created</param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostSingleTenderPackPlan")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> PostSingleTenderPackPlan([FromBody] PlanModel planModel)
        {
            logger.LogDebug("PlanController called Post");

            var userIdentity = ((ClaimsIdentity)User.Identity);
            var userAiepId = userService.GetUserAiepId(userIdentity);

            var validationResult = await ValidatePassedData(ModelState, planModel, userIdentity, userAiepId);

            if (((StatusCodeResult)validationResult).StatusCode == StatusCodes.Status400BadRequest)
            {
                return validationResult;
            }

            var userId = userService.GetUserId(userIdentity);

            RepositoryResponse<PlanModel> repositoryResponse;

            repositoryResponse = await repository.CreateTenderPackPlan(planModel, userAiepId, userId);

            logger.LogDebug("PlanController end call Post -> return Created plan");

            return repositoryResponse.GetHttpResponse();
        }        

        /// <summary>
        ///     Update a plan
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> Put([FromBody] PlanModel value)
        {
            logger.LogDebug("PlanController called Put");

            if (value.EndUser != null && value.EndUser.Postcode != null)
                value.EndUser.Postcode = postCodeServiceFactory.GetService(null).NormalisePostcode(value.EndUser.Postcode);

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("PlanController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (!await repository.CheckIfExistsAsync(value.Id))
            {
                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call Put -> return Not found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("PlanController end call Put -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var repositoryResponse = await repository.CreateOrUpdateAsync(value);

            logger.LogDebug("PlanController end call Put -> return Plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Update a CHTP plan
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateTenderPackPlan")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> UpdateTenderPackPlan([FromBody] TenderPackUpdatePlanModel newPlanValues)
        {
            logger.LogDebug("PlanController called UpdateTenderPackPlan");
         
            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("PlanController end call UpdateTenderPackPlan -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }          

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("PlanController end call Put -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var repositoryResponse = await repository.UpdateTenderPackPlanAsync(newPlanValues);

            logger.LogDebug("PlanController end call UpdateTenderPackPlan -> return Plan");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve a list of sorted plans
        /// </summary>
        /// <returns>A plan list. This action is not returning archived plans</returns>
        [HttpPost]
        [Route("GetPlansSorted")]
        public async Task<IActionResult> GetPlansSorted(int builderId, [FromBody] SortDescriptor sortModel, bool GetOnlyArchivedPlans = false)
        {
            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));
                return BadRequest(ModelState.GetErrorMessages());
            }
            var currentAiepId = userService.GetUserCurrentAiepId((ClaimsIdentity)this.User.Identity);
            var models = await repository.GetPlansSortedAsync(sortModel, builderId, currentAiepId, GetOnlyArchivedPlans);
            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));
                return models.GetHttpResponse();
            }
            return this.PagedJsonResult(models.Content, true);
        }

        [HttpGet]
        [Route("DuplicatedPlan")]
        public async Task<IActionResult> Get([BindRequired] string planName)
        {
            logger.LogDebug($"PlanController called GET /duplicatedPlan?planName={planName}");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug($"PlanController end call GET /duplicatedPlan?planName={planName} -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug($"PlanController end call GET /duplicatedPlan?planName={planName} -> return Bad request Undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var repositoryResponse = await repository.IsPlanNameDuplicateAsync(planName);

            logger.LogDebug($"PlanController end call GET /duplicatedPlan?planName={planName} -> return bool");

            return repositoryResponse.GetHttpResponse();
        }

        [HttpGet]
        [Route("GetAllPlanTypes")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<List<KeyValuePair<int, string>>> GetAllPlanTypes()
        {
            logger.LogDebug("PlanController called GetAllPlanTypes");

            List<KeyValuePair<int, string>> response = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "plan.planTypeOptions.noPlanType"),
                new KeyValuePair<int, string>(1, "plan.planTypeOptions.localAuthNewBuild"),
                new KeyValuePair<int, string>(2, "plan.planTypeOptions.localAuthPlannedMaint"),
                new KeyValuePair<int, string>(3, "plan.planTypeOptions.localAuthReactiveMaint"),
                new KeyValuePair<int, string>(4, "plan.planTypeOptions.housingAssnNewBuild"),
                new KeyValuePair<int, string>(5, "plan.planTypeOptions.housingAssnPlannedMaint"),
                new KeyValuePair<int, string>(6, "plan.planTypeOptions.housingAssnReactiveMaint"),
                new KeyValuePair<int, string>(7, "plan.planTypeOptions.commercialNewBuild"),
                new KeyValuePair<int, string>(8, "plan.planTypeOptions.commercialDev"),
                new KeyValuePair<int, string>(9, "plan.planTypeOptions.landlordsNewBuild"),
                new KeyValuePair<int, string>(10, "plan.planTypeOptions.landlordsDev"),
                new KeyValuePair<int, string>(11, "plan.planTypeOptions.landlordsMaint"),
                new KeyValuePair<int, string>(12, "plan.planTypeOptions.rentalNewBuild"),
                new KeyValuePair<int, string>(13, "plan.planTypeOptions.rentalDev"),
                new KeyValuePair<int, string>(14, "plan.planTypeOptions.rentalMaint"),
                new KeyValuePair<int, string>(15, "plan.planTypeOptions.domesticNewBuild"),
                new KeyValuePair<int, string>(16, "plan.planTypeOptions.domesticRepl"),
                new KeyValuePair<int, string>(17, "plan.planTypeOptions.privateNewBuild"),
                new KeyValuePair<int, string>(18, "plan.planTypeOptions.privateRepl")
            };

            logger.LogDebug("PlanController end call GetAllPlanTypes -> return Dictionary of plan types");

            return response;
        }


        #endregion

        #region Methods Private

        private async Task<ActionResult> ValidatePassedData(ModelStateDictionary modelState, PlanModel planModel, ClaimsIdentity userIdentity, int userAiepId, bool isChtp = false)
        {
            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("PlanController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (userIdentity.IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("PlanController end call Post -> return Bad request Undefined user");

                return BadRequest();
            }

            if (userAiepId == 0)
            {
                logger.LogDebug("Undefined Aiep");

                logger.LogDebug("PlanController end call Post -> return Bad request Undefined Aiep");

                return BadRequest();
            }

            var entityNotFound = ErrorCode.EntityNotFound.GetDescription();

            if (!await catalogRepository.CheckIfExistsAsync(planModel.CatalogId))
            {
                logger.LogDebug(entityNotFound);

                logger.LogDebug("PlanController end call Post -> return Bad request Entity not found");

                return BadRequest($"{entityNotFound} Plan.CatalogId = {planModel.CatalogId}");
            }

            if (!isChtp)
            {
                return Ok();
            }

            if (!await projectRepository.CheckIfExistsAsync(planModel.ProjectId))
            {
                logger.LogDebug(entityNotFound);

                logger.LogDebug("PlanController end call Post -> return Bad request Entity not found");

                return BadRequest($"{entityNotFound} Plan.ProjectId = {planModel.ProjectId}");
            }

            
            return Ok();
        }

        #endregion

    }

}

