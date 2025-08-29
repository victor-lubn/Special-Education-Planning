using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
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
using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Koa.Persistence.Abstractions.QueryResult;
using System.Security.Claims;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Service.Search;
using SpecialEducationPlanning
.Api.Service.User;
using Microsoft.Extensions.Options;
using System.Linq;
using SpecialEducationPlanning
.Api.Service.Publish;
using System.Globalization;
using SpecialEducationPlanning
.Api.Model.AutomaticArchiveModel;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Project controller
    /// </summary>
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectRepository repository;
        private readonly ILogger<ProjectController> logger;
        private readonly IObjectMapper mapper;
        private readonly IPublishProjectService projectService;
        private readonly IUserService userService;
        private readonly IAzureSearchService azureSearchService;
        private readonly AzureSearchConfiguration azureSearchConfiguration;
        private readonly IOptions<AutomaticArchiveConfiguration> options;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository"></param>
        public ProjectController(IProjectRepository repository, IUserService userService, IAzureSearchService azureSearchService, IOptions<AzureSearchConfiguration> configuration, ILogger<ProjectController> logger, IObjectMapper mapper, IPublishProjectService projectService, IOptions<AutomaticArchiveConfiguration> options)
        {
            this.userService = userService;
            this.azureSearchConfiguration = configuration.Value;
            this.azureSearchService = azureSearchService;
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
            this.projectService = projectService;
            this.options = options;
        }

        /// <summary>
        ///     Automatically archive projects
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AutomaticArchive")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> AutomaticArchive(int? archiveDays, int? AiepId)
        {
            logger.LogDebug("ProjectController called AutomaticArchive");

            if (archiveDays.IsNull())
            {
                archiveDays = int.Parse(options.Value.Archive, CultureInfo.InvariantCulture);
            }
            var delete = double.Parse(options.Value.Delete, CultureInfo.InvariantCulture);
            await repository.AutomaticArchive((int)archiveDays, AiepId);

            logger.LogDebug("ProjectController end call AutomaticArchive -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Delete a project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("ProjectController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<Project>(id);
            var model = mapper.Map<Project, ProjectModel>(entity);
            if (model == null)
            {
                logger.LogDebug("No region found");

                logger.LogDebug("ProjectController end call Delete -> return Not found");

                return NotFound();
            }
            repository.Remove(id);

            logger.LogDebug("ProjectController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="planState"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangeProjectState/{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> ChangeProjectState(int id, [FromBody] PlanState planState)
        {
            logger.LogDebug("ProjectController called ChangeProjectState");

            var projectEntity = await repository.FindOneAsync<Project>(id);
            var projectModel = mapper.Map<Project, ProjectModel>(projectEntity);
            if (projectModel.IsNull())
            {
                logger.LogDebug("No project found");

                logger.LogDebug("ProjectController end call ChangeProjectState -> return No found");

                return NotFound();
            }

            if (((ClaimsIdentity)User.Identity).IsNull())
            {
                logger.LogDebug("Undefined User");

                logger.LogDebug("ProjectController end call ChangePorjectState -> return Bad request undefined user");

                return new BadRequestObjectResult(ErrorCode.UndefinedUser.GetDescription());
            }

            var repositoryResponse = await repository.ChangeProjectStateAsync(projectModel, planState);

            logger.LogDebug("ProjectController end call ChangeProjectState -> return Project");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("ProjectController called GetAll");

            var repositoryResponse = new RepositoryResponse<IEnumerable<ProjectModel>>();
            var projectEntities = await repository.GetAllAsync<Project>();
            var projectModels = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(projectEntities);
            repositoryResponse.Content = projectModels;

            logger.LogDebug("ProjectController end call GetAll -> return All Projects");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get a specific project by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("ProjectController called Get");

            var repositoryResponse = new RepositoryResponse<ProjectModel>();
            var entity = await repository.FindOneAsync<Project>(id);
            var model = mapper.Map<Project, ProjectModel>(entity);
            repositoryResponse.Content = model;
            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No region found");

                logger.LogDebug("ProjectController end call Get -> return Not found");

                return NotFound();
            }
            logger.LogDebug("ProjectController end call Get -> return Project");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Send to Creatio a specific project by Id with Rom Items
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("SendProjectRomItens/{id}")]
        public async Task<IActionResult> SendProjectRomItems(int id)
        {
            logger.LogDebug("ProjectController called SendProjectRomItems");

            var response = await projectService.SendRomItemsToCreatioAsync(id);


            logger.LogDebug("ProjectController end call SendProjectRomItems -> return Project");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Send to Creatio a specific plan by Id under specified projectId with Rom Items
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("SendPlanRomItens/plan/{planId}/project/{projectId}")]
        public async Task<IActionResult> SendPlanRomItems(int planId, int projectId)
        {
            logger.LogDebug("ProjectController called SendProjectRomItems");

            var response = await projectService.SendRomItemsToCreatioAsync(projectId, planId);


            logger.LogDebug("ProjectController end call SendProjectRomItems -> return Project");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Get projects by using Filters
        /// </summary>
        /// <returns>A project list. This action is not returning archived project</returns>
        [HttpPost]
        [Route("GetProjectsFiltered")]
        [AuthorizeTdpFilter(PermissionType.Project_Management)]
        public async Task<IActionResult> GetProjectsFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("ProjectController called GetProjectsFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));
                logger.LogDebug("ProjectController end call GetProjectsFiltered -> return Bad request");
                return BadRequest(ModelState.GetErrorMessages());
            }

            var currentAiepId = userService.GetUserCurrentAiepId((ClaimsIdentity)this.User.Identity);

            if (azureSearchConfiguration.LefthandSearchEnabled && azureSearchService.GetType() == typeof(AzureSearchService))
            {
                var skipValue = searchModel.Skip ?? 0;
                var takeValue = searchModel.Take ?? 100;

                var idsFilterd = await azureSearchService.GetProjectIdsFilteredAsync(searchModel, currentAiepId);
                var projectIds = idsFilterd.ProjectFilteredIds;
                var projects = await repository.GetProjectsByIdsAsync(projectIds, skipValue, takeValue, currentAiepId, idsFilterd.Sort);
                var pagedQuery = new PagedQueryResult<ProjectModelContractHub>(projects.Content, takeValue, skipValue, idsFilterd.TotalCount);

                logger.LogDebug("ProjectController end call GetProjectsFiltered -> return Query with AzureSearch left hand enabled");

                return this.PagedJsonResult(pagedQuery, true);
            }
            else
            {
                var repositoryResponse = await repository.GetProjectsFiltered(searchModel, currentAiepId);
                if (repositoryResponse.ErrorList.Any())
                {
                    logger.LogDebug("ProjectController end call GetProjectsFiltered -> return Error");

                    return repositoryResponse.GetHttpResponse();
                }

                logger.LogDebug("ProjectController end call GetProjectsFiltered -> return Query with AzureSearch left hand disabled");

                return this.PagedJsonResult(repositoryResponse.Content, true);
            }
        }

        /// <summary>
        ///     Create a new project
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post([FromBody] ProjectModel value)
        {
            logger.LogDebug("ProjectController called Post -> Create project");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("ProjectController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = new RepositoryResponse<ProjectModel>();
            var projectEntity = mapper.Map<ProjectModel, Project>(value);
            
            var projectApply = await repository.ApplyChangesAsync(projectEntity);
            var projectModel = mapper.Map<Project,ProjectModel>(projectApply);
            repositoryResponse.Content = projectModel;

            logger.LogDebug("ProjectController end call Post -> return Created project");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Update a project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectModel value)
        {
            logger.LogDebug("ProjectController called Put -> Update project");

            //TODO Refactor specification
            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));
                return BadRequest(ModelState.GetErrorMessages());
            }

            Project project = await this.repository.FindOneAsync<Project>(id);

            if (project is null)
            {
                logger.LogDebug("No region found");

                logger.LogDebug("ProjectController end call Put -> return Not found");

                return NotFound();
            }

            var repositoryResponse = new RepositoryResponse<ProjectModel>();
            var projectEntity = mapper.Map<ProjectModel, Project>(value, project);
            var projectApply = await repository.ApplyChangesAsync(projectEntity);
            var projectModel = mapper.Map<Project, ProjectModel>(projectApply);
            repositoryResponse.Content = projectModel;

            logger.LogDebug("ProjectController end call Put -> return Updated project");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creata a project template
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateProjectTemplate")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> CreateProjectTemplate([FromBody] PlanModel value)
        {
            logger.LogDebug("ProjectController called CreateProjectTemplate -> Create project template");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("ProjectController end call CreateProjectTemplate -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = await repository.CreateProjectTemplate(value);

            logger.LogDebug("ProjectController end call Post -> return Created project");

            return repositoryResponse.GetHttpResponse();
        }
    }
}
