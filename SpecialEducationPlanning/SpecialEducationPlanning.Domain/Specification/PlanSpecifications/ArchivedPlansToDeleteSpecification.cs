using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ArchivedPlansToDeleteSpecification : Specification<Plan>
    {
        public ArchivedPlansToDeleteSpecification(double deleteDays) :
            base(x =>
                !x.IsStarred &&
                x.PlanState == PlanState.Archived &&
                (System.DateTime.UtcNow - x.UpdatedDate).TotalDays >= deleteDays
            )
        {
        }
    }
}