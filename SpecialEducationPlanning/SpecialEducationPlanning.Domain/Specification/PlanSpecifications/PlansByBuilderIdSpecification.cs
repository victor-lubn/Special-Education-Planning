using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlansByBuilderIdSpecification : Specification<Plan>
    {
        public PlansByBuilderIdSpecification(int builderId) :
            base(
                x => (x.BuilderId == builderId)
            )
        {
        }
    }
}
