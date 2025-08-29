using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.CsvFile.Implementation
{
    /// <summary>
    /// Abstract class for all the EntityCsv classes
    /// </summary>
    /// <typeparam name="Model"></typeparam>
    public abstract class ACsv<Model> : ICsv
    {
        protected readonly ICsvFileRepository repository;
        protected ACsv(ICsvFileRepository repository)
        {
            this.repository = repository;
        }

        protected async Task<IEnumerable<Model>> ReadCsvFile(Stream data)
        {
            IEnumerable<Model> records;
            using (StreamReader sr = new StreamReader(data))
            {
                var reader = new CsvReader(sr);
                reader.Configuration.IncludePrivateMembers = true;
                string delimiter = ",";
                reader.Configuration.Delimiter = delimiter;
                reader.Configuration.MissingFieldFound = null;
                records = reader.GetRecords<Model>().ToList();
            }

            return records;
        }

        public async Task<RepositoryResponse<int>> LoadCsv(Stream data)
        {
            var response = new RepositoryResponse<int>();
            try
            {
                var result = await this.ReadCsvFile(data);
                response = await this.DoBusiness(result);
                //return await this.DoBusiness(result);
            }
            catch (System.Exception)
            {
                response.AddError(ErrorCode.ArgumentErrorBusiness, "Format error in CSV file");
            }

            return response;

        }

        public abstract Task<RepositoryResponse<int>> DoBusiness(IEnumerable<Model> data);
        public abstract bool IsEntity(string entity);
    }
}
