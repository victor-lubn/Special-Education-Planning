using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class PermissionRole : BaseEntity<int>
    {
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
