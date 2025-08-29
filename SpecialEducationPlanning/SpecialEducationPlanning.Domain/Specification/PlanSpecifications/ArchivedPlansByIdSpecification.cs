using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ArchivedPlansByIdSpecification : Specification<Plan>
    {
        public ArchivedPlansByIdSpecification(int id) :
            base(
                x => x.Id == id
                && x.PlanState == PlanState.Archived
            )
        {
        }
    }
}
