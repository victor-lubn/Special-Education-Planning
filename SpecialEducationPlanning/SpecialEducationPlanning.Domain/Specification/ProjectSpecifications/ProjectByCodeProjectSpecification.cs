using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications
{
    public class ProjectByCodeProjectSpecification : Specification<Project>
    {
        public ProjectByCodeProjectSpecification(string codeproject) : base(x => x.CodeProject.Equals(codeproject))
        {
        }
    }
}