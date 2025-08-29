using Koa.Domain.Search.Filter;
using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.PlanDetails;
using SpecialEducationPlanning
.Business.Model.Project;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.PlanSpecifications;
using SpecialEducationPlanning
.Domain.Specification.VersionSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class PlanRepository : BaseRepository<Plan>, IPlanRepository
    {
        private readonly IObjectMapper mapper;
        private readonly DataContext context;

        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly IEntityRepository entityRepository;
        private readonly IAiepRepository AiepRepo;
        private readonly ILogger<PlanRepository> logger;
        private readonly IAzureSearchManagementService azureSearchManagementService;
        private readonly IPostCodeServiceFactory postCodeServiceFactory;
        private readonly IDbExecutionStrategy executionStrategy;

        public PlanRepository(ILogger<PlanRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, IAzureSearchManagementService azureSearchManagementService,
            IPostCodeServiceFactory postCodeServiceFactory, 
            ISpecificationBuilder specificationBuilder, 
            IDbExecutionStrategy executionStrategy, 
            IEntityRepository entityRepository,
            IAiepRepository AiepRepo) :
                base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.entityRepository = entityRepository;
            this.AiepRepo = AiepRepo;
            this.mapper = mapper;
            this.logger = logger;
            this.azureSearchManagementService = azureSearchManagementService;
            this.context = (DataContext)dbContextAccessor.GetCurrentContext();
            this.postCodeServiceFactory = postCodeServiceFactory;
            this.executionStrategy = executionStrategy;
        }

        public async Task<RepositoryResponse<string>> GeneratePlanIdAsync(DateTime? date = null)
        {
            logger.LogDebug("PlanRepository called GeneratePlanIdAsync");

            var d = date.IsNotNull() ? date : DateTime.UtcNow;

            logger.LogDebug("PlanRepository end call GeneratePlanIdAsync -> return Call entityRepository.GeneratePlanIdAsync");

            return await entityRepositoryKey.GeneratePlanIdAsync(d.Value, context, logger);
        }

        public async Task<RepositoryResponse<PlanModel>> GetPlanAsync(int planId)
        {
            logger.LogDebug($"{nameof(PlanRepository)} called {nameof(GetPlanAsync)}");

            var spec = new Specification<Plan>(p => p.Id == planId);
            var plan = await repository.Where(spec)
                .Include(p => p.EducationToolOrigin)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync();

            var planModel = mapper.Map<Plan, PlanModel>(plan);

            return new RepositoryResponse<PlanModel>(planModel);
        }

        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> GetPlansByIdsAndTypeAsync(Dictionary<Type, HashSet<int>> entityTypesAndIds, int skip, int take, int AiepId, SortDescriptor azureSort, ICollection<FilterDescriptor> searchFilters)
        {
            logger.LogDebug("PlanRepository called GetPlansByIdsAndTypeAsync");

            var planIds = new HashSet<int>();

            if (entityTypesAndIds.Keys.Any(x => x != typeof(Plan) && x != typeof(Builder)))
            {
                var versionIds = entityTypesAndIds[typeof(Domain.Entity.Version)];
                if (versionIds.Any())
                {
                    planIds.AddRange(repository.GetAll<Domain.Entity.Version>()
                                   .Where(v => versionIds.Contains(v.Id))
                                   .Take(take)
                                   .Select(v => v.PlanId).ToList());
                }

                var endUserIds = entityTypesAndIds[typeof(EndUser)];
                if (entityTypesAndIds[typeof(EndUser)].Any())
                {
                    planIds.AddRange(repository.GetAll<Plan>()
                                   .Where(p => p.EndUserId != null && endUserIds.Contains(p.EndUserId.Value))
                                   .Take(take)
                                   .Select(p => p.Id).ToList());
                }

                var userIds = entityTypesAndIds[typeof(User)];
                if (userIds.Any())
                {
                    planIds.AddRange(repository.GetAll<Plan>()
                                   .Where(p => p.EducationerId != null && userIds.Contains(p.EducationerId.Value))
                                   .Take(take)
                                   .Select(p => p.Id).ToList());
                }
            }

            var builderIds = entityTypesAndIds[typeof(Builder)];
            if (builderIds.Any())
            {
                planIds.AddRange(repository.GetAll<Plan>()
                               .Where(p => p.BuilderId != null && builderIds.Contains(p.BuilderId.Value))
                               .Take(take)
                               .Select(p => p.Id).ToList());
            }

            planIds.AddRange(entityTypesAndIds[typeof(Plan)].Take(take));

            logger.LogDebug("PlanRepository end call GetPlansByIdsAndTypeAsync -> return Call GetPlansByIdsAsync");

            return await GetPlansByIdsAsync(planIds, AiepId, take, azureSort, searchFilters);
        }

        public async Task<RepositoryResponse<ICollection<VersionModel>>> GetPlanVersionsAsync(int planId)
        {
            logger.LogDebug("PlanRepository called GetPlanVersionsAsync");

            var plan = await repository.Where(new EntityByIdSpecification<Plan>(planId)).Include(x => x.Versions)
                .FirstOrDefaultAsync();

            logger.LogDebug("PlanRepository end call GetPlanVersionsAsync -> return Repository response Collection of VersionModel");

            return new RepositoryResponse<ICollection<VersionModel>>(this.mapper.Map<ICollection<Domain.Entity.Version>, ICollection<VersionModel>>(plan?.Versions));
        }

        public async Task<RepositoryResponse<IPagedQueryResult<PlanModel>>> GetPlansFilteredAsync(IPageDescriptor searchModel, int? AiepId = null)
        {
            logger.LogDebug("PlanRepository called GetPlansFilteredAsync");

            ISpecification<Plan> spec;

            if (searchModel.Filters.Any(f => f.Member.Contains("planState")))
            {
                spec = new Specification<Plan>(p => p.PlanState == PlanState.Archived);
            }
            else
            {
                spec = new Specification<Plan>(p => p.PlanState == PlanState.Active);
            }

            if (AiepId.HasValue && AiepId.Value != -1)
            {
                spec = spec.And(new Specification<Plan>(p => p.Project.AiepId == AiepId.Value));
            }

            if (searchModel.Filters.Any(f => f.Member.Contains("endUser.")))
            {
                spec = spec.And(new Specification<Plan>(p => p.EndUserId != null && p.EndUserId != 0));
            }

            if (searchModel.Filters.Any(f => f.Member.Contains(".externalCode")))
            {
                var filter = searchModel.Filters.FirstOrDefault(f => f.Member.Contains(".externalCode"));
                spec = spec.And(new Specification<Plan>(p => p.Versions.Any(v => v.ExternalCode == filter.Value)));
            }

            if (searchModel.Filters.Any(f => f.Member.Contains(".quoteOrderNumber")))
            {
                var filter = searchModel.Filters.FirstOrDefault(f => f.Member.Contains(".quoteOrderNumber"));
                spec = spec.And(new Specification<Plan>(p => p.Versions.Any(v => v.QuoteOrderNumber.Contains(filter.Value))));
            }

            if (searchModel.Take <= 0) searchModel.Take = 1;

            var modelSpec = SpecificationBuilder.Create<PlanModel>(searchModel.Filters);

            var query = new PlanMaterializedPlanModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = await repository.QueryAsync(query);



            if (result.Total > 1000)
            {
                logger.LogDebug("PlanRepository end call GetPlansFilteredAsync -> return Paged query PlanModel Errors Max take exceeded");

                return new RepositoryResponse<IPagedQueryResult<PlanModel>>(result, ErrorCode.MaxTakeExceeded, "The maximum number of plans has been exceeded");
            }

            logger.LogDebug("PlanRepository end call GetPlansFilteredAsync -> return Paged query PlanModel");

            return new RepositoryResponse<IPagedQueryResult<PlanModel>>(result);
        }


        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> GetPlansByIdsAsync(IEnumerable<int> ids, int AiepId, int take, SortDescriptor azureSort, ICollection<FilterDescriptor> searchFilters)
        {
            logger.LogDebug("PlanRepository called GetPlansByIdsAsync");

            ISpecification<Plan> spec;
            if (searchFilters.IsNotNull() && searchFilters.Any(f => f.Member.Contains("planState")))
            {
                spec = new Specification<Plan>(p => p.PlanState == PlanState.Archived);
            }
            else
            {
                spec = new Specification<Plan>(p => p.PlanState == PlanState.Active);
            }

            spec = spec.And(new Specification<Plan>(p => ids.Contains(p.Id)));

            if (AiepId != -1)
            {
                spec = spec.And(new Specification<Plan>(p => p.Project.AiepId == AiepId));
            }

            List<Plan> plans;

            var planQueryable = repository.Where(spec)
                .Include(p => p.EndUser)
                .Include(p => p.Educationer)
                .Include(p => p.Project)
                .Include(p => p.EducationToolOrigin)
                .Take(take);

            if (azureSort.IsNull())
            {
                plans = await planQueryable.OrderByDescending(p => p.UpdatedDate).ToListAsync();
            }
            else
            {
                if (azureSort.Member == "EndUserSurname")
                {
                    if (azureSort.Direction == SortDirection.Descending)
                    {
                        plans = await planQueryable.OrderByDescending(b => b.EndUser.Surname).ToListAsync();
                    }
                    else
                    {
                        plans = await planQueryable.OrderBy(b => b.EndUser.Surname).ToListAsync();
                    }
                }
                else if (azureSort.Member == "EducationerSurname")
                {
                    if (azureSort.Direction == SortDirection.Descending)
                    {
                        plans = await planQueryable.OrderByDescending(b => b.Educationer.Surname).ToListAsync();
                    }
                    else
                    {
                        plans = await planQueryable.OrderBy(b => b.Educationer.Surname).ToListAsync();
                    }
                }
                else
                {
                    MethodInfo order = System.Linq.QueryableExtensions.OrderByMethod;

                    if (azureSort.Direction == SortDirection.Descending)
                    {
                        order = System.Linq.QueryableExtensions.OrderByDescendingMethod;
                    }

                    plans = await planQueryable.OrderByPropertyName(order, azureSort.Member).ToListAsync();
                }
            }

            var plansModels = mapper.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(plans, new List<PlanModel>());

            for (int i = 0; i < plansModels.Count(); i++)
            {
                plansModels.ElementAt(i).Educationer = mapper.Map<User, UserModel>(plans.ElementAt(i).Educationer);
            }

            logger.LogDebug("PlanRepository end call GetPlansByIdsAsync -> return Repository response List of PlanModel");

            return new RepositoryResponse<IEnumerable<PlanModel>>(plansModels);
        }

        public async Task<RepositoryResponse<Tuple<ICollection<PlanModel>, int>>> GetPlansOmniSearchAsync(string textToSearch, int take, int? AiepId = null)
        {
            logger.LogDebug("PlanRepository called GetPlansOmniSearchAsync");

            int countPlans = 0;
            ICollection<Plan> planList;
            // Get Active Plans
            ISpecification<Plan> baseSpec = new ActivePlansSpecification();
            if (AiepId.HasValue && AiepId.Value != -1)
            {
                baseSpec = baseSpec.And(new Specification<Plan>(p => p.Project.AiepId == AiepId.Value));
            }

            if (string.IsNullOrEmpty(textToSearch))
            {
                planList = await repository.Where(baseSpec).Include(x => x.EndUser).Include(x => x.Educationer).OrderByDescending(x => x.UpdatedDate).Take(take).ToListAsync();
                countPlans = planList.Count;
            }
            else
            {
                var search = textToSearch;

                ISpecification<Plan> searchSpec = new Specification<Plan>(x => x.Title.Contains(search));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.PlanCode.Contains(search)));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.KeyName.Contains(search)));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.BuilderTradingName.Contains(search)));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.CadFilePlanId.Contains(search)));

                // EndUser fields
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.EndUser.FullName.Contains(search)));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.EndUser.Address0.Contains(search)));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.EndUser.MobileNumber.Contains(search)));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.EndUser.Postcode.Contains(search)));

                //Versions fields
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.Versions.Any(v => v.ExternalCode.Contains(search))));
                searchSpec = searchSpec.Or(new Specification<Plan>(x => x.Versions.Any(v => v.QuoteOrderNumber.Contains(search))));

                baseSpec = baseSpec.And(searchSpec);

                planList = await repository.Where(baseSpec).Include(x => x.EndUser).Include(x => x.Versions).Include(x => x.Educationer)
                    .OrderByDescending(x => x.UpdatedDate).Take(take).ToListAsync();
                countPlans = planList.Count;
            }

            var planModelList = mapper.Map<ICollection<Plan>, ICollection<PlanModel>>(planList);

            for (int i = 0; i < planModelList.Count; i++)
            {
                planModelList.ElementAt(i).Educationer = mapper.Map<User, UserModel>(planList.ElementAt(i).Educationer);
            }

            var planModelListCountTuple = new Tuple<ICollection<PlanModel>, int>(planModelList, countPlans);

            logger.LogDebug("PlanRepository end call GetPlansOmniSearchAsync -> return Tuple<Collection of PlanModel, int>");

            return new RepositoryResponse<Tuple<ICollection<PlanModel>, int>>(planModelListCountTuple);
        }

        public async Task<RepositoryResponse<PlanModel>> CreateOrUpdateAsync(PlanModel planModel)
        {
            logger.LogDebug("PlanRepository called CreateOrUpdateAsync (PlanModel)");

            RepositoryResponse<PlanModel> repositoryResponse = await ApplyChangesPlanAsync(planModel);

            logger.LogDebug("PlanRepository end call CreateOrUpdateAsync (PlanModel) -> return Repository response PlanModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<PlanModel>> UpdateTenderPackPlanAsync(TenderPackUpdatePlanModel updatePlanModel)
        {
            logger.LogDebug("PlanRepository called UpdateTenderPackPlanAsync(TenderPackUpdatePlanModel)");

            UnitOfWork.BeginTransaction();

            var planEntity = await base.FindOneAsync<Plan>(updatePlanModel.Id);
            if (planEntity.IsNull())
            {
                UnitOfWork.Rollback();

                logger.LogDebug("No plan found");

                logger.LogDebug("PlanController end call UpdateTenderPackPlan -> return Not found");
                var response = new RepositoryResponse<PlanModel>();
                response.AddError(ErrorCode.EntityNotFound, "Plan not found");
                
                return response;
            }
            planEntity.PlanType = updatePlanModel.PlanType;
            planEntity.PlanName = updatePlanModel.PlanName;
            planEntity.PlanReference = updatePlanModel.PlanReference;

            await entityRepositoryKey.UpdateHousingTypeNameAsync(planEntity.HousingTypeId ?? 0, 
                                                                 updatePlanModel.HousingTypeName, 
                                                                 context, logger);
            await entityRepositoryKey.UpdateHousingSpecificationNameAsync(planEntity.HousingSpecificationId ?? 0,
                                                                          updatePlanModel.HousingSpecificationName,
                                                                          context, logger);
            UnitOfWork.Commit();

            var planModel = mapper.Map<Plan, PlanModel>(planEntity);

            logger.LogDebug("PlanRepository end call UpdateTenderPackPlanAsync(TenderPackUpdatePlanModel) -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>(planModel);
        }

        public async Task<RepositoryResponse<PlanModel>> CreateOrUpdateAsync(PlanModel planModel, int AiepId)
        {
            logger.LogDebug("PlanRepository called CreateOrUpdateAsync (PlanModel, Int AiepId)");

            RepositoryResponse<PlanModel> repositoryResponse = await ApplyChangesPlanAsync(planModel, AiepId);

            logger.LogDebug("PlanRepository end call CreateOrUpdateAsync (PlanModel, Int AiepId) -> return Repository response PlanModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<PlanModel>> ApplyChangesPlanAsync(PlanModel model)
        {
            logger.LogDebug($"{nameof(PlanRepository)} called {nameof(ApplyChangesPlanAsync)}");

            var spec = new Specification<Plan>(p => p.Id == model.Id);
            var entityById = await repository.Where(spec)
                .Include(p => p.EducationToolOrigin)
                .FirstOrDefaultAsync();

            var repoResponse = await FindOnePlanWhithoutArchivedPlansAsync(model.Id);

            //Plan Archived
            if ((entityById == null) != (repoResponse.Content == null))
            {
                repoResponse.ErrorList.Add(ErrorCode.GenericBusinessError.GetDescription());
                return repoResponse;
            }

            // Create
            if (entityById == null && repoResponse.Content == null)
            {
                var EducationToolOrigin = string.IsNullOrEmpty(model.EducationOrigin)
                    ? null
                    : (await this.repository.Where(new Specification<EducationToolOrigin>(x => x.Name == model.EducationOrigin))
                        .SingleOrDefaultAsync());

                this.executionStrategy.Execute(() =>
                {
                    UnitOfWork.BeginTransaction();

                    model.PlanState = PlanState.Active;

                    entityById = mapper.Map<PlanModel, Plan>(model);
                    entityById.EducationToolOriginId = EducationToolOrigin?.Id;

                    entityById = repository.Add(entityById);

                    logger.LogDebug("PlanRepository ApplyChangesPlanAsync call Commit");

                    UnitOfWork.Commit();
                });
            }
            //Update
            else if (entityById != null && repoResponse.Content != null)
            {
                this.executionStrategy.Execute(() =>
                {
                    UnitOfWork.BeginTransaction();

                    entityById.OfflineSyncDate = null;
                    entityById.PlanType = model.PlanType;
                    entityById.PlanName = model.PlanName;
                    entityById.Survey = model.Survey;
                    entityById.EducationerId = model.EducationerId;
                    if (model.EndUser != null)
                    {
                        entityById.EndUser = this.mapper.Map(model.EndUser, entityById.EndUser);
                    }
                    else if (model.EndUserId <= 0)
                    {
                        entityById.EndUser = null;
                    }

                    entityById.EndUserId = model.EndUserId;
                    entityById.MasterVersionId = model.MasterVersionId;
                    entityById.CatalogId = model.CatalogId;

                    logger.LogDebug("PlanRepository ApplyChangesPlanAsync call Commit");

                    UnitOfWork.Commit();
                });
            }

            repoResponse.Content = this.mapper.Map<Plan, PlanModel>(entityById);

            logger.LogDebug("PlanRepository end call ApplyChangesPlanAsync -> return Repository response PlanModel");

            return repoResponse;
        }

        public async Task<RepositoryResponseGeneric> ChangePlanStateAsync(int planId, PlanState planState,
            bool includeArchived)
        {
            logger.LogDebug("PlanRepository called ChangePlanStateAsync");

            Plan plan;

            if (includeArchived)
            {
                plan = await repository.GetAll<Plan>().IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == planId);
            }
            else
            {
                plan = await repository.Where(new EntityByIdSpecification<Plan>(planId)).FirstOrDefaultAsync();
            }

            if (plan == null)
            {
                var response = new RepositoryResponseGeneric();
                response.AddError(ErrorCode.EntityNotFound, "Plan not found");

                logger.LogDebug("PlanRepository end call ChangePlanStateAsync -> return Repository response generic Errors Entity not found");

                return response;
            }

            if (plan.IsStarred && planState != PlanState.Active)
            {
                var response = new RepositoryResponseGeneric();
                response.AddError(ErrorCode.ActionNotAllowed, "Plan Is Starred");

                logger.LogDebug("PlanRepository end call ChangePlanStateAsync -> return Repository response generic Errors Action not allowed");

                return response;
            }

            UnitOfWork.BeginTransaction();
            plan.PlanState = planState;

            logger.LogDebug("PlanRepository ChangePlanStateAsync call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call ChangePlanStateAsync -> return Repository response generic");

            return new RepositoryResponseGeneric();
        }


        public async Task<RepositoryResponse<PlanModel>> ApplyChangesPlanAsync(PlanModel model, int AiepId)
        {
            logger.LogDebug("PlanRepository called ApplyChangesPlanAsync");

            var plan = await ApplyChangesPlanAsync(model);

            if (plan.HasError() || plan.Content == null)
            {
                logger.LogDebug("PlanRepository end call ApplyChangesPlanAsync -> return Repository response PlanModel Errors or Null plan");

                return plan;
            }

            var entityById = await repository.Where(new EntityByIdSpecification<Plan>(plan.Content.Id))
                .Include(p => p.EndUser)
                .ThenInclude(e => e.EndUserAieps)
                .FirstOrDefaultAsync();

            if (entityById.EndUser != null)
            {
                this.executionStrategy.Execute(() =>
                {
                    UnitOfWork.BeginTransaction();

                    if (!entityById.EndUser.EndUserAieps.Any(e => e.AiepId == AiepId))
                    {
                        entityById.EndUser.EndUserAieps.Add(new EndUserAiep()
                        {
                            EndUserId = entityById.EndUser.Id,
                            AiepId = AiepId
                        });
                    }

                    logger.LogDebug("PlanRepository ApplyChangesPlanAsync call Commit");

                    UnitOfWork.Commit();
                });
            }

            logger.LogDebug("PlanRepository end call ApplyChangesPlanAsync -> Repository response PlanModel");

            return plan;
        }


        /// <summary>
        /// Deletes all the empty plans in the last 24 hours
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public async Task<bool> DeleteEmptyPlans()
        {
            logger.LogDebug("PlanRepository called DeleteEmptyPlans");

            try
            {
                UnitOfWork.BeginTransaction();
                DateTime now = DateTime.UtcNow;
                var spec = new Specification<Plan>(p => p.CreatedDate > now.AddDays(-1));
                var todayPlans = await repository.Where(spec).Include(x => x.Project).ToListAsync();

                foreach (var plan in todayPlans)
                {
                    var specVersion = new Specification<Domain.Entity.Version>(v => v.PlanId == plan.Id);
                    var versionExists = await repository.AnyAsync(specVersion);
                    if (!versionExists)
                    {
                        base.Remove(plan.Id);
                    }

                }

                logger.LogDebug("PlanRepository DeleteEmptyPlans call Commit");

                UnitOfWork.Commit();

                logger.LogDebug("PlanRepository end call DeleteEmptyPlans -> return True");

                return true;
            }
            catch (Exception)
            {
                logger.LogDebug("PlanRepository end call DeleteEmptyPlans -> return False");

                return false;
            }
        }

        public async Task<RepositoryResponse<PlanModel>> ChangePlanStateAsync(PlanModel planModel, PlanState planState)
        {
            logger.LogDebug("PlanRepository called ChangePlansStateAsync");

            if ((planState != PlanState.Active) && planModel.IsStarred)
            {
                logger.LogDebug("PlanRepository end call ChangePlansStateAsync -> return Repository response PlanModel Errors Action not allowed");

                return new RepositoryResponse<PlanModel>(ErrorCode.ActionNotAllowed.GetDescription());
            }

            UnitOfWork.BeginTransaction();
            var entity = entityRepositoryKey.FindOne<Plan>(planModel.Id);
            var plan = await entityRepositoryKey.ChangePlanState(entity, planState, logger);

            logger.LogDebug("PlanRepository ChangePlansStateAsync call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call ChangePlansStateAsync -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>(mapper.Map<Plan, PlanModel>(plan));
        }

        public async Task<RepositoryResponse<PlanModel>> AssignBuilderToPlan(int planId, BuilderModel builderModel)
        {
            logger.LogDebug("PlanRepository called AssignBuilderToPlan");

            try
            {
                UnitOfWork.BeginTransaction();
                var plan = base.FindOne<Plan>(planId);
                plan.BuilderId = builderModel.Id;
                plan.BuilderTradingName = builderModel.TradingName;
                plan.OfflineSyncDate = null;

                logger.LogDebug("PlanRepository AssignBuilderToPlan call Commit");

                UnitOfWork.Commit();

                var planModel = mapper.Map<Plan, PlanModel>(plan);

                logger.LogDebug("PlanRepository end call AssignBuilderToPlan -> return Repository response PlanModel");

                return new RepositoryResponse<PlanModel>() { Content = planModel, ErrorList = new Collection<string>() { } };
            }
            catch (Exception ex)
            {
                logger.LogError("AssignBuilderToPlan", ex);
                UnitOfWork.Rollback();

                logger.LogDebug("PlanRepository end call AssignBuilderToPlan -> exception");

                throw;
            }

        }

        public async Task<RepositoryResponse<PlanModel>> UnassignBuilderFromPlan(int planId)
        {
            logger.LogDebug("PlanRepository called UnassignBuilderFromPlan");

            UnitOfWork.BeginTransaction();
            var plan = await entityRepositoryKey.UnassignBuilderFromPlan(planId, logger);

            logger.LogDebug("PlanRepository UnassignBuilderFromPlan call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call UnassignedBuilderFromPlan -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>(mapper.Map<Plan, PlanModel>(plan));
        }

        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> GetAllPlansForProjectAsync(int projectId)
        {
            logger.LogDebug("PlanRepository called GetAllPlansForProjectAsync");

            var spec = new Specification<Plan>(p => p.ProjectId == projectId);
            var pageDescriptor = new PageDescriptor(null, null);
            var sortModel = new SortDescriptor { Member = "Id", Direction = SortDirection.Ascending };
            pageDescriptor.Sorts.Add(sortModel);

            var query = new PlanMaterializedPlanModelPagedValueQuery(spec, Specification<PlanModel>.True, pageDescriptor.Sorts, pageDescriptor);
            var result = await repository.QueryAsync(query);

            if (result.Total > 1000)
            {
                logger.LogDebug("PlanRepository end call GetAllPlansForProjectAsync -> return List of PlanModel Errors Max take exceeded");

                return new RepositoryResponse<IEnumerable<PlanModel>>(result.Result, ErrorCode.MaxTakeExceeded, "The maximum number of plans has been exceeded");
            }

            logger.LogDebug("PlanRepository end call GetAllPlansForProjectAsync -> return Repository response List of PlanModel");

            return new RepositoryResponse<IEnumerable<PlanModel>>() { Content = result.Result };
        }

        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> GetAllPlansWithoutArchivedPlansAsync()
        {
            logger.LogDebug("PlanRepository called UnassignedBuilderFromPlan");

            var spec = new ActivePlansSpecification();
            var planList = await this.repository.Where<Plan>(spec).Include(p => p.EducationToolOrigin).ToListAsync();
            var planModelList = this.mapper.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(planList);

            logger.LogDebug("PlanRepository end call UnassignedBuilderFromPlan -> return Repository response List of PlanModel");

            return new RepositoryResponse<IEnumerable<PlanModel>>() { Content = planModelList };
        }

        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> GetAllPlansWithArchivedPlansAsync()
        {
            logger.LogDebug("PlanRepository called GetAllPlansWithArchivedPlansAsync");

            var planList = await this.repository.GetAll<Plan>().Include(p => p.EducationToolOrigin).ToListAsync();
            var planModelList = this.mapper.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(planList);

            logger.LogDebug("PlanRepository end call GetAllPlansWithArchivedPlansAsync -> Repository response List of PlanModel");

            return new RepositoryResponse<IEnumerable<PlanModel>>() { Content = planModelList };
        }

        public async Task<RepositoryResponse<IPagedQueryResult<PlanModel>>> GetAllArchivedPlansAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("PlanRepository called GetAllArchivedPlansAsync");

            var spec = new ArchivedPlansSpecification();

            var modelSpec = SpecificationBuilder.Create<PlanModel>(searchModel.Filters);

            var query = new PlanMaterializedPlanModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("PlanRepository end call GetAllArchivedPlansAsync -> return Repository response Paged query PlanModel");

            return new RepositoryResponse<IPagedQueryResult<PlanModel>>(result);
        }

        public async Task<RepositoryResponse<PlanModel>> FindOnePlanWhithoutArchivedPlansAsync(int planId)
        {
            logger.LogDebug("PlanRepository called FindOnePlanWhithoutArchivedPlansAsync");

            var spec = new ActivePlanByIdSpecification(planId);
            var plan = await this.repository.Where<Plan>(spec)
                .Include(x => x.EndUser)
                .Include(p => p.EducationToolOrigin)
                .FirstOrDefaultAsync();
            var planModel = this.mapper.Map<Plan, PlanModel>(plan);

            logger.LogDebug("PlanRepository end call FindOnePlanWhithoutArchivedPlansAsync -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>() { Content = planModel };
        }

        private string ComposeAddress0(EndUser endUser)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(endUser.Address1).Append(" ");
            stringBuilder.Append(endUser.Address2).Append(" ");
            stringBuilder.Append(endUser.Address3);
            return stringBuilder.ToString();
        }

        private string ComposeFullName(EndUser endUser)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(endUser.FirstName).Append(" ").Append(endUser.Surname);
            return stringBuilder.ToString();
        }

        public async Task AutomaticArchive(int archiveDays)
        {
            logger.LogDebug("PlanRepository called AutomaticArchive (int)");

            UnitOfWork.BeginTransaction();
            ISpecification<Plan> spec = new PlansToArchiveSpecification(archiveDays);
            var plans = await repository.Where(spec).IgnoreQueryFilters().ToListAsync();
            foreach (var plan in plans)
            {
                plan.PlanState = PlanState.Archived;
            }

            logger.LogDebug("PlanRepository AutomaticArchive (int) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AutomaticArchive (int)");
        }

        public async Task AutomaticNonTenderPackArchiveAsync(DateTime dateTime, int archiveDays, int AiepId)
        {
            logger.LogDebug("PlanRepository called AutomaticNonChtpArchive (DateTime, int archiveDays, int AiepId)");

            ISpecification<Plan> spec = new PlansToArchiveByStateSpecification();
            UnitOfWork.BeginTransaction();
            List<Plan> plans = await repository.Where(spec)
                                               .Include(p => p.Project)
                                               .Where(x => x.Project.AiepId == AiepId)
                                               .IgnoreQueryFilters()
                                               .ToListAsync();

            if (plans.Count > 0)
            {
                List<Plan> nonChtpPlans = new();
                foreach (var plan in plans)
                {
                    if (!await IsPlanChtpAsync(plan.Id, logger))
                    {
                        nonChtpPlans.Add(plan);
                    }
                }
                List<Plan> planToArchive = nonChtpPlans.Where(p => (dateTime - p.UpdatedDate).TotalDays >= archiveDays)
                                                        .ToList();
                foreach (var plan in planToArchive)
                {
                    plan.PlanState = PlanState.Archived;
                }
            }
            logger.LogDebug("PlanRepository AutomaticNonChtpArchive (DateTime, int archiveDays, int AiepId) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AutomaticNonChtpArchive (DateTime, int archiveDays, int AiepId)");
        }

        public async Task AutomaticTenderPackArchiveAsync(DateTime dateTime, int archiveDays, int AiepId)
        {
            logger.LogDebug("PlanRepository called AutomaticNonChtpArchive (DateTime, int archiveDays, int AiepId)");

            ISpecification<Plan> spec = new PlansToArchiveByStateSpecification();
            UnitOfWork.BeginTransaction();
            List<Plan> plans = await repository.Where(spec)
                                               .Include(p => p.Versions)
                                               .Include(p => p.Project)
                                               .Where(x => x.Project.AiepId == AiepId)
                                               .IgnoreQueryFilters()
                                               .ToListAsync();

            if (plans.Count > 0)
            {
                List<Plan> chtpPlans = new();
                foreach (var plan in plans)
                {
                    if (await IsPlanChtpAsync(plan.Id, logger))
                    {
                        chtpPlans.Add(plan);
                    }
                }
                List<Plan> planToArchive = plans.Where(p => ((dateTime - p.UpdatedDate).TotalDays >= archiveDays) &&
                                                            p.Versions.All(v => (dateTime - v.UpdatedDate).TotalDays >= archiveDays))
                                                .ToList();
                foreach (var plan in planToArchive)
                {
                    plan.PlanState = PlanState.Archived;
                }
            }
            logger.LogDebug("PlanRepository AutomaticNonChtpArchive (DateTime, int archiveDays, int AiepId) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AutomaticNonChtpArchive (DateTime, int archiveDays, int AiepId)");
        }

        public async Task AutomaticDeletion(double delete)
        {
            logger.LogDebug("PlanRepository called AutomaticDeletion (double)");

            UnitOfWork.BeginTransaction();
            ISpecification<Plan> spec = new ArchivedPlansToDeleteSpecification(delete);
            var plans = await repository.Where(spec).IgnoreQueryFilters().ToListAsync();
            foreach (var plan in plans)
            {
                if (plan.MasterVersionId.IsNotNull())
                {
                    context.NullifyMasterVersionSqlAsync(plan);
                }

                var versions = await repository.Where(new VersionsByPlanIdSpecification(plan.Id)).IgnoreQueryFilters()
                    .Include(v => v.RomItems).ToListAsync();
                foreach (var version in versions)
                {
                    foreach (var romItem in version.RomItems)
                    {
                        logger.LogDebug("PlanRepository AutomaticDeletion (double) call DeleteRomItemSqlAsync");
                        context.DeleteRomItemSqlAsync(romItem.Id);
                    }

                    logger.LogDebug("PlanRepository AutomaticDeletion (double) call DeleteVersionByIdSqlAsync");
                    context.DeleteVersionByIdSqlAsync(version.Id);
                }

                logger.LogDebug("PlanRepository AutomaticDeletion (double) call ExecuteSqlCommandAsync");
                context.DeletePlanByIdSqlAsync(plan.Id);
            }

            logger.LogDebug("PlanRepository AutomaticDeletion (double) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AutomaticDeletion (double)");
        }

        public async Task AutomaticDeletion(double delete, DateTime dateTime, int AiepId)
        {
            logger.LogDebug("PlanRepository called AutomaticDeletion (double, DateTime, int)");

            UnitOfWork.BeginTransaction();
            ISpecification<Plan> spec = new ArchivedPlansToDeleteByDateSpecification();
            List<Plan> plans = await repository.Where(spec).Include(p => p.Project).Where(x => x.Project.AiepId == AiepId).IgnoreQueryFilters().ToListAsync();

            if (plans.Count > 0)
            {
                List<Plan> plansToDelete = plans.AsQueryable().Where(p => ((dateTime - p.UpdatedDate).TotalDays >= delete)).ToList();

                foreach (var plan in plansToDelete)
                {
                    if (plan.MasterVersionId.IsNotNull())
                    {
                        context.NullifyMasterVersionSqlAsync(plan);
                    }

                    var versions = await repository.Where(new VersionsByPlanIdSpecification(plan.Id)).IgnoreQueryFilters()
                        .Include(v => v.RomItems).ToListAsync();
                    foreach (var version in versions)
                    {
                        foreach (var romItem in version.RomItems)
                        {
                            logger.LogDebug("PlanRepository AutomaticDeletion (double, DateTime, int) call ExecuteSqlCommandAsync");

                            context.DeleteRomItemSqlAsync(romItem.Id);
                        }

                        logger.LogDebug("PlanRepository AutomaticDeletion (double, DateTime, int) call ExecuteSqlCommandAsync");

                        context.DeleteVersionByIdSqlAsync(version.Id);
                    }

                    logger.LogDebug("PlanRepository AutomaticDeletion (double, DateTime, int) call ExecuteSqlCommandAsync");
                    context.DeletePlanByIdSqlAsync(plan.Id);
                }
            }


            logger.LogDebug("PlanRepository AutomaticDeletion (double, DateTime, int) call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AutomaticDeletion (double, DateTime, int)");

        }

        public async Task<PlanModel> AssignBuilder(int planId, int builderId)
        {
            logger.LogDebug("PlanRepository called AssignBuilder");

            UnitOfWork.BeginTransaction();
            var spec = new Specification<Plan>(p => p.Id == planId);
            var plan = await repository.Where(spec).Include(p => p.EducationToolOrigin).FirstOrDefaultAsync();
            plan.BuilderId = builderId;

            logger.LogDebug("PlanRepository AssignBuilder call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AssignBuilder -> return PlanModel");

            return mapper.Map<Plan, PlanModel>(plan);
        }

        public async Task AssignPlanToAiepAsync(int planId, int AiepId)
        {
            logger.LogDebug("PlanRepository called AssignPlanToAiepAsync");

            UnitOfWork.BeginTransaction();
            var spec = new Specification<Plan>(p => p.Id == planId);
            var planModels = await repository.Where(spec)
                                             .Include(x => x.Project)
                                             .ToListAsync();

            planModels.FirstOrDefault().Project.AiepId = AiepId;

            logger.LogDebug("PlanRepository AssignPlanToAiepAsync call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call AssignPlanAiepAsync");
        }

        /// <summary>
        /// Transfer a single plan to an unassigned Aiep
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<PlanModel>> TransferSinglePlanBetweenAieps(int planId, int AiepId)
        {
            logger.LogDebug("PlanRepository called TransferSinglePlanBetweenAieps");

            UnitOfWork.BeginTransaction();
            var plan = await entityRepositoryKey.TransferSinglePlanBetweenAieps(planId, AiepId, logger);

            logger.LogDebug("PlanRepository TransferSinglePlanBetweenAieps call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call TransferSinglePlanBetweenAieps -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>(mapper.Map<Plan, PlanModel>(plan));
        }

        public async Task<RepositoryResponse<bool>> TransferMultipleProjectPlansToAiep(int projectId, string AiepCode)
        {
            logger.LogDebug("PlanRepository called TransferMultipleProjectPlansToAiep");

            bool isArchived = await entityRepositoryKey.IsProjectArchived(projectId, logger);
            if (isArchived)
            {
                string message = $"Project with ProjectId = {projectId} is already archived";
                logger.LogDebug(message);
                return new RepositoryResponse<bool>(false, new List<string>() { message });
            }

            RepositoryResponse<AiepModel> Aiep = await AiepRepo.GetAiepByCode(AiepCode);
            if (Aiep.HasError())
            {
                logger.LogDebug(string.Join(";" + Environment.NewLine, Aiep.ErrorList));
                return new RepositoryResponse<bool>(false, Aiep.ErrorList);
            }

            await this.executionStrategy.ExecuteAsync(async () =>
            {
                UnitOfWork.BeginTransaction();
           
                var spec = new Specification<Plan>(p => p.ProjectId == projectId && p.PlanState == PlanState.Active);
                var plans = await repository.Where(spec)
                                                 .Include(x => x.Project)
                                                 .ToListAsync();
                foreach (Plan plan in plans)
                {
                    await entityRepositoryKey.TransferSinglePlanBetweenAieps(plan.Id, Aiep.Content.Id, logger);
                }

                logger.LogDebug("PlanRepository TransferMultiplePlanToUnassignedBuilder call Commit");

                UnitOfWork.Commit();
            }, CancellationToken.None);

            logger.LogDebug("PlanRepository end call TransferMultipleProjectPlansToAiep -> return Repository response List of PlanModel");

            return new RepositoryResponse<bool>(true);
        }

        public async Task<RepositoryResponse<IEnumerable<PlanModel>>> TransferMultiplePlanToUnassignedBuilder(int builderId, int AiepId)
        {
            logger.LogDebug("PlanRepository called TransferMultiplePlanToUnassignedBuilder");

            UnitOfWork.BeginTransaction();
            var spec = new Specification<Plan>(p => p.BuilderId == builderId);
            var plans = await repository.Where(spec)
                                             .Include(x => x.Project)
                                             .ToListAsync();

            foreach (Plan plan in plans)
            {
                await entityRepositoryKey.TransferSinglePlanBetweenAieps(plan.Id, AiepId, logger);
            }

            logger.LogDebug("PlanRepository TransferMultiplePlanToUnassignedBuilder call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call TransferMultiplePlanToUnassignedBuilder -> return Repository response List of PlanModel");

            return new RepositoryResponse<IEnumerable<PlanModel>>(mapper.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(plans));
        }

        public async Task<RepositoryResponse<PlanModel>> CopyToProject(PlanModel planModel, int projectId)
        {
            logger.LogDebug("PlanRepository called CopyToProject");

            planModel.Id = 0;
            planModel.ProjectId = projectId;
            planModel.CreatedDate = DateTime.UtcNow;

            logger.LogDebug("PlanRepository end call CopyToProject -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>(planModel);
        }

        public async Task<RepositoryResponse<PlanModel>> GetPlanWithVersions(int planId)
        {
            logger.LogDebug("PlanRepository called GetPlansWithVersions");

            var plan = await repository.Where(new EntityByIdSpecification<Plan>(planId)).Include(p => p.Versions).FirstOrDefaultAsync();

            if (plan == null)
            {
                logger.LogDebug("PlanRepository end call GetPlanWithVersions -> return Repository response Errors Entity not found");

                return new RepositoryResponse<PlanModel>(null, ErrorCode.EntityNotFound);
            }

            var planModel = mapper.Map<Plan, PlanModel>(plan);

            logger.LogDebug("PlanRepository end call GetPlanWithVersions -> return Repository response PlanModel");

            return new RepositoryResponse<PlanModel>(planModel);
        }

        public async Task<PlanModel2> FindOneWithEducationerAsync(int planId)
        {
            logger.LogDebug("PlanRepository called FindOneWithEducationerAsync");

            var entity = await this.repository.Where(new EntityByIdSpecification<Plan>(planId)).Include(p => p.Educationer).FirstOrDefaultAsync();
            var model = this.mapper.Map<Plan, PlanModel2>(entity);
            //model.Educationer = this.mapper.Map<Educationer, EducationerModel>(entity.Educationer);

            logger.LogDebug("PlanRepository end call FindOneWithEducationerAsync -> return PlanModel2");

            return model;
        }

        public async Task<bool> UpdateBuilderPlansTradingName(int builderId, string builderTradingName)
        {
            logger.LogDebug("PlanRepository called UpdateBuilderPlansTradingName");

            try
            {
                await UnitOfWork.BeginTransactionAsync();

                var planList = await this.repository.GetAll<Plan>().Where(p => p.BuilderId == builderId).IgnoreQueryFilters().ToListAsync();

                foreach (var plan in planList)
                {
                    if (builderTradingName != plan.BuilderTradingName)
                    {
                        plan.BuilderTradingName = builderTradingName;
                    }
                }

                logger.LogDebug("PlanRepository UpdateBuilderPlansTradingName call Commit");

                await UnitOfWork.CommitAsync();

                logger.LogDebug("PlanRepository end call UpdateBuilderPlansTradingName -> return True");

                return true;
            }
            catch (Exception)
            {
                logger.LogDebug("PlanRepository end call UpdateBuilderPlansTradingName -> exception return False");

                return false;
            }
        }

        public async Task CallIndexerAsync(int take, int skip, DateTime? updatedDate, int? indexerWindowInDays)
        {
            logger.LogDebug("PlanRepository called CallIndexerAsync");

            //var plans = await entityRepositoryKey.GetEntitiesNoFiltersAsync<Plan>(take, skip, updatedDate, indexerWindowInDays).ToListAsync();

            //azureSearchManagementService.MergeOrUploadDocuments
            //    (azureSearchManagementService.GetDocuments<OmniSearchPlanIndexModel, Plan>(plans));

            logger.LogDebug("PlanRepository end call CallIndexerAsync");
        }

        public async Task<RepositoryResponse<PlanModel>> CreatePlan(PlanModel planModel, int AiepId, int userId)
        {
            logger.LogDebug("PlanRepository called CreatePlan");

            await PopulateEducationerAndPlanCode(planModel, userId);

            UnitOfWork.BeginTransaction();

            var project = entityRepositoryKey.CreateProjectForAPlan(planModel.PlanCode, AiepId, logger);

            if (planModel.EndUser.IsNotNull())
            {
                var response = await UpdateEndUserInPlan(planModel);
                if (response.HasError()) 
                {
                    UnitOfWork.Rollback();
                    return response;
                }
                planModel = response.Content;               
            }

            if (planModel.BuilderId.IsNotNull())
            {
                var builder = await base.FindOneAsync<Builder>(planModel.BuilderId.Value);
                var builderModel = mapper.Map(builder, new BuilderModel());
                planModel.BuilderTradingName = builderModel.TradingName;
            }

            logger.LogDebug("PlanRepository CreatePlan call Commit");

            UnitOfWork.Commit();

            DetachEntities<EndUser>();

            planModel.ProjectId = project.Id;

            logger.LogDebug("PlanRepository end call CreatePlan -> return Call CreateOrUpdateAsync");

            return await CreateOrUpdateAsync(planModel, AiepId);
        }

        public async Task<RepositoryResponse<PlanModel>> CreateTenderPackPlan(PlanModel planModel, int AiepId, int userId)
        {
            logger.LogDebug("PlanRepository called CreateTenderPackPlan");

            await PopulateEducationerAndPlanCode(planModel, userId);

            UnitOfWork.BeginTransaction();

            var housingSpecs = await entityRepositoryKey.GetHousingSpecificationAsync((int)planModel.HousingSpecificationId, logger);
            var project = await entityRepositoryKey.UpdateProjectWithHousingSpecsAsync(housingSpecs, planModel.ProjectId, context, logger);

            if (planModel.EndUser.IsNotNull())
            {
                var response = await UpdateEndUserInPlan(planModel);
                if (response.HasError())
                {
                    UnitOfWork.Rollback();
                    return response;
                }
                planModel = response.Content;
            }

            var planObj = mapper.Map(planModel, new Plan());
            planObj.EndUserId = planObj.EndUser?.Id;
            planObj.EndUser = null;
            planObj.PlanReference = planModel.PlanReference;

            if (planModel.BuilderId.IsNotNull())
            {
                var builder = await base.FindOneAsync<Builder>(planModel.BuilderId.Value);
                planObj.BuilderTradingName = builder.TradingName;
            }
            
            planObj.ProjectTemplates = null;
            planObj.HousingSpecificationTemplates = null;
            planObj.IsTemplate = false;

            repository.Add(planObj);
            context.SaveChanges();

            //await entityRepositoryKey.UpdateHousingTypeAsync((int)planModel.HousingTypeId, planObj.Id, context, logger);

            logger.LogDebug("PlanRepository CreateTenderPackPlan call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("PlanRepository end call CreateTenderPackPlan -> return RepositoryResponse PlanModel");

            planModel = MappingPlan(planModel, planObj);

            return new RepositoryResponse<PlanModel>(planModel);
        }       

        public async Task<RepositoryResponse<IPagedQueryResult<PlanModel>>> GetPlansSortedAsync(SortDescriptor sortModel, int builderId, int? AiepId = null, bool GetOnlyArchivedPlans = false)
        {
            var spec = GetOnlyArchivedPlans ? new ArchivedPlansSpecification().And(new PlansByBuilderIdSpecification(builderId)) : new ActivePlansSpecification().And(new PlansByBuilderIdSpecification(builderId));

            if (AiepId.HasValue && AiepId.Value != -1)
            {
                spec = spec.And(new Specification<Plan>(p => p.Project.AiepId == AiepId.Value));
            }

            var pageDescriptor = new PageDescriptor(null, null);
            pageDescriptor.Sorts.Add(sortModel);

            var query = new PlanMaterializedPlanModelPagedValueQuery(spec, Specification<PlanModel>.True, pageDescriptor.Sorts, pageDescriptor);
            var result = repository.Query(query);
            return new RepositoryResponse<IPagedQueryResult<PlanModel>>(result);
        }

        public async Task<Plan> GetPlanWithHousingTypeHousingSpecs(int planId)
        {
            logger.LogDebug("PlanRepository called GetPlanWithHousingTypeHousingSpecs");

            var plan = await repository.Where(new EntityByIdSpecification<Plan>(planId))
                                       .Include(x => x.HousingType)
                                       .FirstOrDefaultAsync();
            await entityRepositoryKey.FindOneAsync<HousingSpecification>((int)plan.HousingSpecificationId);

            logger.LogDebug("PlanRepository end call GetPlanWithHousingTypeHousingSpecs -> return Plan");

            return plan;
        }

        public async Task<bool> IsPlanChtpAsync(int planId, ILogger logger)
        {
            logger.LogDebug("PlanRepository called IsPlanChtpAsync");

            var spec = new Specification<Plan>(p => p.Id == planId);
            var plan = await entityRepository.Where(spec)
                                             .Include(x => x.HousingType)
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync();

            if (plan.IsNotNull() && plan.HousingType.IsNotNull())
            {
                return true;
            }

            logger.LogDebug("PlanRepository end call IsPlanChtpAsync -> return booleam");

            return false;
        }

        public async Task<RepositoryResponse<bool>> IsPlanNameDuplicateAsync(string planName)
        {
            logger.LogDebug("PlanRepository called IsPlanNameDuplicateAsync");
            planName = planName.ToLower();

            var spec = new Specification<Plan>(p => p.PlanName.ToLower().Equals(planName));
            bool isDuplicatePlan = await entityRepository.Where(spec)
                                                         .IgnoreQueryFilters()
                                                         .AsNoTracking()
                                                         .AnyAsync();

            logger.LogDebug("PlanRepository end call IsPlanNameDuplicateAsync -> return booleam");

            return new RepositoryResponse<bool>(isDuplicatePlan);
        }

        public async Task<RepositoryResponse<PlanDetailsResponseModel>> GetPlanDetailsAsync(PlanDetailsRequestModel planDetailsRequest)
        {
            var parts = planDetailsRequest.EducationViewPlanUniqueId.Split('-');
            if (long.TryParse(parts[0], out var planId) == false)
            {
                return new RepositoryResponse<PlanDetailsResponseModel>((PlanDetailsResponseModel)null);
            }

            var externalCode = parts.Length > 1 ? parts[1] : null;

            var spec = new Specification<Plan>(p => p.Id == planId
                                                    && p.Educationer.UniqueIdentifier == planDetailsRequest.EducationViewUserId
                                                    && ((externalCode == null && p.Versions.Any() == false) || (externalCode != null && p.Versions.Any(v => v.ExternalCode == externalCode))));

            IQueryable<Plan> query = entityRepository
                .Where(spec)
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(v => v.Catalog)
                .Include(u => u.Educationer)
                .ThenInclude(d => d.UserRoles).ThenInclude(d => d.Role)
                .Include(u => u.Educationer)
                .ThenInclude(d => d.Aiep).ThenInclude(d => d.Area).ThenInclude(d => d.Region).ThenInclude(d => d.Country);

            if (!string.IsNullOrEmpty(externalCode))
            {
                query = query.Include(p => p.Versions)
                    .ThenInclude(v => v.Catalog);
            }

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
            {
                return new RepositoryResponse<PlanDetailsResponseModel>((PlanDetailsResponseModel)null);
            }

            var version = externalCode == null ? null : result.Versions.FirstOrDefault(s => s.ExternalCode == externalCode);
            var response = new PlanDetailsResponseModel
            {
                Catalogue = externalCode == null ? result.Catalog.Name : version?.Catalog.Name,
                AiepCode = result.Educationer.Aiep.AiepCode,
                UserRole = string.Join(",", result.Educationer.UserRoles.Select(s => s.Role.Name)),
                Version3dc = version?.EducationTool3DCVersionId.ToString(),
                PlanId3dc = version?.EducationTool3DCPlanId,
                Country = result.Educationer.Aiep.Area.Region.Country.KeyName,
                Mode = externalCode == null ? "New" : "Edit"
            };

            return new RepositoryResponse<PlanDetailsResponseModel>(response);
        }

        private PlanModel MappingPlan(PlanModel planModel, Plan planObj)
        {
            var houseId = planModel.HousingTypeId;
            var reference = planModel.PlanReference;

            planModel = mapper.Map<Plan, PlanModel>(planObj);

            planModel.HousingTypeId = houseId;
            planModel.PlanReference = reference;

            return planModel;
        }

        private async Task PopulateEducationerAndPlanCode(PlanModel planModel, int userId)
        {
            if (planModel.EducationerId.IsNull() || planModel.EducationerId == 0)
            {
                planModel.EducationerId = userId;
            }

            if (planModel != null && planModel.PlanCode.IsNullOrEmpty())
            {
                var repositoryReponse = await entityRepositoryKey.GeneratePlanIdAsync(DateTime.UtcNow, context, logger);

                if (repositoryReponse != null)
                {
                    planModel.PlanCode = repositoryReponse.Content;
                }
            }
        }

        private async Task<RepositoryResponse<PlanModel>> UpdateEndUserInPlan(PlanModel planModel)
        {
            if (!String.IsNullOrEmpty(planModel.EndUser.Postcode))
                planModel.EndUser.Postcode = postCodeServiceFactory.GetService(null).NormalisePostcode(planModel.EndUser.Postcode);

            var entity = mapper.Map<EndUserModel, EndUser>(planModel.EndUser);
            var endUser = await entityRepositoryKey.GetEndUserByMandatoryFieldsAsync(entity, logger);

            if (endUser.IsNotNull())
            {
                planModel.EndUser.Id = endUser.Id;

                var planModels = mapper.Map<Plan, PlanModel>(endUser.Plans);
                var endUserModel = mapper.Map<EndUser, EndUserModel>(endUser);
                endUserModel.Plans = planModels.ToList();

                foreach (var plan in endUserModel.Plans)
                {
                    plan.EndUser = planModel.EndUser;
                    await CreateOrUpdateAsync(plan);
                }
            }
            else if (planModel.EndUser?.Id != 0)
            {
                logger.LogDebug(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("PlanRepository end call CreatePlan -> return Repository response Errors Entity not found");

                return new RepositoryResponse<PlanModel>(ErrorCode.EntityNotFound.GetDescription());
            }
            return new RepositoryResponse<PlanModel>(planModel);
        }

        private void DetachEntities<T>() where T : class
        {
            foreach (var entry in context.ChangeTracker.Entries<T>().ToList())
            {
                context.Entry(entry.Entity).State = EntityState.Detached;
            }
        }
    }
}




