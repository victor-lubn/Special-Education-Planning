using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.User;


namespace SpecialEducationPlanning
.Api.Middleware
{
    public class UserClaimsMiddleware : IMiddleware
    {
        private readonly ILogger<UserClaimsMiddleware> logger;
        private readonly IUserService userService;

        public UserClaimsMiddleware(ILogger<UserClaimsMiddleware> logger, IUserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User == null || !(context.User.Identity is ClaimsIdentity webUser))
            {
                logger.LogError("Invalid User ");
                await next.Invoke(context);
                return;
            }
            if (!webUser.IsAuthenticated)
            {
                logger.LogDebug("NOT Authenticated");
                await next.Invoke(context);
                return;
            }
            var user = await userService.GetUserFromAppAsync(webUser);
            if (user == null)
            {
                logger.LogDebug("User do not exist in the App");
                await next.Invoke(context);
                return;
            }
            webUser.AddClaims(await userService.GetClaimsAsync(user.Id));
            await next.Invoke(context);
        }
    }
}