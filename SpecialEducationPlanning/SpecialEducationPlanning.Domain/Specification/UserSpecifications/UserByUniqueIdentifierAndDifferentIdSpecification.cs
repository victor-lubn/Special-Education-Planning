using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserSpecifications
{
    public class UserByUniqueIdentifierAndDifferentIdSpecification : Specification<User>
    {
        public UserByUniqueIdentifierAndDifferentIdSpecification(string uniqueIdentifier, int id) : base(user => user.UniqueIdentifier == uniqueIdentifier && user.Id != id)
        {
        }
    }
}