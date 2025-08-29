
using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.PlanSpecifications
{
    public class PlansToArchiveByStateSpecification : Specification<Plan>
    {



        public PlansToArchiveByStateSpecification() :
            base(x =>
                !x.IsStarred && x.PlanState == PlanState.Active)

        {
        }
    }
}