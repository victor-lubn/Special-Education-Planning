using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class UserRole : BaseEntity<int>
    {
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual Role Role { get; set; }
        public int RoleId { get; set; }
    }
}
