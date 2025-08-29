using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Configuration.OAuth
{

    /// <inheritdoc />
    public class PermissionFilter : IAuthorizationFilter
    {

        private readonly IEnumerable<PermissionType> permissions;

        public PermissionFilter(IEnumerable<PermissionType> permissions)
        {
            this.permissions = permissions;
        }

        #region Methods IAuthorizationFilter

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasAnyClaim = context.HttpContext.User.Claims.Any(c =>
                c.Type == nameof(AppClaimType.AppPermission) &&
                permissions.Any(perm => perm.GetDescription() == c.Value));

            if (!hasAnyClaim)
            {
                context.Result = new ForbidResult();
            }
        }

        #endregion

    }

}