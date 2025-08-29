using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using SpecialEducationPlanning
.Api.Host.Options.OAuth2;

namespace SpecialEducationPlanning
.Api.Host.Options.Swagger
{
    internal class SecurityRequirementsOperationFilter : IOperationFilter
    {
        private readonly AuthorizationOptions authorizationOptions;
        private readonly SwaggerGenOptions swaggerOptions;
        private readonly OAuth2Options oauth2Options;

        public SecurityRequirementsOperationFilter(
            IOptions<OAuth2Options> oauth2Options,
            IOptions<AuthorizationOptions> authorizationOptions,
            IOptions<SwaggerGenOptions> swaggerOptions)
        {
            this.oauth2Options = oauth2Options.Value;
            this.authorizationOptions = authorizationOptions.Value;
            this.swaggerOptions = swaggerOptions.Value;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var allowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AllowAnonymousAttribute>()
                .Any() ?? false;
            if (allowAnonymous) return;

            var authAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .ToArray() ?? new AuthorizeAttribute[0];

            var denyAnonymous = this.authorizationOptions.DefaultPolicy?.Requirements
                                        .Any(x => x.GetType() == typeof(DenyAnonymousAuthorizationRequirement)) ?? false;

            var denyAnonymousFallback = this.authorizationOptions.FallbackPolicy?.Requirements
                                       .Any(x => x.GetType() == typeof(DenyAnonymousAuthorizationRequirement)) ?? false;


            if (authAttributes.Any() || denyAnonymous || denyAnonymousFallback)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                var requiredClaimTypes = authAttributes
                    .Select(x => x.Policy)
                    .Select(x => this.authorizationOptions.GetPolicy(x))
                    .SelectMany(x => x.Requirements)
                    .OfType<ClaimsAuthorizationRequirement>()
                    .Select(x => x.ClaimType)
                    .ToArray();


                foreach (var scheme in this.swaggerOptions.SwaggerGeneratorOptions.SecuritySchemes)
                {
                    var secRequirement = new OpenApiSecurityRequirement
                    {
                        [scheme.Value] = scheme.Value.Flows.Implicit.Scopes.Values.ToList()
                    };

                    operation.Security.Add(secRequirement);
                }
            }
        }
    }
}