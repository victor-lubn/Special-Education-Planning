using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Aiep Controller
    /// </summary>
    [Route("api/[controller]")]
    public class AiepController : Controller
    {
        private readonly IAiepRepository repository;

        private readonly ILogger<AiepController> logger;

        private readonly IUserService userService;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="logger"></param>
        /// <param name="userService"></param>
        public AiepController(
            IAiepRepository repository,
            ILogger<AiepController> logger,
            IUserService userService,
            IObjectMapper mapper
        )
        {
            this.repository = repository;
            this.logger = logger;
            this.userService = userService;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Deletes a Aiep
        /// </summary>
        /// <param name="id"></param>
        /// <returns>OkResult if the Aiep is deleted.  In case the Aiep doesn't exist, a NotFoundResult</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("AiepController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<Aiep>(id);
            var model = mapper.Map<Aiep, AiepModel>(entity);

            if (model == null)
            {
                logger.LogDebug("No Aiep found");

                logger.LogDebug("AiepController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("AiepController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Check if the Aiep has existing Builder
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns></returns>
        [HttpGet("CurrentHasBuilder/{builderId}")]
        public async Task<IActionResult> CurrentAiepHasBuilder(int builderId)
        {
            logger.LogDebug("AiepController called CurrentAiepHasBuilder");

            var AiepId = userService.GetUserAiepId((ClaimsIdentity)User.Identity);
            var response = await repository.CheckBuilderInAiepAsync(AiepId, builderId);

            logger.LogDebug("AiepController end call CurrentAiepHasBuilder -> return Response (true or false)");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Get Aiep by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>If the ID is found returns Aiep Model, if not, it returns a NotFoundResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("AiepController called Get");

            var entity = await repository.FindOneAsync<Aiep>(id);
            var model = mapper.Map<Aiep, AiepModel>(entity);
            var repositoryResponse = new RepositoryResponse<AiepModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No Aiep found");

                logger.LogDebug("AiepController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("AiepController end call Get -> return Aiep");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///    Get all Aieps
        /// </summary>
        /// <returns>Collection of all Aieps</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("AiepController called GetAll");
            IEnumerable<Aiep> Aieps = await repository.GetAllAsync<Aiep>();
            IEnumerable<AiepModel> AiepModels = mapper.Map<IEnumerable<Aiep>, IEnumerable<AiepModel>>(Aieps);
            var repositoryResponse = new RepositoryResponse<IEnumerable<AiepModel>>
            {
                Content = AiepModels
            };

            logger.LogDebug("AiepController end call GetAll -> return All Aieps");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Search for all Aieps in an Area
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns>Collection of all Aieps assigned and available inside an Area</returns>
        [HttpGet]
        [Route("GetAllAiepsByArea")]
        public async Task<IActionResult> GetAllAiepsByArea(int areaId)
        {
            logger.LogDebug("AiepController called GetAllAiepsByArea");

            var repositoryResponse = await repository.GetAllAiepsByAreaAsync(areaId);

            logger.LogDebug("AiepController end call GetAllAiepsByArea -> return List of Aieps");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all areas filtered for pagination
        /// </summary>
        /// <param name="searchModel">PageDescriptorSearchModel</param>
        /// <returns>JSON with all areas with a paged query</returns>
        [HttpPost]
        [Route("GetAiepsFiltered")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> GetAiepsFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("AiepController called GetAiepsFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("AiepController end call GetAiepsFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await repository.GetAiepsFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("AiepController end call GetAiepsFiltered -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("AiepController end call GetAiepsFiltered -> return Paged Aieps");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Creates an Area
        /// </summary>
        /// <param name="value">AiepModel</param>
        /// <returns>Returns the Aiep created, else, the error why it is not created</returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Post([FromBody] AiepModel value)
        {
            logger.LogDebug("AiepController called Post -> Create area");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("AiepController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryReponse = new RepositoryResponse<AiepModel>
            {
                Content = await repository.CreateUpdateAiepAsync(value)
            };

            logger.LogDebug("AiepController end call Post -> Aiep created");

            return repositoryReponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates a Aiep
        /// </summary>
        /// <param name="id">Aiep ID</param>
        /// <param name="value">Updated AiepModel</param>
        /// <returns>Returns the Aiep updated, if not, an error depending if the Aiep is not found by its ID or an error</returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] AiepModel value)
        {
            logger.LogDebug("AiepController called Put -> Update a Aiep");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("AiepController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (!await repository.CheckIfExistsAsync(id))
            {
                logger.LogDebug("No Aiep found");

                logger.LogDebug("AiepController end call Put -> return Not found");

                return NotFound();
            }

            // If the value.Id is null it will create a new Aiep, therefore we check and set it.
            if (value.Id == 0)
            {
                value.Id = id;
            }

            var repositoryResponse = new RepositoryResponse<AiepModel>
            {
                Content = await repository.CreateUpdateAiepAsync(value)
            };

            logger.LogDebug("AiepController end call Put -> return Updated Aiep");

            return repositoryResponse.GetHttpResponse();
        }
    }

}
