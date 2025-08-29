using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlansWithEndUser : Specification<Plan>
    {
        public PlansWithEndUser() :
            base(x => x.EndUserId != null)

        {
        }
    }
}