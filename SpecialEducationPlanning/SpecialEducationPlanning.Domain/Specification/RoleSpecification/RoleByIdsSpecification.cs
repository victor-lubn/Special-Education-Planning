using Koa.Domain.Specification;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class RoleByIdsSpecification : Specification<Permission>
    {
        public RoleByIdsSpecification(IEnumerable<int> roleIds) : base(role =>
            roleIds.Any(requestedId => requestedId == role.Id))
        {
        }
    }
}