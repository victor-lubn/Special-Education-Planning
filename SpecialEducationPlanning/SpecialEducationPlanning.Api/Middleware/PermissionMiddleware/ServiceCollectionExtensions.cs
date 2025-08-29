using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SpecialEducationPlanning
.Api.Middleware.PermissionMiddleware
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserClaimsMiddleware(this IServiceCollection services)
        {

            return services.AddScoped<UserClaimsMiddleware>();
        }

        public static IApplicationBuilder UseUserClaimsMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserClaimsMiddleware>();
        }
    }
}