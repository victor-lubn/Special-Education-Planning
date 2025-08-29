using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class UserReleaseInfo : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int ReleaseInfoId { get; set; }
        public virtual User User { get; set; }
        public virtual ReleaseInfo ReleaseInfo { get; set; }
    }
}
