using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Specification;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class PlanCore
    {
        public static async Task<Plan> TransferSinglePlanBetweenAieps(this IEntityRepository<int> entityRepository, int planId, int AiepId,
            ILogger logger)
        {
            logger.LogDebug("PlanCore called TransferSinglePlanBetweenAieps");

            var spec = new Specification<Plan>(p => p.Id == planId);
            var plan = await entityRepository.Where(spec)
                                             .Include(x => x.Project)
                                             .FirstOrDefaultAsync();

            plan.BuilderId = null;
            plan.BuilderTradingName = null;
            plan.Project.AiepId = AiepId;

            logger.LogDebug("PlanCore end call TransferSinglePlanBetweenAieps -> return Plan");

            return plan;
        }

        public static async Task<bool> IsProjectArchivedByPlans(this IEntityRepository<int> entityRepository, 
            int projectId,
            ILogger logger)
        {
            logger.LogDebug("PlanCore called IsProjectArchived");

            var spec = new Specification<Plan>(p => p.ProjectId == projectId && p.PlanState == PlanState.Active);
            var isPlanActive = await entityRepository.Where(spec).AnyAsync();

            logger.LogDebug("PlanCore end call IsProjectArchived -> return boolean");

            return !isPlanActive;
        }

        public static async Task<Plan> UnassignBuilderFromPlan(this IEntityRepository<int> entityRepository, int planId,
            ILogger logger)
        {
            logger.LogDebug("PlanCore called UnassignBuilderFromPlan");

            var spec = new Specification<Plan>(p => p.Id == planId);
            var plan = await entityRepository.Where(spec)
                                             .FirstOrDefaultAsync();

            plan.BuilderId = null;
            plan.BuilderTradingName = null;

            logger.LogDebug("PlanCore end call UnassignBuilderFromPlan -> return Plan");

            return plan;
        }

        public static async Task<Plan> ChangePlanState(this IEntityRepository<int> entityRepository, Plan plan, PlanState planState,
            ILogger logger)
        {
            logger.LogDebug("PlanCore called ChangePlanState");

            plan.PlanState = planState;

            logger.LogDebug("PlanCore end call ChangePlanState -> return Plan");

            return await Task.FromResult(plan);
        }

        public static async Task<RepositoryResponse<string>> GeneratePlanIdAsync(this IEntityRepository<int> entityRepository, DateTime date, DbContext context,
            ILogger logger)
        {
            logger.LogDebug("PlanCore called GeneratePlanIdAsync");

            if (date.Year < 1900) date = DateTime.UtcNow;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(date.Year.ToString().Substring(2, 2));
            stringBuilder.Append(date.Month.ToString().PadLeft(2, '0'));
            stringBuilder.Append(date.Day.ToString().PadLeft(2, '0'));

            var sequencValue = context.GetNextPlanCodeSequenceValueAsync();
            stringBuilder.Append(sequencValue.ToString());

            logger.LogDebug("PlanCore end call GeneratePlanIdAsync -> return Repository response String");

            var response = new RepositoryResponse<string>(stringBuilder.ToString(), new Collection<string>());
            return await Task.FromResult(response);
        }

    }



}

