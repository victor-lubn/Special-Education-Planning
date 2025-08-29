using System;

namespace SpecialEducationPlanning
.Api.Configuration.DistributedCache
{

    public class DistributedCacheUserConfiguration
    {

        #region Properties 

        public TimeSpan ClaimsAbsoluteExpirationRelativeToNow { get; set; }

        public TimeSpan ClaimsSlidingExpiration { get; set; }

        #endregion

    }

}