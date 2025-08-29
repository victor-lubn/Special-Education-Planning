using Koa.Domain.Search.Page;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification;

namespace SpecialEducationPlanning
.Business.Repository
{

    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly ILogger<RoleRepository> logger;
        private readonly IObjectMapper mapper;

        public RoleRepository(ILogger<RoleRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository entityRepository) : base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.mapper = mapper;
            this.logger = logger;
        }

        #region Implements IRoleRepository

        public async Task<RepositoryResponse<IEnumerable<PermissionModel>>> GetPermissionsFromRole(int roleId)
        {
            logger.LogDebug("RoleRepository called GetPermissionsFromRole");

            var repResponse = new RepositoryResponse<IEnumerable<PermissionModel>>();
            var roleEntity = await entityRepositoryKey.GetRoleWithPermissionsAsync(roleId, logger);

            if (roleEntity == null)
            {
                repResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());
                repResponse.ErrorList.Add(typeof(RoleRepository) + "GetPermissions");

                logger.LogDebug("RoleRepository end call GetPermissionsFromRole -> return Repository response Errors Entity not found");

                return repResponse;
            }

            var permissions = roleEntity.PermissionRoles.Select(permissionRole => permissionRole.Permission).ToList();
            var permissionModels = mapper.Map<Permission, PermissionModel>(permissions);
            repResponse.Content = permissionModels;

            logger.LogDebug("RoleRepository end call GetPermissionsFromRole -> return Repository reponse List of PermissionModel");

            return repResponse;
        }

        public async Task<RepositoryResponse<PermissionAssignedAvailableModel>> GetPermissionsAssignedAvailable(int roleId)
        {
            logger.LogDebug("RoleRepository called GetPermissionsAssignedAvailable");

            var repositoryResponse = new RepositoryResponse<PermissionAssignedAvailableModel>();

            var roleEntity = await entityRepositoryKey.GetRoleWithPermissionsAsync(roleId, logger);
            if (roleEntity == null)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());
                repositoryResponse.ErrorList.Add(typeof(RoleRepository) + "GetPermissionsAssignedAvailable");

                logger.LogDebug("RoleRepository end call GetPermissionsAssignedAvailable -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }

            var allPermissions = await entityRepositoryKey.GetAll<Permission>().ToListAsync();

            var assignedPermissions = roleEntity.PermissionRoles.Select(permissionRole => permissionRole.Permission);
            var availablePermissions = allPermissions.Except(assignedPermissions);

            var permissionsAssigned = new PermissionAssignedAvailableModel()
            {
                PermissionAssigned = mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionModel>>(assignedPermissions),
                PermissionsAvailable = mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionModel>>(availablePermissions)
            };

            repositoryResponse.Content = permissionsAssigned;

            logger.LogDebug("RoleRepository end call GetPermissionsAssignedAvailable -> return Repository response PermissionAssignedAvailableModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<IEnumerable<PermissionModel>>> GetUserPermissionsAsync(int userId)
        {
            logger.LogDebug("RoleRepository called GetUserPermissionsAsync");

            var repositoryResponse = new RepositoryResponse<IEnumerable<PermissionModel>>();
            var userEntity = await entityRepositoryKey.GetUserWithRolesAndPermissionsAsync(userId, logger);

            if (userEntity == null)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());

                logger.LogDebug("RoleRepository end call GetUserPermissionsAsync -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }

            var userPermissions = userEntity.UserRoles
                .SelectMany(ur => ur.Role.PermissionRoles.Select(pr => pr.Permission)).ToImmutableHashSet();

            var permissionModels = mapper.Map<Permission, PermissionModel>(userPermissions);
            repositoryResponse.Content = permissionModels.ToList();

            logger.LogDebug("RoleRepository end call GetUserPermissionsAsync -> return Repository response List of PermissionModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<IEnumerable<RoleModel>>> GetUserRolesAsync(int userId)
        {
            logger.LogDebug("RoleRepository called GetUserRolesAsync");

            var repositoryResponse = new RepositoryResponse<IEnumerable<RoleModel>>();
            var userEntity = await entityRepositoryKey.GetUserWithRolesAsync(userId, logger);

            if (userEntity == null)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.NoResults.GetDescription());
                repositoryResponse.ErrorList.Add(GetType() + "GetUserRolesAsync");

                logger.LogDebug("RoleRepository end call GetUserRolesAsync -> return Repository response Errors No result");

                return repositoryResponse;
            }

            var roleList = userEntity.UserRoles.Select(userRole => userRole.Role).ToList();
            var roleModels = mapper.Map<Role, RoleModel>(roleList);
            repositoryResponse.Content = roleModels;

            logger.LogDebug("RoleRepository end call GetUserRolesAsync -> return Repository response List of RoleModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponseGeneric> RefreshPermissionListAsync()
        {
            logger.LogDebug("RoleRepository called RefreshPermissionListAsync");

            try
            {
                var newValues = GetEnumValues.GetValues<PermissionType>().Select(nv => nv.GetDescription());
                var oldPermissions = await entityRepositoryKey.GetAll<Permission>().ToListAsync();
                var removePermissions = oldPermissions.Where(ov => newValues.All(nv => nv != ov.Name));
                var addValues = newValues.Where(nv => oldPermissions.All(op => op.Name != nv));
                UnitOfWork.BeginTransaction();

                foreach (var valueToAdd in addValues)
                {
                    entityRepositoryKey.Add(new Permission { Name = valueToAdd, DescriptionCode = valueToAdd });
                    UnitOfWork.Commit();
                }

                foreach (var removePermission in removePermissions)
                {
                    entityRepositoryKey.Remove(removePermission);
                    UnitOfWork.Commit();
                }
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();

                throw;
            }

            logger.LogDebug("RoleRepository end call RefreshPermissionsListAsync -> return Repository response generic");

            return new RepositoryResponseGeneric();
        }

        public async Task<RepositoryResponse<RoleModel>> SetPermissions(RolePermissionModel roleModel)
        {
            logger.LogDebug("RoleRepository called SetPermissions");

            var roleNameEntity = await entityRepositoryKey.Where(new RoleByNameSpecification(roleModel.Name))
                .FirstOrDefaultAsync();

            if (roleNameEntity.IsNotNull())
            {
                logger.LogError("{type}#{Role} already exists", typeof(RoleModel), roleModel.Name);

                logger.LogDebug(
                    "RoleRepository end call SetPermissions -> return Repository response Errors Entity already exist");

                return new RepositoryResponse<RoleModel>(null, ErrorCode.EntityAlreadyExist, "Role already exists");
            }

            try
            {
                await UnitOfWork.BeginTransactionAsync();

                var model = new RoleModel() { Name = roleModel.Name };

                var role = entityRepositoryKey.CreateRole(model, roleModel.Permissions, mapper, logger);

                await this.Add(role);

                logger.LogDebug("RoleRepository SetPermissions call Commit");

                await UnitOfWork.CommitAsync();

                logger.LogDebug("RoleRepository end call SetPermissions -> return Repository response RoleModel");

                return new RepositoryResponse<RoleModel>(mapper.Map<Role, RoleModel>(role));
            }
            catch (Exception)
            {
                await UnitOfWork.RollbackAsync();

                logger.LogDebug("RoleRepository end call SetPermissions -> exception");

                throw;
            }
        }

        public async Task<RepositoryResponseGeneric> UpdatePermissions(RoleModel roleModel, IEnumerable<int> queriedPermissionsIds)
        {
            logger.LogDebug("RoleRepository called UpdatePermissions");

            var repositoryResponse = new RepositoryResponseGeneric();

            var roleNameEntity = await entityRepositoryKey.Where(new RoleByNameAndDifferentIdSpecification(roleModel.Name, roleModel.Id)).FirstOrDefaultAsync();
            if (roleNameEntity != null)
            {
                logger.LogError("{type}#{Role} already exists", typeof(RoleModel), roleModel.Name);
                repositoryResponse.ErrorList.Add(ErrorCode.EntityAlreadyExist.ToString());
                repositoryResponse.ErrorList.Add(typeof(RoleRepository) + "UpdatePermissions");

                logger.LogDebug("RoleRepository end call UpdatePermissions -> return Repository response Errors Entity already exist");

                return repositoryResponse;
            }

            var roleEntity = await entityRepositoryKey.GetRoleWithPermissionsAsync(roleModel.Id, logger);

            if (roleEntity == null)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());
                repositoryResponse.ErrorList.Add(typeof(RoleRepository) + "SetPermissions");

                logger.LogDebug("RoleRepository end call UpdatePermissions -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }

            var queriedPermissions = entityRepositoryKey.Where(new PermissionByIdsSpecification(queriedPermissionsIds)).ToList();

            if (queriedPermissions.Count() != queriedPermissionsIds.Count())
            {
                repositoryResponse.ErrorList.Add(ErrorCode.NoResults.ToString());
                repositoryResponse.ErrorList.Add(GetType() + " SetRolePermissions" + "PermissionsId");

                logger.LogDebug("RoleRepository end call UpdatePermissions -> return Repository response Errors No results");

                return repositoryResponse;
            }
            try
            {
                var previousPermissions = roleEntity.PermissionRoles;

                var permissionsRolesToRemoveList =
                    previousPermissions.Where(previous => queriedPermissions.All(queried => previous.PermissionId != queried.Id)).ToList();

                var permissionsToAddList =
                    queriedPermissions.Where(queried => previousPermissions.All(previous => previous.PermissionId != queried.Id)).ToList();

                var permissionRolesList = roleEntity.PermissionRoles.ToList();

                // permissionRolesList.RemoveAll(permissionRole => permissionsToRemoveList.Any(permission => permission.Permission.Id == permissionRole.PermissionId));
                UnitOfWork.BeginTransaction();
                foreach (var item in permissionsRolesToRemoveList)
                {
                    entityRepositoryKey.Remove(item);
                }

                permissionRolesList.AddRange(permissionsToAddList.Select(permissionAdd =>
                    new PermissionRole { PermissionId = permissionAdd.Id, RoleId = roleEntity.Id }));

                roleEntity.Name = roleModel.Name;
                roleEntity.PermissionRoles = permissionRolesList;

                logger.LogDebug("RoleRepository UpdatePermissions call Commit");

                UnitOfWork.Commit();
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("RoleRepository end call UpdatePermissions -> exception");

                throw;
            }

            logger.LogDebug("RoleRepository end call UpdatePermissions -> return Repository response generic");

            return repositoryResponse;
        }

        public async Task<RepositoryResponseGeneric> SetUserRolesAsync(int userId, IEnumerable<int> queriedRolesIds)
        {
            logger.LogDebug("RoleRepository called SetUserRolesAsync");

            var repositoryResponse = new RepositoryResponseGeneric();
            var userEntity = await entityRepositoryKey.GetUserWithRolesAsync(userId, logger);

            if (userEntity == null)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());
                repositoryResponse.ErrorList.Add(typeof(RoleRepository) + "SetUserRolesAsync");

                logger.LogDebug("RoleRepository end call SetUserRolesAsync -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }

            var queriedRoles = entityRepositoryKey.Where(new PermissionByIdsSpecification(queriedRolesIds));

            if (queriedRoles.Count() != queriedRolesIds.Count())
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());
                repositoryResponse.ErrorList.Add(GetType() + " SetUserRolesAsync");

                logger.LogDebug("RoleRepository end call SetUserRolesAsync -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }

            var previousRoles = userEntity.UserRoles.Select(permissionRole => permissionRole.Role).ToList();

            var roleToRemoveList =
                previousRoles.Where(previous => queriedRoles.All(queried => previous.Id != queried.Id));

            var rolesToAddList =
                queriedRoles.Where(queried => previousRoles.All(previous => previous.Id != queried.Id));

            var userRolesList = userEntity.UserRoles.ToList();
            userRolesList.RemoveAll(permissionRole => roleToRemoveList.Any(role => role.Id == permissionRole.RoleId));

            userRolesList.AddRange(rolesToAddList.Select(roleAdd => new UserRole
            { RoleId = roleAdd.Id, UserId = userEntity.Id }));

            try
            {
                UnitOfWork.BeginTransaction();
                userEntity.UserRoles = userRolesList;

                logger.LogDebug("RoleRepository SetUserRolesAsync call Commit");

                UnitOfWork.Commit();
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("RoleRepository end call SetUserRolesAsync -> exception");

                throw;
            }

            logger.LogDebug("RoleRepository end call SetUserRolesAsync -> return Repository response generic");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<RoleModel>> GetRolesByNameAsync(string roleName)
        {
            logger.LogDebug("RoleRepository called GetRolesByNameAsync");

            var repositoryResponse = new RepositoryResponse<RoleModel>();
            var role = await entityRepositoryKey.Where(new RoleByNameSpecification(roleName)).FirstOrDefaultAsync();
            if (role == null)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.ToString());

                logger.LogDebug("RoleRepository end call GetRolesByNameAsync -> return Repository response Errors Entity not found");

                return repositoryResponse;
            }
            repositoryResponse.Content = mapper.Map<Role, RoleModel>(role);

            logger.LogDebug("RoleRepository end call GetRolesByNameAsync -> return Repository response RoleModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<IPagedQueryResult<RoleModel>>> GetRolesFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("RoleRepository called GetRolesFilteredAsync");

            var spec = Specification<Role>.True;

            var modelSpec = SpecificationBuilder.Create<RoleModel>(searchModel.Filters);

            var query = new RoleMaterializedRoleModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = entityRepositoryKey.Query(query);

            logger.LogDebug("RoleRepository end call GetRolesFilteredAsync -> return Paged query RoleModel");

            return new RepositoryResponse<IPagedQueryResult<RoleModel>>(result);
        }
        #endregion

    }

}