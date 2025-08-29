using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Configuration.Authorization
{
    public class AuthorizationOptionsConfigure : IConfigureOptions<AuthorizationOptions>
    {
        private readonly ApiAuthorizationOptions apiAuthorizationOptions;

        public AuthorizationOptionsConfigure(IOptions<ApiAuthorizationOptions> options)
        {
            this.apiAuthorizationOptions = options.Value;
        }

        public void Configure(AuthorizationOptions options)
        {

            if (this.apiAuthorizationOptions.IsEnabled)
            {

                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                                          .RequireAuthenticatedUser()
                                          .RequireClaim(nameof(AppClaimType.AppAccessClaimType), nameof(AppInternalPermission.AppAccess))
                                          .Build();
            }
            else
            {
                options.DefaultPolicy = null;
            }
        }
    }
}