using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.CsvFile.Implementation
{
    /// <summary>
    /// Class for User entity works
    /// </summary>
    public class UserCsv : ACsv<UserCsvModel>
    {
        private readonly string ENTITY = nameof(User);

        public UserCsv(ICsvFileRepository repository) : base(repository)
        {

        }

        public override async Task<RepositoryResponse<int>> DoBusiness(IEnumerable<UserCsvModel> records)
        {
            var response = await repository.InsertUsers(records);
            return response;

        }

        public override bool IsEntity(string entity)
        {
            return entity.Equals(this.ENTITY);
        }
    }
}
