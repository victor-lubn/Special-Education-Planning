using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserReleaseInfoSpecifications
{
    public class UserReleaseInfoByUserSpecification : Specification<UserReleaseInfo>
    {
        public UserReleaseInfoByUserSpecification(int id) : base(x => x.UserId == id)
        {
        }
    }
}
