using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using SpecialEducationPlanning
.Api.Configuration.DistributedCache;
using System.Web;


namespace SpecialEducationPlanning
.Api.Service.DistributedCache
{

    /// <inheritdoc />
    public class UserDistributedCacheService : IUserDistributedCacheService
    {

        private const char SplitClaim = '#';

        private const char SplitObject = ';';

        private const string UserClaimsKey = "UsrClms";

        private const string CacheApimTokenKey = "ApimToken";

        private readonly IDistributedCache cache;

        private readonly IOptions<DistributedCacheUserConfiguration> distributedCacheUserConfiguration;

        /// <inheritdoc />
        public UserDistributedCacheService(IDistributedCache cache,
            IOptions<DistributedCacheUserConfiguration> distributedCacheUserConfiguration)
        {
            this.cache = cache;
            this.distributedCacheUserConfiguration = distributedCacheUserConfiguration;
        }

        #region Methods IUserDistributedCacheService

        /// <inheritdoc />
        public async Task<ICollection<Claim>> ClaimsGet(int userId)
        {
            try
            {
                var output = new List<Claim>();
                var cacheByte = await cache.GetAsync($"{UserClaimsKey}{userId}");

                if (cacheByte is null)
                {
                    return output;
                }

                var cacheString = Encoding.ASCII.GetString(cacheByte);

                if (!string.IsNullOrEmpty(cacheString))
                {
                    var claimStrings = cacheString.Split(SplitObject).Select(claimString =>
                        claimString.Split(SplitClaim));

                    output.AddRange(claimStrings.Select(cs => new Claim(cs.First(), cs.Last())));
                }

                return output;
            }
            catch (Exception e)
            {
                Log.Error($"Cache error in the UserDistributedCacheService class while executing the ClaimsGet method: {e}");
            }
            return new List<Claim>();
        }

        public async Task<string> ApimTokenGetAsync()
        {
            try
            {
                var tokenByte = await cache.GetAsync($"{CacheApimTokenKey}");

                if (tokenByte is null)
                {
                    return null;
                }

                var tokenString = Encoding.ASCII.GetString(tokenByte);

                return tokenString;
            }
            catch (Exception e)
            {
                Log.Error($"Cache error in the UserDistributedCacheService class while executing the ApimTokenGet method: {e}");
            }
            return null;
        }

        public async Task<bool> ApimTokenSetAsync(int expireTime, string token)
        {
            try
            {
                await cache.SetAsync($"{CacheApimTokenKey}", Encoding.ASCII.GetBytes(token),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireTime - 10),
                    });
            }
            catch (Exception e)
            {
                Log.Error($"Cache error in the UserDistributedCacheService class while executing the ApimTokenSetAsync method: {e}");
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ClaimsRemoveAsync(int userId)
        {
            try
            {
                await cache.RemoveAsync(UserClaimsKey + userId);
               
            }

            catch (Exception e)
            {
                Log.Error($"Cache error in the UserDistributedCacheService class while executing the ClaimsRemoveAsync method: {e}");  
            }
            return true;
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public async Task<bool> ClaimsSetAsync(int userId, IEnumerable<Claim> claims)
        {
            try
            {

                var toCacheString = string.Join(SplitObject, claims.Select(c => $"{c.Type}{SplitClaim}{c.Value}").ToList());

                await cache.SetAsync($"{UserClaimsKey}{userId}", Encoding.ASCII.GetBytes(toCacheString),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = distributedCacheUserConfiguration.Value.ClaimsSlidingExpiration,
                        AbsoluteExpirationRelativeToNow =
                            distributedCacheUserConfiguration.Value.ClaimsAbsoluteExpirationRelativeToNow
                    });
            }
            
            catch (Exception e)
            {
                Log.Error($"Cache error in the UserDistributedCacheService class while executing the ClaimsSetAsync method: {e}");
            }
            return true;
        }

        #endregion

    }

}