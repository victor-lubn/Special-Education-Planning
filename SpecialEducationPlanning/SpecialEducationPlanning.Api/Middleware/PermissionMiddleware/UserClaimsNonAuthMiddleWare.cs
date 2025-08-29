using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.User;

namespace SpecialEducationPlanning
.Api.Middlewares
{
    public class UserClaimsNonAuthMiddleWare
    {
        private readonly RequestDelegate next;


        public UserClaimsNonAuthMiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService,
            ILogger<UserClaimsNonAuthMiddleWare> logger)
        {
            logger.LogDebug("Using UserClaimsNonAuthMiddleWare");
            if (context.User == null || !(context.User.Identity is ClaimsIdentity webUser))
            {
                logger.LogError("UserClaimsNonAuthMiddleWare: Invalid User ");
                await next.Invoke(context);
                return;
            }

            var user = await userService.GetUserFromAppAsync(webUser);
            if (user == null)
            {
                logger.LogDebug("UserClaimsNonAuthMiddleWare: User do not exist in the App");
                await next.Invoke(context);
                return;
            }

            webUser.AddClaims(await userService.GetClaimsAsync(user.Id));
            await next.Invoke(context);
        }
    }
}