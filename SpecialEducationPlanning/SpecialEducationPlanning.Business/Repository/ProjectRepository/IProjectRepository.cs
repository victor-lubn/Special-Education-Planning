using Koa.Domain.Search.Filter;
using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Persistence.Abstractions.QueryResult;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IProjectRepository : IBaseRepository<Project> {
        Task<RepositoryResponse<IEnumerable<ProjectModelContractHub>>> GetProjectsByIdsAndTypeAsync(Dictionary<Type, HashSet<int>> entityTypesAndIds, int skip, int take, int AiepId, SortDescriptor azureSort, ICollection<FilterDescriptor> searchFilters);
        Task<RepositoryResponse<IEnumerable<ProjectModel>>> GetProjectsByProjectCode(string projectCode);
        Task<RepositoryResponse<Project>> GetProjectByProjectCodeAsync(string projectCode);
        Task<RepositoryResponse<IEnumerable<PlanModel>>> GetProjectPlans(int projectId);
        Task<RepositoryResponse<Project>> GetProjectRomItemsAsync(int projectId);
        Task<RepositoryResponse<Project>> GetPlanRomItemsAsync(int projectId, int planId);
        Task<RepositoryResponse<ProjectModel>> ChangeProjectStateAsync(ProjectModel projectModel, PlanState planState);
        Task<RepositoryResponse<IEnumerable<ProjectModelContractHub>>> GetProjectsByIdsAsync(IEnumerable<int> ids, int skip, int take, int AiepId, SortDescriptor azureSort);
        Task<RepositoryResponse<IPagedQueryResult<ProjectModel>>> GetProjectsFiltered(IPageDescriptor model, int? currentAiepId);
        Task<RepositoryResponse<ProjectModel>> CopyToAiep(ProjectModel projectModel, int AiepId);
        Task<RepositoryResponse<ProjectModel>> CreateProjectForPlan(PlanModel value, int AiepId);
        Task<Project> AddProject(Project entity);
        Task AutomaticArchive(int archiveDays, int? AiepId);
        Task AutomaticArchive(DateTime dateTime, int archiveDays, int AiepId);
        Task<RepositoryResponse<ProjectTemplatesModel>> CreateProjectTemplate(PlanModel value);
        Task<bool> IsProjectChtpAsync(int projectId, ILogger logger);
        Task CallIndexerAsync(int take, int skip, DateTime? updatedDate, int? indexerWindowInDays);
        Task<RepositoryResponse<Project>> GetProjectByProjectKeyNameAsync(string keyName);
    }
}
