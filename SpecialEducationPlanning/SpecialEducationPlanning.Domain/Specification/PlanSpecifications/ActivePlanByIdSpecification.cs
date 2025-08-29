using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ActivePlanByIdSpecification : Specification<Plan>
    {
        public ActivePlanByIdSpecification(int id) :
            base(x => x.Id == id && x.PlanState == PlanState.Active)
        {
        }
    }
}