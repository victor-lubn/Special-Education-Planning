using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<RepositoryResponse<IEnumerable<PermissionModel>>> GetPermissionsFromRole(int roleId);
        Task<RepositoryResponse<PermissionAssignedAvailableModel>> GetPermissionsAssignedAvailable(int roleId);
        Task<RepositoryResponse<RoleModel>> SetPermissions(RolePermissionModel rolePermission);
        Task<RepositoryResponseGeneric> UpdatePermissions(RoleModel roleModel, IEnumerable<int> queriedPermissionsIds);

        Task<RepositoryResponse<IEnumerable<RoleModel>>> GetUserRolesAsync(int userId);

        Task<RepositoryResponse<IEnumerable<PermissionModel>>> GetUserPermissionsAsync(int userId);

        Task<RepositoryResponseGeneric> SetUserRolesAsync(int userId, IEnumerable<int> roleIds);

        Task<RepositoryResponseGeneric> RefreshPermissionListAsync();

        Task<RepositoryResponse<RoleModel>> GetRolesByNameAsync(string roleName);

        Task<RepositoryResponse<IPagedQueryResult<RoleModel>>> GetRolesFilteredAsync(IPageDescriptor searchModel);
    }
}