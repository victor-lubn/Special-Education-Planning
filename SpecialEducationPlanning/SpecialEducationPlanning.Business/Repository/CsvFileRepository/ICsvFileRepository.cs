using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface ICsvFileRepository
    {
        Task<RepositoryResponse<int>> InsertUsers(IEnumerable<UserCsvModel> records);
        Task<RepositoryResponse<int>> InsertAieps(IEnumerable<AiepCsvModel> records);
    }
}

