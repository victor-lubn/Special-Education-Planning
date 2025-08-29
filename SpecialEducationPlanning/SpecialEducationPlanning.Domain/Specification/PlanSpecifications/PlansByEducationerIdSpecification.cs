using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlansByEducationerIdSpecification : Specification<Plan>
    {
        public PlansByEducationerIdSpecification(int id) :
            base(x => x.EducationerId == id)

        {
        }
    }
}
