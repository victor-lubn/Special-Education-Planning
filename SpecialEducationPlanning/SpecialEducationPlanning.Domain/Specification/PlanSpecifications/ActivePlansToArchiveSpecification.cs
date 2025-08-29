using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class ActivePlansToArchiveSpecification : Specification<Plan>
    {
        public ActivePlansToArchiveSpecification() : base(
            x =>
                !x.IsStarred &&
                (System.DateTime.UtcNow - x.UpdatedDate).TotalDays > 600
        )
        {
        }
    }
}