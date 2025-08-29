using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IBuilderEducationerAiepRepository :IBaseRepository<BuilderEducationerAiep>
    {
        Task<RepositoryResponse<BuilderEducationerAiepModel>> GetBuilderEducationerAiepModelRelation(int builderId, int AiepId);
        Task<RepositoryResponse<BuilderEducationerAiepModel>> CreateBuilderEducationerAiepModelRelation(int builderId, int AiepId);
        Task<RepositoryResponse<BuilderEducationerAiepModel>> GetBuilderEducationerAiepModelRelationByBuilderId(int builderId);
        Task<RepositoryResponse<BuilderEducationerAiepModel>> GetBuilderEducationerAiepModelByBuilderIdAiepId(int builderId, int AiepId);
    }
}


