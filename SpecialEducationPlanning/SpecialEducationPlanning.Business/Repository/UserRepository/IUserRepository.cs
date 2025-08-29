using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IUserRepository :IBaseRepository<User>
    {
        Task<RepositoryResponse<UserModel>> GetUserByIdAsync(int userId);

        /// <summary>
        /// Get User by Email (Unique Identifier)
        /// </summary>
        /// <param name="email"></param>
        /// <returns><see cref="UserModel"/></returns>
        Task<RepositoryResponse<UserModel>> GetUserByEmailAsync(string email);

        Task<RepositoryResponse<UserWithRoleModel>> GetUserWithRolesAsync(int userId);

        Task<RepositoryResponse<object>> SetUserCurrentAiepId(int userId, int? AiepId);

        Task<RepositoryResponse<int>> CreateUserFromCsvModel(IEnumerable<UserCsvModel> records);

        Task<RepositoryResponse<UserModel>> CreateUserWithRole(UserModel userModel, int roleId);

        Task<RepositoryResponse<UserModel>> UpdateUserWithRole(UserModel userModel, int roleId);

        Task<RepositoryResponse<IEnumerable<UserWithRoleModel>>> GetAllUsersWithRolesAsync();

        Task<RepositoryResponse<IEnumerable<UserWithRoleAndPermissionsModel>>> GetAllUsersWithRolesAndPermissionsAsync();

        Task<RepositoryResponse<IPagedQueryResult<UserWithRoleModel>>> GetUsersWithRolesFilteredAsync(IPageDescriptor searchModel);

        Task<RepositoryResponse<IEnumerable<UserModel>>> GetAllUsersByRoleId(int roleId);

        Task<RepositoryResponse<IEnumerable<UserModel>>> GetAllUsersByAiepId(int AiepId, int userIdEdited);

        Task<RepositoryResponse<bool>> DeleteUserAsync(int id);

        Task<RepositoryResponse<bool>> UpdateEducationerAclAsync(int EducationerId);

        Task CallIndexerAsync(int take, int skip, DateTime? updateDate, int? indexerWindowInDays);

        Task<int> GetNumberofUsersMarkedForDeletion();

        Task AutomaticDeleteLeavers();
    }
}


