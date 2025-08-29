using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class RoleByNameSpecification : Specification<Role>
    {
        public RoleByNameSpecification(string name) : base(x => x.Name == name)
        {
        }
    }
}