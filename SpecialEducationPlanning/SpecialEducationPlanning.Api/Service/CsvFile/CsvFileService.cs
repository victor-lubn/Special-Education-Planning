using Koa.Hosting.AspNetCore.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.CsvFile.Implementation;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.CsvFile
{
    /// <summary>
    /// Service to upload a csv file
    /// </summary>
    public class CsvFileService : ICsvFileService
    {
        public IEnumerable<ICsv> CsvFactory { get; }
        private readonly ICsvFileRepository Repository;
        private readonly ILogger<CsvFileService> Logger;

        public CsvFileService(IEnumerable<ICsv> CsvFactory, ICsvFileRepository repository, ILogger<CsvFileService> Logger)
        {

            this.CsvFactory = CsvFactory;
            this.Repository = repository;
            this.Logger = Logger;
        }

        /// <summary>
        /// Sets the correct entity for the upload
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<int>> DumpCsv(string entity, MultiUploadedFileModel fileUpload)
        {
            var response = new RepositoryResponse<int>();
            var file = fileUpload.Files.FirstOrDefault();
            if (file.ContentLength == 0)
            {
                Logger.LogError("Error reading file");
                response.AddError(ErrorCode.ArgumentErrorController, "Error reading file");
                return response;
            }
            using (var stream = file.FileStream)
            {
                var csv = this.CsvFactory.Where(_csv => _csv.IsEntity(entity));
                if (csv != null && csv.Count() > 0)
                {
                    var csvInstance = csv.FirstOrDefault();
                    return await csvInstance.LoadCsv(stream);
                }
                else
                {
                    Logger.LogError("Entity do not exist");
                    response.AddError(ErrorCode.EntityNotFound, "Entity do not exist");
                    return response;
                }
            }
        }
    }
}
