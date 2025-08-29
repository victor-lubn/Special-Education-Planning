using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications
{
    public class ProjectByIdSpecification : Specification<Project>
    {
        public ProjectByIdSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}