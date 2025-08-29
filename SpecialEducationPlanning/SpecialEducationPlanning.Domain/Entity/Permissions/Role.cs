using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Role : BaseEntity<int>
    {
        public string Name { get; set; }
        public virtual ICollection<PermissionRole> PermissionRoles { get; set; } = new Collection<PermissionRole>();
        public virtual IEnumerable<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
    }
}
