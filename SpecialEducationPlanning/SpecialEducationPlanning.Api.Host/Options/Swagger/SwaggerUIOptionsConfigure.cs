using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using SpecialEducationPlanning
.Api.Host.Options.OAuth2;

namespace SpecialEducationPlanning
.Api.Host.Options.Swagger
{
    public class SwaggerUIOptionsConfigure : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly SwaggerOAuth2Options swaggerOAuth2Options;
        private readonly OAuth2Options oauth2Options;

        public SwaggerUIOptionsConfigure(
            IOptions<SwaggerOAuth2Options> swaggerOAuth2Options,
            IOptions<OAuth2Options> oauth2Options)
        {
            this.swaggerOAuth2Options = swaggerOAuth2Options.Value;
            this.oauth2Options = oauth2Options.Value;
        }
        public void Configure(SwaggerUIOptions options)
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");

            //if this is a production environment then inject a custom CSS file 
            //that will apply a red background to make it obvious 
            if (this.swaggerOAuth2Options.ApplicationName.ToLower().Contains("tdp - prod"))
            {
                options.InjectStylesheet("/swagger-custom-styles.css");
            }
            options.OAuthClientId(this.swaggerOAuth2Options.ClientId);
            options.OAuthAppName(this.swaggerOAuth2Options.ApplicationName);
            options.OAuthScopeSeparator(" ");
        }
    }
}