using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Permission : BaseEntity<int>
    {
        public string Name { get; set; }
        public string DescriptionCode { get; set; }
        public virtual IEnumerable<PermissionRole> PermissionRoles { get; set; } = new Collection<PermissionRole>();
    }
}
