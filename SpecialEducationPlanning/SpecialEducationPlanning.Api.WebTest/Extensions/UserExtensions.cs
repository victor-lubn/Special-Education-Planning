using SpecialEducationPlanning
.Domain.Entity;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.WebTest
{
    public static class UserExtensions
    {
        public static Task<User> WithUserInTheDatabase(this BaseTest current)
        {
            return current.WithEntityInTheDatabase(new User());
        }
    }
}
