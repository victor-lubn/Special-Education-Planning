using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserSpecifications
{
    public class UserByAiepIdSpecification : Specification<User>
    {
        public UserByAiepIdSpecification(int AiepId, int userIdEdited) : base(user => user.AiepId == AiepId && !user.Leaver && user.Id!=userIdEdited)
        {
        }
    }
}
