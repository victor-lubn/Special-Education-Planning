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
.Domain.Specification;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class RoleCore
    {
        public static async Task<Role> GetRoleWithPermissionsAsync(this IEntityRepository<int> entityRepository,
            int roleId, ILogger logger)
        {
            logger.LogDebug("RoleCore called GetRoleWithPermissionsAsync");

            logger.LogDebug("RoleCore end call GetRoleWithPermissionsAsync -> return Role");

            return await entityRepository.Where(new EntityByIdSpecification<Role>(roleId))
                .Include(r => r.PermissionRoles).ThenInclude(r => r.Permission).FirstOrDefaultAsync();
        }

        public static async Task<Role> GetRoleByName(this IEntityRepository<int> entityRepository,
            string roleName, ILogger logger)
        {
            logger.LogDebug("RoleCore called GetRoleByName");

            logger.LogDebug("RoleCore end call GetRoleByName -> return Role");

            return await entityRepository.Where(new RoleByNameSpecification(roleName)).FirstOrDefaultAsync();
        }

        public static Role CreateRole(this IEntityRepository<int> entityRepository,
            RoleModel role, IEnumerable<int> permissions, IObjectMapper mapper, ILogger logger)
        {
            logger.LogDebug("RoleCore called CreateRole");

            var roleEntity = mapper.Map<RoleModel, Role>(role);
            foreach (var permission in permissions)
            {
                var rolePermission = new PermissionRole
                {
                    Role = roleEntity,
                    PermissionId = permission
                };
                roleEntity.PermissionRoles.Add(rolePermission);
            }

            logger.LogDebug("RoleCore end call CreateRole -> return Role");

            return roleEntity;
        }
    }
}