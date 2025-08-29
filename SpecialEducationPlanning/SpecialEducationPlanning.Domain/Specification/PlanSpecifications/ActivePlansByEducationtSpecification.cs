using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ActivePlansSpecification : Specification<Plan>
    {
        public ActivePlansSpecification() :
            base(x => x.PlanState == PlanState.Active)
        {
        }
    }
}