using CsvHelper;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.DistributedCache;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.UserSpecifications;

namespace SpecialEducationPlanning
.Api.Service.User
{

    public class UserService : IUserService
    {

        private readonly IEntityRepository<int> entityRepository;

        private readonly IUserDistributedCacheService userDistributedCache;
        private readonly IObjectMapper mapper;

        private readonly ILogger<UserService> logger;

        public UserService(IDistributedCache distributedCache, IEntityRepository<int> entityRepository,
            IConfiguration configuration, IUserDistributedCacheService userDistributedCache, IObjectMapper mapper,
            ILogger<UserService> logger)
        {
            this.entityRepository = entityRepository;
            this.userDistributedCache = userDistributedCache;
            this.mapper = mapper;

            this.logger = logger;
        }

        #region Methods IUserService

        public async Task<IEnumerable<Claim>> GetClaimsAsync(int userId)
        {
            logger.LogDebug("UserService called GetClaimsAsync");

            var userClaims = await userDistributedCache.ClaimsGet(userId);

            if (userClaims.Any())
            {
                logger.LogDebug("UserService end call GetClaimsAsync -> return Collection of Claims");

                return userClaims;
            }

            userClaims.AddRange(await GetInernalClaims(userId));
            await userDistributedCache.ClaimsSetAsync(userId, userClaims);

            logger.LogDebug("UserService end call GetClaimsAsync -> return Collection of Claims");

            return userClaims;
        }

        public int GetUserAssignedAiepId(ClaimsIdentity webUser)
        {
            logger.LogDebug("GetUserAssignedAiepId called GetUserAssignedAiepId");

            var AiepIdString = webUser.Claims.FirstOrDefault(c => c.Type == nameof(AppClaimType.AppUserAiepId))
                ?.Value;

            logger.LogDebug("GetUserAssignedAiepId end call GetUserAssignedAiepId -> return Int");

            return int.Parse(AiepIdString);
        }

        public int GetUserCurrentAiepId(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserCurrentAiepId");

            var currentAiepIdString = webUser.Claims
                .FirstOrDefault(c => c.Type == nameof(AppClaimType.AppUserCurrentAiepId))
                ?.Value;

            if (currentAiepIdString.IsNotNull() && currentAiepIdString != "-1")
            {
                logger.LogDebug("UserService end call GetUserCurrentAiepId -> return Int");

                return int.Parse(currentAiepIdString);
            }

            logger.LogDebug("UserService end call GetUserCurrentAiepId -> return Int -1");

            return -1;
        }

        public int GetUserAiepId(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserAiepId");

            var currentAiepId = GetUserCurrentAiepId(webUser);

            logger.LogDebug("UserService end call GetUserAiepId -> return Int");

            return currentAiepId != -1 ? currentAiepId : GetUserAssignedAiepId(webUser);
        }

        public async Task<UserModel> GetUserFromAppAsync(ClaimsIdentity claimsIdentity)
        {
            logger.LogDebug("UserService called GetUserFromAppAsync");

            var userIdentifier = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "preferred_username") != null ?
                claimsIdentity.Claims.FirstOrDefault(c => c.Type == "preferred_username").Value :
                claimsIdentity.Claims.FirstOrDefault(c => c.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;

            var user = await entityRepository.Where(new UserByUniqueIdentifierSpecification(userIdentifier))
                .FirstOrDefaultAsync();

            logger.LogDebug("UserService end call GetUserFromAppAsync -> return UserModel");

            return this.mapper.Map<Domain.Entity.User, UserModel>(user);
        }

        public bool GetUserFullAclAccess(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserFullAclAccess");

            var fullAclAccessString = webUser.Claims
                .FirstOrDefault(c => c.Type == nameof(AppClaimType.AppUserFullAclAccess))
                ?.Value;

            logger.LogDebug("UserService end call GetUserFullAclAccess -> return Bool");

            return bool.Parse(fullAclAccessString);
        }

        public int GetUserId(ClaimsPrincipal webUser)
        {
            logger.LogDebug("UserService called GetUserId (ClaimsPrincipal)");

            logger.LogDebug("UserService end call GetUserId (ClaimsPrincipal) -> return Int");

            return webUser?.Identity == null ? 0 : GetUserId((ClaimsIdentity)webUser.Identity);
        }

        public int GetUserId(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserId (ClaimsPrincipal)");

            var userIdString = webUser.Claims.FirstOrDefault(c => c.Type == nameof(AppClaimType.AppUserIdClaimType))
                ?.Value;

            int.TryParse(userIdString, out var userId);

            logger.LogDebug("UserService end call GetUserId (ClaimsPrincipal) -> return Int");

            return userId;
        }

        public string GetUserIdentifier(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserIdentifier");

            var emailUsername = webUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(emailUsername))
            {
                return webUser.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            }

            logger.LogDebug("UserService end call GetUserIdentifier -> return String");

            return emailUsername;
        }

        public IEnumerable<Claim> GetUserPermissions(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserIdentifier");

            logger.LogDebug("UserService end call GetUserIdentifier -> return List of Claims Call Query");

            return webUser.Claims.Where(c => c.Type == nameof(AppClaimType.AppPermission)).ToList();
        }

        public IEnumerable<Claim> GetUserRoles(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called GetUserRoles");

            logger.LogDebug("UserService end call GetUserRoles -> return List of Claims Call Query");

            return webUser.Claims.Where(c => c.Type == nameof(AppClaimType.AppRole)).ToList();
        }

        public async Task<IEnumerable<UserCsvModel>> ReadCsvFile(Stream stream)
        {
            logger.LogDebug("UserService called ReadCsvFile");

            IEnumerable<UserCsvModel> records;

            using (var sr = new StreamReader(stream))
            {
                var reader = new CsvReader(sr);
                reader.Configuration.IncludePrivateMembers = true;
                var delimiter = ",";
                reader.Configuration.Delimiter = delimiter;
                reader.Configuration.MissingFieldFound = null;
                records = reader.GetRecords<UserCsvModel>().ToList();
            }

            logger.LogDebug("UserService end call ReadCsvFile -> return List of UserCsvModel");

            return records;
        }

        public async Task<bool> ResetClaimsCacheAsync(int userId)
        {
            logger.LogDebug("UserService called ResetClaimsCacheAsync (Int)");

            logger.LogDebug("UserService end call ResetClaimsCacheAsync (Int) -> return Bool Call ClaimsRemoveAsync");

            return await userDistributedCache.ClaimsRemoveAsync(userId);
        }

        public async Task<bool> ResetClaimsCacheAsync()
        {
            logger.LogDebug("UserService called ResetClaimsCacheAsync");

            var userIds = await entityRepository.GetAll<Domain.Entity.User>().Select(u => u.Id).ToListAsync();
            var tasks = new List<Task>();

            foreach (var userId in userIds)
            {
                tasks.Add(userDistributedCache.ClaimsRemoveAsync(userId));
            }

            logger.LogDebug("UserService end call ResetClaimsCacheAsync -> return Bool");

            return Task.WaitAll(tasks.ToArray(), new TimeSpan(0, 0, 0, 15));
        }

        public async Task<bool> ResetClaimsCacheAsync(ClaimsIdentity webUser)
        {
            logger.LogDebug("UserService called ResetClaimsCacheAsync (ClaimsIdentity)");

            var userId = GetUserId(webUser);

            logger.LogDebug("UserService end call ResetClaimsCacheAsync (ClaimsIdentity) -> return Bool Call ClaimsRemoveAsync");

            return await userDistributedCache.ClaimsRemoveAsync(userId);
        }

        #endregion

        #region Methods Private

        private async Task<IEnumerable<Claim>> GetInernalClaims(int userId)
        {
            logger.LogDebug("UserService called GetInernalClaims");

            var internalClaims = new List<Claim>
            {
                new Claim(nameof(AppClaimType.AppAccessClaimType), nameof(AppInternalPermission.AppAccess)),
                new Claim(nameof(AppClaimType.AppUserIdClaimType), userId.ToString())
            };

            var user = await entityRepository.GetUserWithRolesAndPermissionsAsync(userId, logger);

            if (user.AiepId.HasValue && user.AiepId != 0)
            {
                internalClaims.Add(new Claim(nameof(AppClaimType.AppUserAiepId), user.AiepId.ToString()));
            }
            else
            {
                internalClaims.Add(new Claim(nameof(AppClaimType.AppUserAiepId), "-1"));
            }

            if (user.CurrentAiepId.HasValue && user.CurrentAiepId != 0)
            {
                internalClaims.Add(
                    new Claim(nameof(AppClaimType.AppUserCurrentAiepId), user.CurrentAiepId.ToString()));
            }
            else
            {
                internalClaims.Add(new Claim(nameof(AppClaimType.AppUserCurrentAiepId), "-1"));
            }

            internalClaims.Add(new Claim(nameof(AppClaimType.AppUserFullAclAccess), user.FullAclAccess.ToString()));
            var permissions = user.UserRoles.SelectMany(ur => ur.Role.PermissionRoles.Select(pr => pr.Permission));
            var roles = user.UserRoles.Select(ur => ur.Role);

            internalClaims.AddRange(permissions.Select(permission =>
                new Claim(nameof(AppClaimType.AppPermission), permission.Name)));

            internalClaims.AddRange(roles.Select(role => new Claim(nameof(AppClaimType.AppRole), role.Name)));

            logger.LogDebug("UserService end call GetInernalClaims -> return List of Claim");

            return internalClaims;
        }

        #endregion

    }

}
