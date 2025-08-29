using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Api.Service.CsvFile.Implementation
{
    /// <summary>
    /// Class for Aiep entity works
    /// </summary>

    public class AiepCsv : ACsv<AiepCsvModel>
    {
        private readonly string ENTITY = nameof(Aiep);
        private readonly IPostCodeServiceFactory postCodeServiceFactory;

        public AiepCsv(IPostCodeServiceFactory postCodeServiceFactory, ICsvFileRepository repository) : base(repository)
        {
            this.postCodeServiceFactory = postCodeServiceFactory;

        }
        public override Task<RepositoryResponse<int>> DoBusiness(IEnumerable<AiepCsvModel> records)
        {
            records = SetRecords(records);
            var response = repository.InsertAieps(records);
            return response;
        }

        public override bool IsEntity(string entity)
        {
            return entity.Equals(this.ENTITY);
        }

        private IEnumerable<AiepCsvModel> SetRecords(IEnumerable<AiepCsvModel> records)
        {
            var postCodeService = postCodeServiceFactory.GetService(null);
            foreach (var Aiep in records)
            {
                string normalisedPostCode = postCodeService.RepresentPostcode(Aiep.Postcode);
                if (!postCodeService.IsValidPostcode(normalisedPostCode))
                {
                    Aiep.Postcode = "N/P";
                }
            }
            return records;
        }
    }
}

