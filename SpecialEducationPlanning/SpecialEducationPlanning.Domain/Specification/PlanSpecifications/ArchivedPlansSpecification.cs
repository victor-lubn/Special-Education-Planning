using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ArchivedPlansSpecification : Specification<Plan>
    {
        public ArchivedPlansSpecification() :
            base(
                x => (x.PlanState == PlanState.Archived)
            )
        {
        }
    }
}
