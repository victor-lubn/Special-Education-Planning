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
    public interface IEndUserRepository : IBaseRepository<EndUser>
    {
        Task<RepositoryResponse<EndUserModel>> GetEndUserById(int id);
        Task<RepositoryResponse<EndUserModel>> FindExistingEndUser(EndUserModel endUser);
        IEnumerable<EndUserDiffModel> CompareEndUsers(EndUserModel source, EndUserModel target);
        Task<RepositoryResponse<EndUserModel>> GetOrCreateEndUserAssignAiep(EndUserModel endUserModel, int AiepId);
        Task<RepositoryResponse<AiepModel>> GetEndUserLatestUserAiepAsync(int endUserId);

        Task<RepositoryResponse<AiepModel>> GetEndUserOwnOrLatestUserAiepAsync(int endUserId, int AiepId);
        Task<RepositoryResponse<EndUserModel>> GetEndUserByMandatoryFieldsAsync(EndUserModel endUser);
        Task<RepositoryResponseGeneric> EndUserCleanManagment();
        Task CallIndexerAsync(int take, int skip, DateTime? updateDate, int? indexerWindowInDays);
    }
}
