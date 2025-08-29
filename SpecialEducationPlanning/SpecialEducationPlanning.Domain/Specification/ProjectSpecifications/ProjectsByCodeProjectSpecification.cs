using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.ProjectsSpecifications
{
    public class ProjectsByCodeProjectSpecification : Specification<Project>
    {
        public ProjectsByCodeProjectSpecification(string codeproject) : base(x => x.CodeProject.Contains(codeproject))
        {
        }
    }
}
