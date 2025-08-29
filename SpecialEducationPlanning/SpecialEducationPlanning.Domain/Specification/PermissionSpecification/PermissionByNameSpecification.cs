using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class PermissionByNameSpecification : Specification<Permission>
    {
        public PermissionByNameSpecification(string name) : base(x => x.Name == name)
        {
        }
    }
}