using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserReleaseInfoSpecifications
{
    public class UserReleaseInfoByReleaseInfoIdSpecification : Specification<UserReleaseInfo>
    {
        public UserReleaseInfoByReleaseInfoIdSpecification(int releaseInfoId) : base(x => x.ReleaseInfoId == releaseInfoId)
        {
        }
    }
}
