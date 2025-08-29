using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.PermissionRoleSpecification
{
    public class GetPermissionIdsByRoleIdSpecification : Specification<PermissionRole>
    {
        public GetPermissionIdsByRoleIdSpecification(int roleId) : base(x => x.RoleId == roleId)
        {
        }
    }
}
