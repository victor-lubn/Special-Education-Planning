using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserRoleSpecifications
{
    public class GetRoleIdsByUserIdSpecification : Specification<UserRole>
    {
        public GetRoleIdsByUserIdSpecification(int userId) : base(x => x.UserId == userId)
        {
        }
    }
}
