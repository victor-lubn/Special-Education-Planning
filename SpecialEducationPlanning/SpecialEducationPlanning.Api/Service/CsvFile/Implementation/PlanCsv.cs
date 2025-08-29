using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Api.Service.CsvFile.Implementation
{
    /// <summary>
    /// Class for Plan entity works
    /// </summary>
    public class PlanCsv : ACsv<String>
    {
        private readonly string ENTITY = nameof(Plan);

        public PlanCsv(ICsvFileRepository repository) : base(repository)
        {

        }

        public override Task<RepositoryResponse<int>> DoBusiness(IEnumerable<string> data)
        {
            throw new NotImplementedException();
        }

        public override bool IsEntity(string entity)
        {
            return entity.Equals(this.ENTITY);
        }
    }
}
