
using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ArchivedPlansToDeleteByDateSpecification : Specification<Plan>
    {
        public ArchivedPlansToDeleteByDateSpecification() :
            base(x =>
                !x.IsStarred && x.PlanState == PlanState.Archived
            )
        {
        }
    }
}