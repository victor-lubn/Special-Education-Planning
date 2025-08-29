using Microsoft.AspNetCore.Mvc;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Configuration.OAuth
{
    /// <inheritdoc />
    public class AuthorizeTdpFilter : TypeFilterAttribute
    {
        public AuthorizeTdpFilter(params PermissionType[] permissions) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permissions };
        }
    }
}