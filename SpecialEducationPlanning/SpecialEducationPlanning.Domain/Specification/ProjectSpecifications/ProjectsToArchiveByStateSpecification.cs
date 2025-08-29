using Koa.Domain.Specification;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications
{
    public class ProjectsToArchiveByStateSpecification: Specification<Project>
    {
        public ProjectsToArchiveByStateSpecification()
            : base(x => x.Plans.All(p => p.PlanState == PlanState.Archived))
        {
        }
    }
}
