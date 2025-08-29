
using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlansToArchiveSpecification : Specification<Plan>
    {
        public PlansToArchiveSpecification(int archiveDays) :
            base(x =>
                !x.IsStarred &&
                (x.PlanState == PlanState.Active &&
                (System.DateTime.UtcNow - x.UpdatedDate).TotalDays >= archiveDays)
            )
        {
        }
    }
}