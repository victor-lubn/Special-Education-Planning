using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Title Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TitleController : Controller
    {
        private readonly ITitleRepository repository;
        private readonly ILogger<TitleController> logger;
        private readonly IObjectMapper mapper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="logger"></param>
        public TitleController(ITitleRepository repository, ILogger<TitleController> logger, IObjectMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;   
        }

        /// <summary>
        /// Retrieves a list of all available titles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            logger.LogDebug("TitleController called GetAllAsync");

            var response = await repository.GetAllAsync<Title>();
            var model = mapper.Map<Title, TitleModel>(response);
            var repositoryResponse = new RepositoryResponse<IEnumerable<TitleModel>>(model);

            logger.LogDebug("TitleController end call GetAllAsync -> return All title");

            return repositoryResponse.GetHttpResponse();
        }
    }
}