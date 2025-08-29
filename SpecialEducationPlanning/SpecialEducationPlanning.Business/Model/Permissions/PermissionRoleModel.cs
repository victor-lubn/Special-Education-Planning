using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class PermissionRoleModel : BaseModel<int>
    {
        public int PermissionId { get; set; }
        public PermissionModel Permission { get; set; }
        public int UserRoleId { get; set; }
        public RoleModel UserRole { get; set; }
    }
}
