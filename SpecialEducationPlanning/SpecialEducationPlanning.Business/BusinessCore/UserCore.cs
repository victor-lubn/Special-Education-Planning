using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Extensions.DataContextSqlExtensions;
using SpecialEducationPlanning
.Domain.Specification;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class UserCore
    {
        public static async Task<User> GetUserWithRolesAsync(this IEntityRepository<int> entityRepository, int userId,
            ILogger logger)
        {
            logger.LogDebug("UserCore called GetUserWithRoleAsync");

            logger.LogDebug("UserCore end call GetUserWithRoleAsync -> return User");

            return await entityRepository
                .Where<User>(new EntityByIdSpecification<User>(userId))
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role).FirstOrDefaultAsync();
        }

        public static async Task<User> GetUserWithRolesAndPermissionsAsync(this IEntityRepository<int> entityRepository, int userId,
            ILogger logger)
        {
            logger.LogDebug("UserCore called GetUserWithRolesAndPermissionsAsync");

            logger.LogDebug("UserCore end call GetUserWithRolesAndPermissionsAsync -> return User");

            return await entityRepository
                .Where<User>(new EntityByIdSpecification<User>(userId))
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(ur => ur.PermissionRoles)
                .ThenInclude(pr => pr.Permission)
                .FirstOrDefaultAsync();
        }

        public static async Task<User> CreateUpdateUserAsync(this IEntityRepository<int> entityRepository,
            UserModel userModel, Role role, IObjectMapper mapper, ILogger logger)
        {
            logger.LogDebug("UserCore called CreateUpdateUserAsync");

            if (role.Name.Equals("Admin") || role.Name.Equals("HubUser")) userModel.FullAclAccess = true;
            else userModel.FullAclAccess = false;

            var user = mapper.Map<UserModel, User>(userModel);

            var userRole = new UserRole
            {
                User = user,
                RoleId = role.Id
            };

            user.UserRoles.Add(userRole);

            logger.LogDebug("UserCore end call CreateUpdateUserAsync -> return User");

            return user;
        }

        public static async Task<IEnumerable<User>> GetAllUsersWithRolesAsync(this IEntityRepository<int> entityRepository, ILogger logger)
        {
            logger.LogDebug("UserCore called GetAllUsersWithRolesAsync");

            logger.LogDebug("UserCore called GetAllUsersWithRolesAsync -> return List of User");

            return await entityRepository
                .GetAll<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role).ToListAsync();
        }

        public static async Task<IEnumerable<User>> GetAllUserWithRolesAndPermissionsAsync(this IEntityRepository<int> entityRepository,
            ILogger logger)
        {
            logger.LogDebug("UserCore called GetAllUserWithRolesAndPermissionsAsync");

            logger.LogDebug("UserCore called GetAllUserWithRolesAndPermissionsAsync -> return List of User");

            return await entityRepository
                .GetAll<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(ur => ur.PermissionRoles)
                .ThenInclude(pr => pr.Permission).ToListAsync();
        }

        public static async Task<IEnumerable<User>> GetAllUsersByAiepIdAsync(this IEntityRepository<int> entityRepository,
            ILogger logger,int AiepId)
        {
            logger.LogDebug("UserCore called GetAllUsersByAiepIdAsync");

            logger.LogDebug("UserCore called GetAllUsersByAiepIdAsync -> return List of User");

            return await entityRepository
                .Where(new EntityByIdSpecification<User>(AiepId)).ToListAsync();
        }

        public static async Task<int> GetNumberofUsersMarkedForDeletion(this IEntityRepository<int> entityRepository,
           ILogger logger)
        {
            logger.LogDebug("UserCore called GetAllUsersMarkedForDeletionAsync");

            logger.LogDebug("UserCore called GetAllUsersMarkedForDeletionAsync -> return List of Leavers");

            return await entityRepository
                .GetAll<User>().CountAsync(x => x.Leaver && x.DelegateToUserId!=null);
        }

        public static void DeleteLeavers(this DbContext dbContext)
        {
           dbContext.StoreProcedureDeleteLeaversAndUpdatePlans();
        }
    }
}

