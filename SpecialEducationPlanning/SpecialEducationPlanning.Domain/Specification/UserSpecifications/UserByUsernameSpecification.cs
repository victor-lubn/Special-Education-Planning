using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.UserSpecifications
{
    public class UserByUsernameSpecification : Specification<User>
    {
        public UserByUsernameSpecification(string username) : base(x => x.FirstName + " " + x.Surname == username)
        {
        }
    }
}