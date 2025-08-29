using Microsoft.AspNetCore.Mvc;

namespace SpecialEducationPlanning
.Api.Configuration.Authorization
{
    public class ApiKeyAuthorizeAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthorizeAttribute(string configurationKey)
            : base(typeof(ApiKeyAuthorizeFilter))
        {
            Arguments = new object[] { configurationKey };
        }
    }
}
