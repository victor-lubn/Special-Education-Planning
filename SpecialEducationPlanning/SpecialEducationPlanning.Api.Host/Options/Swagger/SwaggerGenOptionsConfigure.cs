using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using SpecialEducationPlanning
.Api.Configuration.Authorization;
using SpecialEducationPlanning
.Api.Host.Options.OAuth2;

namespace SpecialEducationPlanning
.Api.Host.Options.Swagger
{
    public class SwaggerGenOptionsConfigure : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly OAuth2Options oauth2Options;
        private ApiAuthorizationOptions apiAuthorizationOptions;

        public SwaggerGenOptionsConfigure(
            IOptions<OAuth2Options> oauth2Options,
            IOptions<ApiAuthorizationOptions> apiAuthorizationOptions
            )
        {
            this.oauth2Options = oauth2Options.Value;
            this.apiAuthorizationOptions = apiAuthorizationOptions.Value;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Education View API",
                    Description = "Tool for testing Education View API"
                }
            );

            if (this.apiAuthorizationOptions.IsEnabled)
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Name = "oauth2",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    },
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{this.oauth2Options.Authority}/oauth2/v2.0/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                {this.oauth2Options.Scope, this.oauth2Options.Scope}
                            }
                        }
                    }
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            }



            options.CustomOperationIds(description =>
            {
                if (description.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var controllerName = controllerActionDescriptor.ControllerName;
                    var actionName = controllerActionDescriptor.AttributeRouteInfo.Name ??
                                     controllerActionDescriptor.ActionName;
                    var actionTemplate = controllerActionDescriptor.AttributeRouteInfo.Template;
                    var actionVerb = description.HttpMethod;
                    return $"{controllerName}_{actionName}_{actionTemplate}_{actionVerb}";
                }

                return description.ActionDescriptor.Id;
            });

            options.OperationFilter<BadRequestOperationFilter>();

            options.OperationFilter<DefineHeaderOperationFilter>();

            options.IncludeXmlComments(this.GetXmlCommentsPath(PlatformServices.Default.Application));
        }

        private string GetXmlCommentsPath(ApplicationEnvironment appEnvironment)
        {
            return Path.Combine(appEnvironment.ApplicationBasePath, "SpecialEducationPlanning
.Api.Host.xml");
        }
    }
}
