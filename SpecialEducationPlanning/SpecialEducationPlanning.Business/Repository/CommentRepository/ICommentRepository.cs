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
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<RepositoryResponse<IEnumerable<CommentModel>>> GetModelComments<T>(int id);
        Task<RepositoryResponse<CommentModel>> CreateComment<T>(CommentModel commentModel, int modelId, string userInfo);
        Task<RepositoryResponse<CommentModel>> UpdateComment<T>(CommentModel commentModel, string userUniqueIdentifier);
    }
}
