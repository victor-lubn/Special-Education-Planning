using Koa.Domain.Search.Filter;
using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.PlanDetails;
using SpecialEducationPlanning
.Business.Model.Project;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IPlanRepository :IBaseRepository<Plan>
    {
        Task<RepositoryResponse<string>> GeneratePlanIdAsync(DateTime? date = null);
        Task<RepositoryResponse<PlanModel>> GetPlanAsync(int planId);
        Task<RepositoryResponse<IEnumerable<PlanModel>>> GetPlansByIdsAndTypeAsync(Dictionary<Type, HashSet<int>> entityTypesAndIds, int skip, int take, int AiepId, SortDescriptor azureSort, ICollection<FilterDescriptor> searchFilters);
        Task<RepositoryResponse<IPagedQueryResult<PlanModel>>> GetPlansFilteredAsync(IPageDescriptor searchModel, int? currentAiepId);
        Task<RepositoryResponse<Tuple<ICollection<PlanModel>, int>>> GetPlansOmniSearchAsync(string textToSearch, int take, int? currentAiepId);
        Task<RepositoryResponse<ICollection<VersionModel>>> GetPlanVersionsAsync(int planId);
        Task<RepositoryResponse<PlanModel>> ChangePlanStateAsync(PlanModel planModel, PlanState planState);
        Task<RepositoryResponse<PlanModel>> CreateOrUpdateAsync(PlanModel planModel);
        Task<RepositoryResponse<IEnumerable<PlanModel>>> GetAllPlansForProjectAsync(int projectId);
        Task<RepositoryResponse<IEnumerable<PlanModel>>> GetAllPlansWithoutArchivedPlansAsync();
        Task<RepositoryResponse<IEnumerable<PlanModel>>> GetAllPlansWithArchivedPlansAsync();
        Task<RepositoryResponse<IPagedQueryResult<PlanModel>>> GetAllArchivedPlansAsync(IPageDescriptor searchModel);
        Task<RepositoryResponse<PlanModel>> FindOnePlanWhithoutArchivedPlansAsync(int planId);
        Task<RepositoryResponse<PlanModel>> AssignBuilderToPlan(int planId, BuilderModel builderModel);
        Task AutomaticArchive(int archiveDays);
        Task AutomaticNonTenderPackArchiveAsync(DateTime dateTime, int archiveDays, int AiepId);
        Task AutomaticTenderPackArchiveAsync(DateTime dateTime, int archiveDays, int AiepId);
        Task AutomaticDeletion(double delete);
        Task AutomaticDeletion(double delete, DateTime dateTime, int AiepId);
        Task<PlanModel> AssignBuilder(int planId, int builderId);
        Task<RepositoryResponse<PlanModel>> UnassignBuilderFromPlan(int planId);
        Task AssignPlanToAiepAsync(int planId, int AiepId);
        Task<RepositoryResponse<PlanModel>> TransferSinglePlanBetweenAieps(int planId, int AiepId);
        Task<RepositoryResponse<IEnumerable<PlanModel>>> TransferMultiplePlanToUnassignedBuilder(int builderId, int AiepId);
        Task<RepositoryResponse<PlanModel>> CopyToProject(PlanModel planModel, int projectId);
        Task<bool> DeleteEmptyPlans();
        Task<RepositoryResponse<PlanModel>> GetPlanWithVersions(int planId);
        Task<bool> UpdateBuilderPlansTradingName(int builderId, string builderTradingName);


        Task<RepositoryResponse<PlanModel>> CreateOrUpdateAsync(PlanModel planModel, int AiepId);
        Task<RepositoryResponse<PlanModel>> ApplyChangesPlanAsync(PlanModel model, int AiepId);
        Task<PlanModel2> FindOneWithEducationerAsync(int planId);
        Task<RepositoryResponseGeneric> ChangePlanStateAsync(int planId, PlanState planState, bool includeArchived);
        Task<RepositoryResponse<PlanModel>> ApplyChangesPlanAsync(PlanModel model);
        Task<RepositoryResponse<IEnumerable<PlanModel>>> GetPlansByIdsAsync(IEnumerable<int> ids, int AiepId, int take, SortDescriptor azureSort, ICollection<FilterDescriptor> searchFilters);
        Task CallIndexerAsync(int take, int skip, DateTime? updatedDate, int? indexerWindowInDays);
        Task<RepositoryResponse<PlanModel>> CreatePlan(PlanModel planModel, int AiepId, int userId);
        Task<RepositoryResponse<PlanModel>> CreateTenderPackPlan(PlanModel planModel, int AiepId, int userId);
        Task<RepositoryResponse<IPagedQueryResult<PlanModel>>> GetPlansSortedAsync(SortDescriptor sortModel, int builderId, int? AiepId = null, bool GetOnlyArchivedPlans = false);
        Task<RepositoryResponse<bool>> TransferMultipleProjectPlansToAiep(int projectId, string AiepCode);
        Task<Plan> GetPlanWithHousingTypeHousingSpecs(int planId);
        Task<bool> IsPlanChtpAsync(int planId, ILogger logger);
        Task<RepositoryResponse<PlanModel>> UpdateTenderPackPlanAsync(TenderPackUpdatePlanModel updatePlanModel);
        Task<RepositoryResponse<bool>> IsPlanNameDuplicateAsync(string planName);
        Task<RepositoryResponse<PlanDetailsResponseModel>> GetPlanDetailsAsync(PlanDetailsRequestModel planDetailsRequest);
    }
}

