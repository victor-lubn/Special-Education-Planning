using System;
using System.IO;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.CsvFile.Implementation
{
    /// <summary>
    /// Interface for ACsv abstract class
    /// </summary>
    public interface ICsv
    {
        /// <summary>
        /// Method that reads a csv file and inserts its values into DB.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        Task<RepositoryResponse<int>> LoadCsv(Stream data);
        /// <summary>
        /// Returns it is the correct entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsEntity(String entity);
    }
}
