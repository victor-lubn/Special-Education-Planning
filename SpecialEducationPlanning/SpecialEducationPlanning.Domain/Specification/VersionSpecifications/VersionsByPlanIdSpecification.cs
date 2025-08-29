using Koa.Domain.Specification;

namespace SpecialEducationPlanning
.Domain.Specification.VersionSpecifications
{
    public class VersionsByPlanIdSpecification : Specification<Domain.Entity.Version>
    {
        public VersionsByPlanIdSpecification(int planId) :
            base(x => x.PlanId.Equals(planId))
        {
        }
    }
}
