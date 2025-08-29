using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlanByProjectIdSpecification : Specification<Plan>
    {
        public PlanByProjectIdSpecification(int projectId) : base(x => x.ProjectId == projectId)
        {
        }
    }
}
