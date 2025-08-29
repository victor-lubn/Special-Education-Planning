using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlanByPlanCodeSpecification : Specification<Plan>
    {
        public PlanByPlanCodeSpecification(string planCode) : base(x => x.PlanCode == planCode)
        {
        }
    }
}