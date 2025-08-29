using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Service.DistributedCache
{

    public interface IUserDistributedCacheService
    {

        #region Methods Public

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ICollection<Claim>> ClaimsGet(int userId);

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        Task<bool> ClaimsRemoveAsync(int userId);

        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claims"></param>
        Task<bool> ClaimsSetAsync(int userId, IEnumerable<Claim> claims);

        Task<string> ApimTokenGetAsync();

        Task<bool> ApimTokenSetAsync(int expireTime, string token);

        #endregion

    }

}