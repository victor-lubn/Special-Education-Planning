using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserSpecifications
{
    public class UserByUniqueIdentifierSpecification : Specification<User>
    {
        public UserByUniqueIdentifierSpecification(string uniqueIdentifier) : base(user => user.UniqueIdentifier == uniqueIdentifier)
        {
        }
    }
}