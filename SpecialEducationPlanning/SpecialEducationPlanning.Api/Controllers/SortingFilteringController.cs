
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Controllers
{

    /// <summary>
    ///     Omni Search Controller
    /// </summary>
    [Route("api/[controller]")]
    public class SortingFilteringController : Controller
    {
        private readonly ISortingFilteringRepository repository;
        private readonly ILogger<SortingFilteringController> logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        public SortingFilteringController(ISortingFilteringRepository repository, ILogger<SortingFilteringController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        #region Methods Public
        /// <summary>   
        ///     Retrieve the sorting and filterig options for a specified entity
        /// </summary>
        /// <param name="entityType"></param>
        [HttpGet("{entityType}/GetSortingFilteringOptionsByEntity")]
        public async Task<IActionResult> GetSortingFilteringOptionsByEntity(string entityType)
        {
            logger.LogDebug("SortingFilteringController called GetSortingFilteringOptionsByEntity");

            var repositoryResponse = await repository.GetSortingFilteringOptionsByEntity(entityType);

            logger.LogDebug("SortingFilteringController end call -> return List of sorting filtering options");

            return repositoryResponse.GetHttpResponse();
        }
        #endregion
    }

}