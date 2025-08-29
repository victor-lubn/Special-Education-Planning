using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class UserRoleWithPermissionsModel : BaseModel<int>
    {
        public virtual UserModel User { get; set; }
        public int UserId { get; set; }
        public virtual RolePermissionModel Role { get; set; }
        public int RoleId { get; set; }
    }
}
