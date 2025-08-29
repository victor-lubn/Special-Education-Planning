using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications
{
    public class ProjectChtpSpecification : Specification<Project>
    {
        public ProjectChtpSpecification() : base(x => !x.SinglePlanProject)
        {
        }
    }
}
