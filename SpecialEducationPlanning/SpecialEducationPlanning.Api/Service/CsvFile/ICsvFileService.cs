using Koa.Hosting.AspNetCore.Model;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.CsvFile
{
    public interface ICsvFileService
    {
        Task<RepositoryResponse<int>> DumpCsv(string entity, MultiUploadedFileModel fileUpload);
    }
}
