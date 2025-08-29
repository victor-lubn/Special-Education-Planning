using Koa.Domain;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class UserReleaseInfoModel : BaseModel<int>
    {
        public int UserId { get; set; }
        public int ReleaseInfoId { get; set; }
        public virtual User User { get; set; }
        public virtual ReleaseInfo ReleaseInfo { get; set; }
    }
}
