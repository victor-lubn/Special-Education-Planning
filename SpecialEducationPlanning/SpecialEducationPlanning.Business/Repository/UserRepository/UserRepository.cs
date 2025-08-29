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
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.PlanSpecifications;
using SpecialEducationPlanning
.Domain.Specification.UserReleaseInfoSpecifications;
using SpecialEducationPlanning
.Domain.Specification.UserRoleSpecifications;
using SpecialEducationPlanning
.Domain.Specification.UserSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly ILogger<UserRepository> logger;
        private readonly IAzureSearchManagementService azureSearchManagementService;
        private readonly IDbContextAccessor dbContextAccessor;
        private readonly IObjectMapper mapper;

        public UserRepository(ILogger<UserRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, IAzureSearchManagementService azureSearchManagementService, ISpecificationBuilder specificationBuilder, IEntityRepository entityRepository) :
base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.logger = logger;
            this.azureSearchManagementService = azureSearchManagementService;
            this.dbContextAccessor = dbContextAccessor;
            this.mapper = mapper;
        }

        #region Methods IUserRepository

        public async Task<RepositoryResponse<UserModel>> GetUserByIdAsync(int userId)
        {
            logger.LogDebug("UserRepository called GetUserByIdAsync");

            var userEntity = await base.FindOneAsync<User>(userId);

            logger.LogDebug("UserRepository end call GetUserByIdAsync -> return Repository response UserModel or Errors Entity not found");

            return userEntity == null
                ? new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound, "UserRepository: GetUserByIdAsync")
                : new RepositoryResponse<UserModel>(mapper.Map<User, UserModel>(userEntity));
        }

        /// <inheritdoc/>
        public async Task<RepositoryResponse<UserModel>> GetUserByEmailAsync(string email)
        {
            if (!email.IsNullOrEmpty())
            {
                UserByUniqueIdentifierSpecification spec = new(email);
                User user = await entityRepositoryKey.GetAll<User>().Where(spec).FirstOrDefaultAsync();

                if (user is null)
                {
                    return new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound);
                }

                UserModel result = mapper.Map<User, UserModel>(user);
                return new RepositoryResponse<UserModel>(result);
            }

            return new RepositoryResponse<UserModel>(null, ErrorCode.NullOrWhitespace);
        }

        public async Task<RepositoryResponse<UserWithRoleModel>> GetUserWithRolesAsync(int userId)
        {
            logger.LogDebug("UserRepository called GetUserWithRolesAsync");

            var userEntity = await  base.Repository.GetUserWithRolesAsync(userId, logger);

            logger.LogDebug("UserRepository end call GetUserWithRolesAsync -> return Repository response UserWithRoleModel or Errors Entity not found");

            return userEntity == null
                ? new RepositoryResponse<UserWithRoleModel>(null, ErrorCode.EntityNotFound, "UserRepository: GetUserWithRolesAsync")
                : new RepositoryResponse<UserWithRoleModel>(mapper.Map<User, UserWithRoleModel>(userEntity));
        }

        public async Task<RepositoryResponse<object>> SetUserCurrentAiepId(int userId, int? AiepId)
        {
            logger.LogDebug("UserRepository called SetUserCurrentAiepId");

            var userEntity = await entityRepositoryKey.Where(new EntityByIdSpecification<User>(userId))
                .FirstOrDefaultAsync();

            if (userEntity.IsNull())
            {
                logger.LogDebug("UserRepository end call SetUserCurrentAiepId -> return Repository response Errors Entity not found User");

                return new RepositoryResponse<object>(null, ErrorCode.EntityNotFound, "User not found.");
            }

            if (AiepId.HasValue)
            {
                var AiepEntity = await entityRepositoryKey.Where(new EntityByIdSpecification<Aiep>(AiepId.Value))
                    .FirstOrDefaultAsync();

                if (AiepEntity.IsNull())
                {
                    logger.LogDebug("UserRepository end call SetUserCurrentAiepId -> return Repository response Errors Entity not found Aiep");

                    return new RepositoryResponse<object>(null, ErrorCode.EntityNotFound,
                        "Aiep not found or user do not have access to it.");
                }
            }

            UnitOfWork.BeginTransaction();
            userEntity.CurrentAiepId = AiepId;

            logger.LogDebug("UserRepository SetUserCurrentAiepId call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("UserRepository end call SetCurrentUserAiep -> return Repository response Object");

            return new RepositoryResponse<object>();
        }

        public async Task<RepositoryResponse<int>> CreateUserFromCsvModel(IEnumerable<UserCsvModel> records)
        {
            logger.LogDebug("UserRepository called CreateUserFromCsvModel");

            var repositoryResponse = new RepositoryResponse<int>();
            int userCount = 0;
            UnitOfWork.BeginTransaction();
            List<string> usersAdded = new List<string>();
            foreach (UserCsvModel user in records)
            {
                if (await CheckUserIdentifierExists(user.userprincipalName, usersAdded))
                {
                    repositoryResponse.AddError(ErrorCode.EntityAlreadyExist, user.userprincipalName);
                }
                else
                {
                    if (user.departmentNumber.IsNull())
                    {
                        repositoryResponse.AddError(ErrorCode.UndefinedAiep, "Aiep is null");
                    }
                    var Aiep = await entityRepositoryKey.GetAiepByAiepCode(user.departmentNumber, logger);
                    if (Aiep.IsNull())
                    {
                        repositoryResponse.AddError(ErrorCode.UndefinedAiep, "Aiep is null");
                    }
                    User newUser = FillUser(user, Aiep);
                    await SetRole(user);
                    var userEntity = entityRepositoryKey.Add(newUser);
                    var role = await entityRepositoryKey.GetRoleByName(user.role, logger);
                    entityRepositoryKey.Add<UserRole>(new UserRole { RoleId = role.Id, UserId = userEntity.Id });
                    usersAdded.Add(newUser.UniqueIdentifier);
                    repositoryResponse.Content = ++userCount;
                }
            }
            if (repositoryResponse.ErrorList.Count == 0)
                UnitOfWork.Commit();
            else
                UnitOfWork.Rollback();

            logger.LogDebug("UserRepository end call CreateUserFromCsvModel -> return Repository response Int");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<UserModel>> CreateUserWithRole(UserModel userModel, int roleId)
        {
            //var role = await repository.FindOneAsync<Role>(roleId.Value);
            //if (role.IsNull())
            //{
            //    logger.LogError("{type}#{role} not found", typeof(RoleModel), roleId);
            //    return new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound, "Role not Found");
            //}

            logger.LogDebug("UserRepository called CreateUserWithRole");

            var AiepExist = await repository.AnyAsync(new EntityByIdSpecification<Aiep>(userModel.AiepId.Value));
            if (!AiepExist)
            {
                logger.LogError("{type}#{Aiep} not found", typeof(AiepModel), userModel.AiepId.Value);

                logger.LogDebug("UserRepository end call CreateUserWithRole -> return Repository response Errors Entity not found");

                return new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound, "Aiep not Found");
            }

            var userExists = await repository.Where(new UserByUniqueIdentifierSpecification(userModel.UniqueIdentifier))
               .IgnoreQueryFilters().FirstOrDefaultAsync();
            if (userExists.IsNotNull())
            {
                logger.LogError("{type}#{user} already exists", typeof(UserModel), userModel.UniqueIdentifier);

                logger.LogDebug("UserRepository end call CreateUserWithRole -> return Repository response Errors Entity already exists");

                return new RepositoryResponse<UserModel>(null, ErrorCode.EntityAlreadyExist, "User already exists");
            }

            try
            {
                UnitOfWork.BeginTransaction();
                User user;
                if (roleId != 0)
                {
                    var role = await base.FindOneAsync<Role>(roleId);
                    if (role.IsNull())
                    {
                        logger.LogError("{type}#{role} not found", typeof(RoleModel), roleId);

                        logger.LogDebug("UserRepository end call CreateUserWithRole -> return Repository response Errors Entity not found Role");

                        return new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound, "Role not Found");
                    }

                    user = await entityRepositoryKey.CreateUpdateUserAsync(userModel, role, mapper, logger);
                }
                else
                {
                    user = mapper.Map<UserModel, User>(userModel);

                }
                repository.Add(user);

                logger.LogDebug("UserRepository CreateUserWithRole call Commit");

                UnitOfWork.Commit();

                logger.LogDebug("UserRepository end call CreateUserWithRole -> return Repository response UserModel");

                return new RepositoryResponse<UserModel>(mapper.Map<User, UserModel>(user));
            }
            catch
            {
                UnitOfWork.Rollback();

                logger.LogDebug("UserRepository end call CreateUserWithRole -> exception");

                throw;
            }
        }

        public async Task<RepositoryResponse<UserModel>> UpdateUserWithRole(UserModel userModel, int roleId)
        {
            logger.LogDebug("UserRepository called UpdateUserWithRole");

            var user = await base.FindOneAsync<User>(userModel.Id);
            if (user.IsNull())  
            {
                logger.LogError("{type}#{user} not found", typeof(UserModel), userModel.Id);

                logger.LogDebug("UserRepository end call UpdateUserWithRole -> return Repository response Errors Entity not found User");

                return new RepositoryResponse<UserModel>(ErrorCode.EntityNotFound.GetDescription());
            }

            var role = await base.FindOneAsync<Role>(roleId);
            if (role.IsNull())
            {
                logger.LogError("{type}#{role} not found", typeof(RoleModel), roleId);
                return new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound, "Role not Found");
            }

            var AiepExist = await repository.AnyAsync(new EntityByIdSpecification<Aiep>(userModel.AiepId.Value));
            if (!AiepExist)
            {
                logger.LogError("{type}#{Aiep} not found", typeof(AiepModel), userModel.AiepId.Value);

                logger.LogDebug("UserRepository end call UpdateUserWithRole -> return Repository response Errors Entity not found Aiep");

                return new RepositoryResponse<UserModel>(null, ErrorCode.EntityNotFound, "Aiep not Found");
            }

            var userExists = await repository.Where(new UserByUniqueIdentifierAndDifferentIdSpecification(userModel.UniqueIdentifier, userModel.Id))
               .IgnoreQueryFilters().FirstOrDefaultAsync();
            if (userExists.IsNotNull())
            {
                logger.LogError("{type}#{user}'s unique identifier already exists", typeof(UserModel), userModel.UniqueIdentifier);

                logger.LogDebug("UserRepository end call UpdateUserWithRole -> return Repository response Errors Entity already exists");

                return new RepositoryResponse<UserModel>(null, ErrorCode.EntityAlreadyExist, "User's unique identifier already exists");
            }

            var currentUserRole = await repository.Where(new GetRoleIdsByUserIdSpecification(userModel.Id))
               .IgnoreQueryFilters().FirstOrDefaultAsync();

            try
            {
                UnitOfWork.BeginTransaction();

                var userUpdate = await base.FindOneAsync<User>(userModel.Id);

                if (roleId != 0)
                {
                    if (currentUserRole.IsNotNull() && currentUserRole.RoleId != roleId)
                    {
                        entityRepositoryKey.Remove<UserRole>(currentUserRole.Id);
                        userUpdate = this.CreateUpdateUser(userUpdate, role);
                    }
                    else
                    {
                        if (role.Name.Equals("Admin") || role.Name.Equals("HubUser"))
                            userModel.FullAclAccess = true;

                        userUpdate = mapper.Map(userModel, userUpdate);
                    }
                }
                else
                {
                    userUpdate = mapper.Map(userModel, userUpdate);
                }

                logger.LogDebug("UserRepository UpdateUserWithRole call Commit");

                UnitOfWork.Commit();

                logger.LogDebug("UserRepository end call UpdateUserWithRole -> return Repository response UserModel");

                return new RepositoryResponse<UserModel>(mapper.Map<User, UserModel>(userUpdate));
            }
            catch
            {
                UnitOfWork.Rollback();

                logger.LogDebug("UserRepository end call UpdateUserWithRole -> exception");

                throw;
            }
        }

        public async Task<RepositoryResponse<bool>> UpdateEducationerAclAsync(int EducationerId)
        {
            logger.LogDebug("UserRepository called UpdateEducationerAclAsync");

            try
            {
                UnitOfWork.BeginTransaction();
                var Educationer = await base.FindOneAsync<User>(EducationerId);

                if (Educationer == null)
                {
                    logger.LogDebug("UserRepository end call UpdateEducationerAclAsync -> return Repository response Entity not found");

                    return new RepositoryResponse<bool>(false, ErrorCode.EntityNotFound);
                }

                if (dbContextAccessor.GetCurrentContext().EducationerUpdateAcl(EducationerId))
                {
                    logger.LogDebug("UserRepository UpdateEducationerAclAsync call Commit");

                    UnitOfWork.Commit();

                    logger.LogDebug("UserRepository end call UpdateEducationerAclAsync -> return Repository response Bool");

                    return new RepositoryResponse<bool>(true);
                }

                UnitOfWork.Rollback();

                logger.LogDebug("UserRepository end call UpdateEducationerAclAsync -> return Repository response Generic controller error");

                return new RepositoryResponse<bool>(false, ErrorCode.GenericControllerError,
                    "repository.UpdateEducationerAclAsync");
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("UserRepository end call UpdateEducationerAclAsync -> return Repository response Commit repository error");

                return new RepositoryResponse<bool>(false, ErrorCode.CommitRepositoryError,
                    e.Message);
            }
        }

        public async Task<RepositoryResponse<IEnumerable<UserWithRoleModel>>> GetAllUsersWithRolesAsync()
        {
            logger.LogDebug("UserRepository called GetAllUsersWithRoleAsync");

            var users = await entityRepositoryKey.GetAllUsersWithRolesAsync(logger);

            var userModels = mapper.Map<IEnumerable<User>, IEnumerable<UserWithRoleModel>>(users);

            logger.LogDebug("UserRepository end call GetAllUsersWithRoleAsync -> return Repository response List of UserWithRoleModel");

            return new RepositoryResponse<IEnumerable<UserWithRoleModel>>(userModels);
        }

        public async Task<RepositoryResponse<IEnumerable<UserWithRoleAndPermissionsModel>>> GetAllUsersWithRolesAndPermissionsAsync()
        {
            logger.LogDebug("UserRepository called GetAllUsersWithRolesAndPermissionsAsync");

            var users = await entityRepositoryKey.GetAllUserWithRolesAndPermissionsAsync(logger);

            var userModels = mapper.Map<IEnumerable<User>, IEnumerable<UserWithRoleAndPermissionsModel>>(users);

            logger.LogDebug("UserRepository end call GetAllUsersWithRoleAndPermssionsAsync -> return Repository response List of UserWithRoleAndPermissionsModel");

            return new RepositoryResponse<IEnumerable<UserWithRoleAndPermissionsModel>>(userModels);
        }

        public async Task<RepositoryResponse<IPagedQueryResult<UserWithRoleModel>>> GetUsersWithRolesFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("UserRepository called GetUserWithRolesFilteredAsync");

            var spec = Specification<User>.True;

            var modelSpec = SpecificationBuilder.Create<UserWithRoleModel>(searchModel.Filters);

            var query = new UserMaterializedUserWithRolesModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("UserRepository end called GetUserWithRolesFilteredAsync -> return Repository response Paged query UserWithRoleModel");

            return new RepositoryResponse<IPagedQueryResult<UserWithRoleModel>>(result);
        }

        public async Task<RepositoryResponse<IEnumerable<UserModel>>> GetAllUsersByAiepId(int AiepId, int userIdEdited)
        {
            logger.LogDebug("UserRepository called GetAllUsersByAiepId");

            var users = await base.Repository.Where(new UserByAiepIdSpecification(AiepId, userIdEdited)).OrderBy(u => u.UniqueIdentifier).ToListAsync();

            var userModels = mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(users);

            logger.LogDebug("UserRepository end call GetAllUsersByAiepId -> return Repository response List of UserModel");

            return new RepositoryResponse<IEnumerable<UserModel>>(userModels);
        }

        public async Task<int> GetNumberofUsersMarkedForDeletion()
        {
            logger.LogDebug("UserRepository called GetNumberofUsersMarkedForDeletion");

            int numberOfLeavers = await base.Repository.GetNumberofUsersMarkedForDeletion(logger);

            logger.LogDebug("UserRepository end call GetNumberofUsersMarkedForDeletion -> return number of Leavers");

            return numberOfLeavers;
        }

        public async Task<RepositoryResponse<IEnumerable<UserModel>>> GetAllUsersByRoleId(int roleId)
        {
            logger.LogDebug("UserRepository called GetAllUsersByRoleId");

            var role = await base.FindOneAsync<Role>(roleId);
            if (role.IsNull())
            {
                logger.LogError("{type}#{role} not found", typeof(RoleModel), roleId);

                logger.LogDebug("UserRepository end call GetAllUsersByRoleId -> return Repository response Errors Entity not found");

                return new RepositoryResponse<IEnumerable<UserModel>>(null, ErrorCode.EntityNotFound, "Role not Found");
            }

            var users = await entityRepositoryKey.GetAllUsersWithRolesAsync(logger);

            var usersWithSpecificRole = new List<User>();
            foreach (var user in users)
            {
                var userRole = user.UserRoles.FirstOrDefault();
                if (userRole != null && userRole.RoleId == roleId)
                {
                    usersWithSpecificRole.Add(user);
                }
            }

            var userModels = mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(usersWithSpecificRole);

            logger.LogDebug("UserRepository end call GetAllUsersByRoleId -> return Repository response List of UserModel");

            return new RepositoryResponse<IEnumerable<UserModel>>(userModels);
        }

        public async Task<RepositoryResponse<bool>> DeleteUserAsync(int id)
        {
            logger.LogDebug("UserRepository called DeleteUserAsync");

            try
            {
                var user = await base.FindOneAsync<User>(id);

                if (user.IsNull())
                {
                    logger.LogError("{type}#{user} not found", typeof(UserModel), id);

                    logger.LogDebug("UserRepository end call DeleteUserAsync -> return Repository response Errors Entity not found");

                    return new RepositoryResponse<bool>(false, ErrorCode.EntityNotFound, "User not found");
                }

                UnitOfWork.BeginTransaction();

                await RemoveEducationerFromPlan(id);
                await RemoveUserReleasesInfo(id);
                await RemoveUserRoles(id);
                await dbContextAccessor.GetCurrentContext().RemoveAllUserAclAsync<Acl>(id);
                entityRepositoryKey.Remove(user);

                logger.LogDebug("UserRepository DeleteUserAsync call Commit");

                UnitOfWork.Commit();

                logger.LogDebug("UserRepository end call DeleteUserAsync return Repository response Bool True");

                return new RepositoryResponse<bool>() { Content = true };
            }
            catch (Exception ex)
            {
                logger.LogError("deleteuserasync", ex);
                UnitOfWork.Rollback();

                logger.LogDebug("UserRepository end call DeleteUserAsync return Repository response Bool False");

                return new RepositoryResponse<bool>() { Content = false };
            }

        }

        public async Task CallIndexerAsync(int take, int skip, DateTime? updateDate, int? indexerWindowInDays)
        {
            logger.LogDebug("UserRepository called CallIndexerAsync");

            var users = await base.Repository.GetEntitiesNoFiltersAsync<User>(take, skip, updateDate, indexerWindowInDays).ToListAsync();

            azureSearchManagementService.MergeOrUploadDocuments
                (azureSearchManagementService.GetDocuments<OmniSearchUserIndexModel, User>(users));

            logger.LogDebug("UserRepository end call CallIndexerAsync");
        }

        public async Task AutomaticDeleteLeavers()
        {
            logger.LogDebug("UserRepository called AutomaticDeleteLeavers");

            UnitOfWork.BeginTransaction();

            dbContextAccessor.GetCurrentContext().DeleteLeavers();

            logger.LogDebug("UserRepository AutomaticDeleteLeavers Commit");

            UnitOfWork.Commit();

            logger.LogDebug("UserRepository end call AutomaticDeleteLeavers");
        }
        #endregion

        #region Private Methods

        private User CreateUpdateUser(User user, Role role)
        {
            var userRole = new UserRole
            {
                User = user,
                RoleId = role.Id
            };
            entityRepositoryKey.Add<UserRole>(userRole);
            if (role.Name.Equals("Admin") || role.Name.Equals("HubUser"))
            {
                user.FullAclAccess = true;
            }
            else
            {
                user.FullAclAccess = false;
            }
            user.UserRoles.Add(userRole);
            return user;
        }

        private async Task<bool> CheckUserIdentifierExists(string uniqueIdentifier, List<string> usersAdded)
        {
            logger.LogDebug("UserRepository called CheckUserIdentifierExists");

            var user = await entityRepositoryKey.Where(new UserByUniqueIdentifierSpecification(uniqueIdentifier)).FirstOrDefaultAsync();
            if (user.IsNull() && !usersAdded.Contains(uniqueIdentifier))
            {
                logger.LogDebug("UserRepository end call CheckUserIdentifierExists -> return False");

                return false;
            }

            logger.LogDebug("UserRepository end call CheckUserIdentifierExists -> return True");

            return true;
        }

        private User FillUser(UserCsvModel user, Aiep Aiep)
        {
            logger.LogDebug("UserRepository called FillUser");

            User newUser = new User
            {
                CreationUser = "manualUserCreate",
                UpdateUser = "manualUserUpdate",
                FullAclAccess = false,
                AiepId = Aiep?.Id,
                FirstName = user.GivenName,
                Surname = user.sn,
                UniqueIdentifier = user.userprincipalName
            };

            logger.LogDebug("UserRepository end call FillUser -> return User");

            return newUser;
        }



        private async Task SetRole(UserCsvModel user)
        {
            logger.LogDebug("UserRepository called SetRole");

            var roles = await entityRepositoryKey.GetAll<Role>().ToListAsync();
            bool found = false;
            foreach (Role role in roles)
            {
                if (user.role.Equals(role.Name))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                user.role = "Educationer";
            }

            logger.LogDebug("UserRepository end call SetRole");
        }

        private async Task RemoveEducationerFromPlan(int id)
        {
            logger.LogDebug("UserRepository RemoveEducationerFromPlan");

            var spec = new PlansByEducationerIdSpecification(id);
            var plans = await entityRepositoryKey.GetAll<Plan>().Where(spec).ToListAsync();

            foreach (var plan in plans)
            {
                plan.EducationerId = null;
            }

            logger.LogDebug("UserRepository end call RemoveEducationerFromPlan");
        }

        private async Task RemoveUserReleasesInfo(int id)
        {
            logger.LogDebug("UserRepository called RemoveUserReleaseInfo");

            var spec = new UserReleaseInfoByUserSpecification(id);
            var userReleasesInfo = await entityRepositoryKey.GetAll<UserReleaseInfo>().Where(spec).ToListAsync();

            foreach (var userReleaseInfo in userReleasesInfo)
            {
                entityRepositoryKey.Remove(userReleaseInfo);
            }

            logger.LogDebug("UserRepository end call RemoveUserReleaseInfo");
        }

        private async Task RemoveUserRoles(int id)
        {
            logger.LogDebug("UserRepository called RemoveUserRoles");

            var spec = new GetRoleIdsByUserIdSpecification(id);
            var userRoles = await entityRepositoryKey.GetAll<UserRole>().Where(spec).ToListAsync();

            foreach (var userRole in userRoles)
            {
                entityRepositoryKey.Remove(userRole);
            }

            logger.LogDebug("UserRepository end call RemoveUserRoles");
        }

        #endregion
    }

}

