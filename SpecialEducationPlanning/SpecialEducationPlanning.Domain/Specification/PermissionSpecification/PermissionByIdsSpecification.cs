using Koa.Domain.Specification;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class PermissionByIdsSpecification : Specification<Permission>
    {
        public PermissionByIdsSpecification(IEnumerable<int> permissionsIds) : base(permission =>
            permissionsIds.Any(requestedId => requestedId == permission.Id))
        {
        }
    }
}