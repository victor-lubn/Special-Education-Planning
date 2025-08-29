using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Area Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AreaController : Controller
    {
        private readonly ILogger<AreaController> logger;
        private readonly IObjectMapper mapper;
        private readonly IAreaRepository repository;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository">Area Repository</param>
        /// <param name="logger">Logger</param>
        public AreaController(
            IAreaRepository repository,
            ILogger<AreaController> logger,
            IObjectMapper mapper
        )
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Deletes an Area
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>OkResult if the Area is deleted.  In case the Area doesn't exist, a NotFoundResult</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("AreaController called Delete -> Deletes Area");

            //TODO Refactor specification
            var model = await repository.FindOneAsync<Area>(id);
            if (model == null)
            {
                logger.LogDebug("No area found");

                logger.LogDebug("AreaController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("AreaController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get Area by ID
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>If the ID is found returns Area Model, if not, it returns a NotFoundResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("AreaController called Get -> Get area");

            var entity = await repository.FindOneAsync<Area>(id);
            var model = mapper.Map<Area, AreaModel>(entity);
            var repositoryResponse = new RepositoryResponse<AreaModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No area found");

                logger.LogDebug("AreaController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("AreaController end call Get -> return Area");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///    Get all Areas
        /// </summary>
        /// <returns>Collection of all Areas</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("AreaController called GetAll -> Gets all areas");
            IEnumerable<Area> areaEntities = await repository.GetAllAsync<Area>();
            IEnumerable<AreaModel> areaModels = mapper.Map<IEnumerable<Area>, IEnumerable<AreaModel>>(areaEntities);

            var repositoryResponse = new RepositoryResponse<IEnumerable<AreaModel>>
            {
                Content = areaModels
            };

            logger.LogDebug("AreaController end call GetAll -> return List empty or not");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all areas filtered for pagination
        /// </summary>
        /// <param name="searchModel">PageDescriptorSearchModel</param>
        /// <returns>JSON with all areas with a paged query</returns>
        [HttpPost]
        [Route("GetAreasFiltered")]
        public async Task<IActionResult> GetAreasFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("AreaController called GetAreasFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("AreaController end call GetAreasFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await repository.GetAreasFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("AreaController end call GetAreasFiltered -> return List Error");

                return models.GetHttpResponse();
            }

            logger.LogDebug("AreaController end call GetAreasFiltered -> return Paged areas");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Creates an Area
        /// </summary>
        /// <param name="value">AreaModel</param>
        /// <returns>Returns the Country created, else, the error why it is not created</returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Post([FromBody] AreaModel value)
        {
            logger.LogDebug("AreaController called Post -> Create area");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("AreaController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = new RepositoryResponse<AreaModel>();
            try
            {
                var areaEntity = mapper.Map<AreaModel,Area>(value);
                var result = await repository.Add(areaEntity);
                var areaModel = mapper.Map<Area, AreaModel>(result);
                repositoryResponse.Content = areaModel;
            }
            catch
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityAlreadyExist.GetDescription());
            }

            logger.LogDebug("AreaController end Call Post -> return Area created");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates an Area with Aieps associated to it
        /// </summary>
        /// <param name="value">AreaModel</param>
        /// <returns>Returns the Country created, else, the error why it is not created</returns>
        [HttpPost("CreateAreaWithAiepIds")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> PostWithAiepIds([FromBody] AreaDtoModel value)
        {
            logger.LogDebug("AreaController called PostWithAiepIds -> Create area with Aiep");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("AreaController end call PostWithAiepIds -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = await repository.SaveArea(value);

            logger.LogDebug("AreaController end call PostWithAiepIds -> return Area created");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates an Area
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="value">Updated AreaModel</param>
        /// <returns>Returns the Area updated, if not, an error depending if the Area is not found by its ID or an error</returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] AreaModel value)
        {
            logger.LogDebug("AreaController called Put -> Update area");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("AreaController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            Area area = await repository.FindOneAsync<Area>(id);

            if (area is null)
            {
                logger.LogDebug("No area found");

                logger.LogDebug("AreaController end call Put -> return Not found");

                return NotFound();
            }

            var repositoryResponse = new RepositoryResponse<AreaModel>();
            try
            {
                area = mapper.Map<AreaModel, Area>(value, area);
                Area result = await repository.ApplyChangesAsync(area);
                var areaModel = mapper.Map<Area, AreaModel>(result);
                repositoryResponse.Content = areaModel;
            }
            catch (Exception)
            {
                repositoryResponse.ErrorList.Add(ErrorCode.EntityAlreadyExist.GetDescription());
            }

            logger.LogDebug("AreaController end call Put -> return Area updated");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates an Area with the existing Aieps
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="value">AreaModel</param>
        /// <returns>Returns the Area created, else, the error why it is not created</returns>
        [HttpPut("{id}/UpdateAreaAndAiepIds")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> PutWithAiepIds(int id, [FromBody] AreaDtoModel value)
        {
            logger.LogDebug("AreaController called PutWithAiepIds -> Updates an area with Aiep");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("AreaController end call PutWithAiepIds -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            if (!await repository.CheckIfExistsAsync(id))
            {
                logger.LogDebug("No area found");

                logger.LogDebug("AreaController end call PutWithAiepIds -> return Not found");

                return NotFound();
            }

            value.Id = id;

            var repositoryResponse = await repository.SaveArea(value);

            logger.LogDebug("AreaController end call PutWithAiepIds -> return Area updated");

            return repositoryResponse.GetHttpResponse();
        }
    }
}
