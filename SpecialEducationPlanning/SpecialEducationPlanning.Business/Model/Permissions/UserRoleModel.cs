using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class UserRoleModel : BaseModel<int>
    {
        public virtual UserModel User { get; set; }
        public int UserId { get; set; }
        public virtual RoleModel Role { get; set; }
        public int RoleId { get; set; }
    }
}
