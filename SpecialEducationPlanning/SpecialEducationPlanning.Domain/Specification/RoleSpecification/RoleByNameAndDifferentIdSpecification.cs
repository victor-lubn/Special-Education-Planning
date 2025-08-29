using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class RoleByNameAndDifferentIdSpecification : Specification<Role>
    {
        public RoleByNameAndDifferentIdSpecification(string name, int id) : base(x => x.Name == name && x.Id != id)
        {
        }
    }
}