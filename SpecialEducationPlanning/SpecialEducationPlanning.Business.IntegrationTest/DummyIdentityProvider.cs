using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

using Koa.Platform.Providers.Identity;

namespace SpecialEducationPlanning
.Business.IntegrationTest
{
    public class DummyIdentityProvider : IIdentityProvider
    {
        public IPrincipal Identity => new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()));
    }
}
