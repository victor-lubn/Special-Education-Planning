using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

using SpecialEducationPlanning
.Business.Model;

namespace SpecialEducationPlanning
.Api.Service.User
{

    public interface IUserService
    {

        #region Methods Public

        Task<IEnumerable<Claim>> GetClaimsAsync(int userId);

        int GetUserAssignedAiepId(ClaimsIdentity webUser);

        int GetUserCurrentAiepId(ClaimsIdentity webUser);

        int GetUserAiepId(ClaimsIdentity webUser);

        Task<UserModel> GetUserFromAppAsync(ClaimsIdentity claimsIdentity);

        bool GetUserFullAclAccess(ClaimsIdentity webUser);

        int GetUserId(ClaimsIdentity webUser);

        int GetUserId(ClaimsPrincipal webUser);

        string GetUserIdentifier(ClaimsIdentity webUser);

        IEnumerable<Claim> GetUserPermissions(ClaimsIdentity webUser);

        IEnumerable<Claim> GetUserRoles(ClaimsIdentity webUser);

        Task<IEnumerable<UserCsvModel>> ReadCsvFile(Stream stream);

        Task<bool> ResetClaimsCacheAsync(int userId);

        Task<bool> ResetClaimsCacheAsync(ClaimsIdentity webUser);

        Task<bool> ResetClaimsCacheAsync();

        #endregion

    }

}
