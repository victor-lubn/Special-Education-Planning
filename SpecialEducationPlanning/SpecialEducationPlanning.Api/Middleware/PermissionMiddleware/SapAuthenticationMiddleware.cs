using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.SOAP;

namespace SpecialEducationPlanning
.Api.Middleware.PermissionMiddleware
{
    public class SapAuthenticationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string authenticationToken;

        public SapAuthenticationMiddleware(RequestDelegate next, string authenticationToken)
        {
            this.next = next;
            this.authenticationToken = authenticationToken;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.StartsWith(SoapSapService.UrlPath))
            {
                string token = context.Request.Headers["Authentication"];
                if (!token.IsNullOrEmpty())
                {
                    if (IsAuthorized(token))
                    {
                        await next.Invoke(context);
                        return;
                    }
                }
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context);
            }
            
        }

        public bool IsAuthorized(string token)
        {
            return token.Equals(authenticationToken, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
