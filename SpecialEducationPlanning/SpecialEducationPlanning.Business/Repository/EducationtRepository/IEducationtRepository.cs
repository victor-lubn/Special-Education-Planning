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

    public interface IAiepRepository : IBaseRepository<Aiep>
    {

        #region Methods Public

        Task<RepositoryResponse<bool>> CheckBuilderInAiepAsync(int AiepId, int builderId);

        Task<RepositoryResponse<IEnumerable<int>>> GetAllIdsIgnoreAcl();

        Task<RepositoryResponse<AiepModel>> GetAiepByCode(string AiepCode, bool detached = false);

        Task<RepositoryResponse<bool>> UpdateAiepAclByAiepCodeAsync(string AiepCode);

        Task<RepositoryResponse<ICollection<ProjectModel>>> GetAiepProjectsAsync(int AiepId);

        Task<RepositoryResponse<bool>> UpdateAllAiepAclAsync();

        Task<RepositoryResponse<IPagedQueryResult<AiepModel>>> GetAiepsFilteredAsync(IPageDescriptor searchModel);

        Task<RepositoryResponse<AiepAreaModel>> GetAllAiepsByAreaAsync(int areaId);

        Task<AiepModel> CreateUpdateAiepAsync(AiepModel AiepModel);

        Task<RepositoryResponse<bool>> UpdateAiepAclAsync(int AiepId);

        Task<RepositoryResponse<AiepModel>> GetAiepByIdIgnoreAclAsync(int AiepId);

        #endregion
    }

}
