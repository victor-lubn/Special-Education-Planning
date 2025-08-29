using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications;
using SpecialEducationPlanning
.Domain.Specification.ProjectsSpecifications;
using SpecialEducationPlanning
.Domain;
using Koa.Domain.Search.Filter;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private readonly IObjectMapper mapper;
        private readonly IHouseSpecificationRepository houseSpecsRepo;
        private readonly IVersionRepository versionRepo;
        private readonly ILogger<ProjectRepository> logger;
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly IAzureSearchManagementService azureSearchManagementService;
        private readonly DataContext context;

        public ProjectRepository(ILogger<ProjectRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, IAzureSearchManagementService azureSearchManagementService,
            ISpecificationBuilder specificationBuilder,
            IEntityRepository entityRepository,
            IHouseSpecificationRepository houseSpecsRepo,
            IVersionRepository versionRepo) :
            base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.context = (DataContext)dbContextAccessor.GetCurrentContext();
            this.entityRepositoryKey = entityRepositoryKey;
            this.azureSearchManagementService = azureSearchManagementService;
            this.mapper = mapper;
            this.houseSpecsRepo = houseSpecsRepo;
            this.versionRepo = versionRepo;
            this.logger = logger;
        }

        public async Task<Project> AddProject(Project entity)
        {
            return await base.Add(entity);
        }

        public async Task AutomaticArchive(int archiveDays, int? AiepId)
        {
            logger.LogDebug("ProjectRepository called AutomaticArchive (int)");

            UnitOfWork.BeginTransaction();
            var projects = await repository.Where(new Specification<Project>(x => x.Plans.All(p => p.PlanState == PlanState.Archived)))
                                           .Include(y => y.Plans)
                                           .IgnoreQueryFilters()
                                           .ToListAsync();
            if (AiepId.IsNotNull())
            {
                projects = projects.Where(p => p.AiepId == AiepId).ToList();
            }

            foreach (var project in projects)
            {
                if (await IsProjectChtpAsync(project.Id, logger))
                {
                    project.IsArchived = true;
                }
            }

            logger.LogDebug("ProjectRepository AutomaticArchive (int) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("ProjectRepository end call AutomaticArchive (int)");
        }

        public async Task AutomaticArchive(DateTime dateTime, int archiveDays, int AiepId)
        {
            logger.LogDebug($"ProjectRepository called AutomaticArchive (DateTime = {DateTime.Today}, int archiveDays = {archiveDays}, int AiepId = {AiepId})");
            if (AiepId == 1)
            {
                logger.LogDebug($"Secial debug AutomaticArchive for AiepID = {AiepId}");
            }
            UnitOfWork.BeginTransaction();
            List<Project> projects = await repository.Where(new Specification<Project>(x => x.Plans.All(p => p.PlanState == PlanState.Archived)))
                                                     .Where(x => x.AiepId == AiepId)
                                                     .Include(y => y.Plans)
                                                     .IgnoreQueryFilters()
                                                     .ToListAsync();
            if (AiepId == 1)
            {
                var ids = string.Join(",", projects.Select(x => x.Id));
                logger.LogDebug($"Special debug AutomaticArchive ProjectsIds = {ids}");
            }
            
            if (projects.Count > 0)
            {
                List<Project> projectsToArchive = projects
                    .Where(p => (dateTime - p.UpdatedDate).TotalDays >= archiveDays)
                    .ToList();
                if (AiepId == 1)
                {
                    var ids = string.Join(",", projectsToArchive.Select(x => x.Id));
                    logger.LogDebug($"Special debug AutomaticArchive ProjectsToArchiveIds = {ids}");
                }
                foreach (var project in projectsToArchive)
                {
                    if (await IsProjectChtpAsync(project.Id, logger))
                    {
                        if (AiepId == 1)
                        {
                            logger.LogDebug($"Special debug AutomaticArchive projectId = {project.Id} is archived");
                        }
                        project.IsArchived = true;
                    }
                    else
                    {
                        if (AiepId == 1)
                        {
                            logger.LogDebug($"Special debug AutomaticArchive projectId = {project.Id} is NOT archived");
                        }
                    }
                }
            }
            if (AiepId == 1)
            {
                logger.LogDebug($"Special debug AutomaticArchive ENDS");
            }
            logger.LogDebug("ProjectRepository AutomaticArchive (DateTime, int archiveDays, int AiepId) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("ProjectRepository end call AutomaticArchive (DateTime, int archiveDays, int AiepId)");
        }

        public async Task<RepositoryResponse<ProjectModel>> CopyToAiep(ProjectModel projectModel, int AiepId)
        {
            logger.LogDebug("ProjectRepository called CopyToAiep");

            projectModel.Id = 0;
            projectModel.AiepId = AiepId;

            logger.LogDebug("ProjectRepository end call CopyToAiep -> return Repository response ProjectModel");

            return new RepositoryResponse<ProjectModel>(projectModel);
        }

        public async Task<RepositoryResponse<Project>> GetProjectByProjectCodeAsync(string projectCode)
        {
            logger.LogDebug("ProjectRepository called GetProjectByProjectCode");

            var spec = new Specification<Project>(x => true) as ISpecification<Project>;
            var spec1 = new ProjectByCodeProjectSpecification(projectCode);
            spec = spec.And(spec1);

            var project = await repository.Where(spec)
                                          .Include(p => p.HousingSpecifications)
                                          .ThenInclude(h => h.HousingTypes)
                                          .IgnoreQueryFilters()
                                          .SingleOrDefaultAsync();
            if (project.IsNull())
            {
                var error = ErrorCode.EntityNotFound.GetDescription();

                logger.LogDebug("ProjectRepository end call GetProjectByProjectCode -> return Repository response Errors Entity not found");

                return new RepositoryResponse<Project>(error);
            }
            logger.LogDebug("ProjectRepository end call GetProjectByProjectCode -> return Repository response List of ProjectModel");

            return new RepositoryResponse<Project>(project);
        }

        public async Task<RepositoryResponse<Project>> GetProjectByProjectKeyNameAsync(string keyName)
        {
            logger.LogDebug("ProjectRepository called GetProjectByProjectCode");

            var spec = new Specification<Project>(x => true) as ISpecification<Project>;
            var spec1 = new Specification<Project>(x => x.KeyName.Equals(keyName));
            spec = spec.And(spec1);

            var projects = await repository.Where(spec)
                                          .Include(p => p.HousingSpecifications)
                                          .ThenInclude(h => h.HousingTypes)
                                          .IgnoreQueryFilters()
                                          .ToListAsync();

            if (projects.Count > 1)
            {
                logger.LogDebug("ProjectRepository end call GetProjectByProjectCode -> return Repository response Errors Entity not found");

                return new RepositoryResponse<Project>("Too many project entities - There are multiple projects with the same CRM Project Id");
            }
            logger.LogDebug("ProjectRepository end call GetProjectByProjectCode -> return Repository response List of ProjectModel");

            return new RepositoryResponse<Project>(projects.FirstOrDefault());
        }

        public async Task<RepositoryResponse<IEnumerable<ProjectModel>>> GetProjectsByProjectCode(string projectCode)
        {
            logger.LogDebug("ProjectRepository called GetProjectsByProjectCode");

            var spec = new Specification<Project>(x => true) as ISpecification<Project>;
            var spec1 = new ProjectsByCodeProjectSpecification(projectCode);
            spec = spec.And(spec1);

            var projects = await repository.Where(spec).ToArrayAsync();

            if (projects.Length == 0)
            {
                var errors = new Collection<string>();
                errors.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("ProjectRepository end call GetProjectsByProjectCode -> return Repository response Errors Entity not found");

                return new RepositoryResponse<IEnumerable<ProjectModel>>(errors);
            }
            var result = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(projects);

            logger.LogDebug("ProjectRepository end call GetProjectsByProjectCode -> return Repository response List of ProjectModel");

            return new RepositoryResponse<IEnumerable<ProjectModel>>(result);
        }

        public async Task<RepositoryResponse<Project>> GetPlanRomItemsAsync(int projectId, int planId)
        {
            logger.LogDebug("ProjectRepository call GetProjectRomItems");

            var spec = Specification<Project>.True;
            spec = spec.And(new ProjectByIdSpecification(projectId));

            var houseSpecs = await houseSpecsRepo.GetHouseSpecsByProjectAndPlanIdAsync(projectId, planId);

            var projectHousing = await GetProjectAsync(spec);

            var projectPlan = await GetProjectFilteredPlanAsync(spec, planId);

            var masterPlans = GetPlansMasterVersion(projectPlan.Plans);

            var projectRomItems = GetProjectRomItems(houseSpecs, masterPlans);

            projectHousing.HousingSpecifications = projectRomItems;

            return new RepositoryResponse<Project>(projectHousing);
        }

        public async Task<RepositoryResponse<Project>> GetProjectRomItemsAsync(int projectId)
        {
            logger.LogDebug("ProjectRepository call GetProjectRomItems");

            var spec = Specification<Project>.True;
            spec = spec.And(new ProjectByIdSpecification(projectId));

            var houseSpecs = await houseSpecsRepo.GetHouseSpecsByProjectIdAsync(projectId);

            var projectHousing = await GetProjectHousingAsync(spec);

            var projectPlan = await GetProjectPlanAsync(spec);

            var masterPlans = GetPlansMasterVersion(projectPlan.Plans);

            var projectRomItems = GetProjectRomItems(houseSpecs, masterPlans);

            projectHousing.HousingSpecifications = projectRomItems;

            return new RepositoryResponse<Project>(projectHousing);
        }

        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> GetProjectPlans(int projectId)
        {
            logger.LogDebug("ProjectRepository call GetProjectPlans");

            var spec = Specification<Project>.True;
            spec = spec.And(new ProjectByIdSpecification(projectId));
            var project = await repository.Where(spec).Include(x => x.Plans).FirstOrDefaultAsync();

            if (project.IsNull())
            {
                var errors = new Collection<string>();
                errors.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("ProjectRepository end call GetProjectPlans -> return Repository response Errors Entity not found");

                return new RepositoryResponse<IEnumerable<PlanModel>>(errors);
            }
            var result = mapper.Map<Plan, PlanModel>(project.Plans);

            logger.LogDebug("ProjectRepository end call GetProjectPlans -> return Repository response List of PlanModel");

            return new RepositoryResponse<IEnumerable<PlanModel>>(result);
        }

        public async Task<RepositoryResponse<ProjectModel>> ChangeProjectStateAsync(ProjectModel projectModel, PlanState planState)
        {
            logger.LogDebug("ProjectRepository called ChangeProjectState");

            ISpecification<Project> spec = new Specification<Project>(p => p.Id == projectModel.Id);

            UnitOfWork.BeginTransaction();

            var entity = await repository.Where(spec)
                .Include(pr => pr.Plans.Where(pl => pl.PlanState == PlanState.Active)) //only active plans are archived
                .FirstOrDefaultAsync();

            entity.IsArchived = (planState == PlanState.Archived); //Archive or Restore the given project

            foreach (var plan in entity.Plans) //archived plans are not restored
            {
                plan.PlanState = planState;
            }

            logger.LogDebug("ProjectRepository ChangeProjectState call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("ProjectRepository end call ChangeProjectState -> return Repository response ProjectModel");

            return new RepositoryResponse<ProjectModel>(mapper.Map<Project, ProjectModel>(entity));
        }

        public async Task<RepositoryResponse<IEnumerable<ProjectModelContractHub>>> GetProjectsByIdsAsync(IEnumerable<int> ids, int skip, int take, int AiepId, SortDescriptor azureSort)
        {
            logger.LogDebug("ProjectRepository called GetProjectsByIdsAsync");

            ISpecification<Project> spec = new Specification<Project>(b => ids.Contains(b.Id));

            if (AiepId != -1)
            {
                spec = spec.And(new Specification<Project>(b => b.AiepId == AiepId));
            }

            List<Project> projects;

            var projectQueryable = repository
                .Where(spec)
                .Include(p => p.Aiep)
                .Include(p => p.Builder)
                .Include(p => p.ProjectTemplates)
                .Include(p => p.HousingSpecifications)
                .ThenInclude(h => h.HousingTypes)
                .Include(p => p.HousingSpecifications)
                .ThenInclude(h => h.HousingSpecificationTemplates)
                .ThenInclude(t => t.Plan)
                .Take(take);

            if (azureSort.IsNull())
            {
                projects = await projectQueryable.OrderByDescending(b => b.UpdatedDate).ToListAsync();
            }
            else
            {
                MethodInfo order = System.Linq.QueryableExtensions.OrderByMethod;

                if (azureSort.Direction == SortDirection.Descending)
                {
                    order = System.Linq.QueryableExtensions.OrderByDescendingMethod;
                }

                projects = await projectQueryable.OrderByPropertyName(order, azureSort.Member).ToListAsync();
            }
            var projectModels = mapper.Map<IList<Project>, IList<ProjectModelContractHub>>(projects, new List<ProjectModelContractHub>());

            logger.LogDebug("ProjectRepository end call GetProjectsByIdsAsync -> return Repository response List of Project model");

            return new RepositoryResponse<IEnumerable<ProjectModelContractHub>>(projectModels);
        }

        public async Task<RepositoryResponse<IEnumerable<ProjectModelContractHub>>> GetProjectsByIdsAndTypeAsync(Dictionary<Type, HashSet<int>> entityTypesAndIds, int skip, int take, int AiepId, SortDescriptor azureSort, ICollection<FilterDescriptor> searchFilters)
        {
            logger.LogDebug("ProjectRepository called GetProjectsByIdsAndTypeAsync");

            var projectIds = new HashSet<int>();

            var builderIds = entityTypesAndIds[typeof(Builder)];
            if (builderIds.Any())
            {
                projectIds.AddRange(repository.GetAll<Project>()
                               .Where(p => p.BuilderId != null && builderIds.Contains(p.BuilderId.Value))
                               .Take(take)
                               .Select(p => p.Id).ToList());
            }

            projectIds.AddRange(entityTypesAndIds[typeof(Project)].Take(take));

            logger.LogDebug("ProjectRepository end call GetProjectsByIdsAndTypeAsync -> return Call GetProjectsByIdsAsync");

            return await GetProjectsByIdsAsync(projectIds, skip, take, AiepId, azureSort);
        }

        public async Task<RepositoryResponse<IPagedQueryResult<ProjectModel>>> GetProjectsFiltered(IPageDescriptor searchModel, int? currentAiepId)
        {
            logger.LogDebug("ProjectRepository called GetProjectsFiltered");

            var spec = Specification<Project>.True;
            if (currentAiepId.HasValue && currentAiepId.Value != -1)
            {
                spec = spec.And(new ProjectByAiepIdSpecification(currentAiepId.Value));
            }

            spec = spec.And(new ProjectChtpSpecification());

            if (searchModel.Filters.Any(f => f.Member.Contains("builder.")))
            {
                spec = spec.And(new Specification<Project>(p => p.Builder != null && p.BuilderId != 0));
            }

            ProjectModel projectModel = new ProjectModel();

            var filters = AddFilters(searchModel.Filters);
            var modelSpec = SpecificationBuilder.Create<ProjectModel>(filters);

            var query = new ProjectMaterializedProjectModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = await repository.QueryAsync(query);

            if (result.Total > 1000)
            {
                logger.LogDebug("ProjectRepository end call GetProjectsFiltered -> return Repository response Paged query Project model Error Max take exceeded");

                return new RepositoryResponse<IPagedQueryResult<ProjectModel>>(result, ErrorCode.MaxTakeExceeded, "The maximum number of projects has been exceeded");
            }

            var repositoryResponse = new RepositoryResponse<IPagedQueryResult<ProjectModel>>
            {
                Content = result
            };

            logger.LogDebug("BuilderRepository end call GetBuildersFiltered -> return Repository response Paged query Builder model");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<ProjectModel>> CreateProjectForPlan(PlanModel value, int AiepId)
        {
            logger.LogDebug("ProjectRepository called CreateProjectForPlan");

            if (AiepId == 0)
            {
                logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response Errors UndefinedAiep");

                return new RepositoryResponse<ProjectModel>(null, ErrorCode.UndefinedAiep);
            }

            if (!repository.Any(new EntityByIdSpecification<Aiep>(AiepId)))
            {
                logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response Errors Argument error business");

                return new RepositoryResponse<ProjectModel>(null, ErrorCode.ArgumentErrorBusiness);
            }

            var newProject = new ProjectModel
            {
                AiepId = AiepId,
                CodeProject = "Project for plan " + value.PlanCode,
                SinglePlanProject = true
            };

            Project projectResponse = mapper.Map<ProjectModel, Project>(newProject);
            Project projectApplyChanges = await this.Add(projectResponse);
            ProjectModel projectModel = mapper.Map<Project, ProjectModel>(projectApplyChanges);
            logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response ProjectModel");

            return new RepositoryResponse<ProjectModel>(projectModel);
        }

        public async Task<RepositoryResponse<ProjectTemplatesModel>> CreateProjectTemplate(PlanModel model)
        {
            logger.LogDebug("ProjectRepository called CreateProjectTemplate");

            UnitOfWork.BeginTransaction();

            var planEntity = await this.CreatePlanForTemplate(model);

            var projectTemplate = entityRepositoryKey.CreateProjectTemplate(model.ProjectId, planEntity.Id, logger);

            if (model.HousingSpecificationId.HasValue)
            {
                var housingSpecificationTemplate = entityRepositoryKey.CreateHousingSpecificationTemplate(model.HousingSpecificationId.Value, planEntity.Id, logger);
            }

            UnitOfWork.Commit();

            ProjectTemplatesModel projectTemplateModel = mapper.Map<ProjectTemplates, ProjectTemplatesModel>(projectTemplate);

            logger.LogDebug("ProjectRepository end call CreateProjectTemplate -> return Repository response ProjectTemplatesModel");

            return new RepositoryResponse<ProjectTemplatesModel>(projectTemplateModel);
        }

        public async Task<bool> IsProjectChtpAsync(int projectId, ILogger logger)
        {
            logger.LogDebug("ProjectRepository called IsProjectChtpAsync");
            logger.LogDebug($"Special debug IsProjectChtpAsync for projectId = {projectId}");

            var spec = new Specification<Project>(p => p.Id == projectId);
            var isProject = await AnyAsync(spec);
            var hasHousingSpec = await entityRepositoryKey.HasProjectHousingSpecificationAsync(projectId, logger);

            logger.LogDebug($"Special debug IsProjectChtpAsync for projectId = {projectId} => Count(projectId {projectId}) = {isProject}");
            logger.LogDebug($"Special debug IsProjectChtpAsync for projectId = {projectId} => HasProjectHousingSpec {projectId}) = {hasHousingSpec}");
            if (isProject && hasHousingSpec)
            {
                logger.LogDebug($"Special debug IsProjectChtpAsync for projectId = {projectId} return TRUE");
                return true;
            }
            logger.LogDebug($"Special debug IsProjectChtpAsync for projectId = {projectId} return FALSE");
            logger.LogDebug("ProjectRepository end call IsProjectChtpAsync -> return booleam");

            return false;
        }

        public async Task CallIndexerAsync(int take, int skip, DateTime? updatedDate, int? indexerWindowInDays)
        {
            logger.LogDebug("ProjectRepository called CallIndexerAsync");

            var projects = await entityRepositoryKey.GetEntitiesNoFiltersAsync<Project>(take, skip, updatedDate, indexerWindowInDays).ToListAsync();

            azureSearchManagementService.MergeOrUploadDocuments
                (azureSearchManagementService.GetDocuments<OmniSearchProjectIndexModel, Project>(projects));

            logger.LogDebug("ProjectRepository end call CallIndexerAsync");
        }

        #region Methods Private

        private ICollection<FilterDescriptor> AddFilters(ICollection<FilterDescriptor> filters)
        {
            var dateFilters = new List<FilterDescriptor>();

            var otherFilters = new List<string> { "projectName", "projectReference", "builderName", "createdDate", "updatedDate" };
            var isOtherFilters = filters.Where(f => otherFilters.Contains(f.Member)).Any();
            var removeArchivedFilter = false;

            foreach (var filter in filters)
            {
                if (filter.Member.Contains("updatedDate") || filter.Member.Contains("createdDate"))
                {
                    filter.Operator = FilterOperator.IsGreaterThanOrEqualTo;
                    dateFilters.Add(new FilterDescriptor
                    {
                        Member = filter.Member,
                        Operator = FilterOperator.IsLessThanOrEqualTo,
                        Value = DateTime.Parse(filter.Value).AddDays(1).ToString()
                    });
                }

                if (filter.Member.Contains("projectName"))
                {
                    filter.Member = "codeProject";
                }

                if (filter.Member.Contains("projectReference"))
                {
                    filter.Member = "keyName";
                }

                if (filter.Member.Contains("isArchived"))
                {
                    if ((isOtherFilters && filter.Value == "False") || !isOtherFilters)
                    {
                        removeArchivedFilter = false;
                    }
                    else
                    {
                        removeArchivedFilter = true;
                    }
                }
            }

            filters.AddRange(dateFilters);

            if (removeArchivedFilter)
            {
                var archivedFilter = filters.FirstOrDefault(f => f.Member.Contains("isArchived"));
                if (archivedFilter != null)
                {
                    filters.Remove(archivedFilter);
                }
            }

            return filters;
        }

        private async Task<Project> GetProjectPlanAsync(ISpecification<Project> spec)
        {
            var projectPlan = await repository.Where(spec)
                .Include(x => x.Plans)
                .AsNoTracking()
                .SingleAsync();

            return projectPlan;
        }

        private async Task<Project> GetProjectFilteredPlanAsync(ISpecification<Project> spec, int planId)
        {
            var projectFilteredPlan = await repository.Where(spec)
                .Include(x => x.Plans.Where(plan => plan.Id == planId))
                .AsNoTracking()
                .SingleAsync();

            return projectFilteredPlan;
        }

        private async Task<Project> GetProjectHousingAsync(ISpecification<Project> spec)
        {
            var projectHousing = await repository.Where(spec)
                .Include(x => x.HousingSpecifications)
                .ThenInclude(x => x.HousingTypes)
                .AsNoTracking()
                .SingleAsync();

            return projectHousing;
        }

        private async Task<Project> GetProjectAsync(ISpecification<Project> spec)
        {
            var projectHousing = await repository.Where(spec)
                .AsNoTracking()
                .SingleAsync();

            return projectHousing;
        }

        private List<Plan> GetPlansMasterVersion(ICollection<Plan> plans)
        {
            var masterPlans = plans.Select(x =>
            {
                var masterVersionId = x.MasterVersionId ?? 0;

                var masterVersion = versionRepo.GetVersionById(masterVersionId).GetAwaiter().GetResult();

                var masterVersions = new List<Version>() { masterVersion };

                x.Versions = masterVersions;

                return x;
            }).ToList();

            return masterPlans;
        }

        private List<HousingSpecification> GetProjectRomItems(ICollection<HousingSpecification> housingSpecifications, List<Plan> masterPlans)
        {
            var projectRomItems = housingSpecifications.Select(x =>
            {
                var addedPlan = x.HousingTypes.Select(y =>
                {
                    var planIds = y.Plans.Select(pl => pl.Id).ToList();
                    var plans = masterPlans.Where(master => planIds.Contains(master.Id));
                    y.Plans = plans.ToList();

                    return y;
                }).ToList();
                x.HousingTypes = addedPlan;

                return x;
            }).ToList();

            return projectRomItems;
        }

        private async Task<Plan> CreatePlanForTemplate(PlanModel model)
        {
            var planEntity = mapper.Map<PlanModel, Plan>(model);
            planEntity.IsTemplate = true;
            planEntity = repository.Add(planEntity);

            await context.SaveChangesAsync();

            return planEntity;
        }

        #endregion


    }
}
