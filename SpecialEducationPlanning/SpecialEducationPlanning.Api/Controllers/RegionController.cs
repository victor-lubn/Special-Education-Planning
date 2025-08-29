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
    ///     Region controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RegionController : Controller
    {
        private readonly IRegionRepository repository;
        private readonly ILogger<RegionController> logger;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="logger">Logger</param>
        public RegionController(IRegionRepository repository, ILogger<RegionController> logger, IObjectMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Deletes a region
        /// </summary>
        /// <param name="id">Region ID</param>
        /// <returns>OkResult if the Region is deleted.  In case the Region is not, a NotFoundResult</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("RegionController called Delete");

            //TODO Refactor specification
            var entity = await repository.FindOneAsync<Region>(id);
            var model = mapper.Map<Region, RegionModel>(entity);
            if (model == null)
            {
                logger.LogDebug("No region found");

                logger.LogDebug("RegionController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("RegionController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///    Return Area by ID
        /// </summary>
        /// <param name="id">Region ID</param>
        /// <returns>If the ID is found returns Region Model if not, it returns a NotFoundResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("RegionController called Get");

            var entity = await repository.FindOneAsync<Region>(id);
            var model = mapper.Map<Region, RegionModel>(entity);
            var repositoryResponse = new RepositoryResponse<RegionModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No region found");

                logger.LogDebug("RegionController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("RegionController end call Get -> return Region");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all Regions
        /// </summary>
        /// <returns>Collection of Regions</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("RegionController called GetAll");

            var entityList = await repository.GetAllAsync<Region>();
            var modelList = mapper.Map<IEnumerable<Region>, IEnumerable<RegionModel>>(entityList);
            var repositoryResponse = new RepositoryResponse<IEnumerable<RegionModel>>
            {
                Content = modelList
            };

            logger.LogDebug("RegionController end call GetAll -> return All regions");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Country</returns>
        [HttpGet("GetWithNavigations/{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> GetWithChildrens(int id)
        {
            logger.LogDebug("RegionController called GetWithChildrens");
            var entityResponse = await repository.GetWithNavigationsAsync<Region>(id);
            var model = mapper.Map<Region, RegionModel>(entityResponse);

            var repositoryResponse = new RepositoryResponse<RegionModel>
            {
                Content = model
            };

            logger.LogDebug("RegionController end call GetWithChildrens -> return Region with children");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// Get all regions filtered for pagination
        /// </summary>
        /// <param name="searchModel">PageDescriptorSearchModel</param>
        /// <returns>JSON with all regions with a paged query</returns>
        [HttpPost]
        [Route("GetRegionsFiltered")]
        public async Task<IActionResult> GetRegionsFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("RegionController called GetRegionsFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("RegionController end call GetRegionsFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }
            var models = await repository.GetRegionsFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("RegionController end call GetRegionsFiltered -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("RegionController end call GetRegionsFiltered -> return Paged regions");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Creates a Region
        /// </summary>
        /// <param name="value">RegionModel</param>
        /// <returns>Returns the Region created, else, the error why it is not created</returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Post([FromBody] RegionModel value)
        {
            logger.LogDebug("RegionController called Post -> Create region");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("RegionController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var repositoryResponse = await repository.GetDuplicatedRegion(value);
            if (!repositoryResponse.ErrorList.Any())
            {
                var region = await repository.FindOneAsync<Region>(value.Id);
                if (region.IsNull())
                {
                    region = mapper.Map(value, region);
                    await repository.Add(region);
                }
                else
                {
                    region = mapper.Map(value, region);
                    await repository.ApplyChangesAsync(region);
                }

                var regionModel = mapper.Map<Region, RegionModel>(region);
                repositoryResponse.Content = regionModel;
            }

            logger.LogDebug("RegionController end call Post -> return Created region");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates a Region
        /// </summary>
        /// <param name="id">Region ID</param>
        /// <param name="value">Updated RegionModel</param>
        /// <returns>Returns the Region updated, if not, an error depending if the Region is not found by its ID or an error</returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] RegionModel value)
        {
            logger.LogDebug("RegionController called Put -> Update region");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("RegionController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            Region region = await repository.FindOneAsync<Region>(id);

            if (region.IsNull())
            {
                logger.LogDebug("No region found");

                logger.LogDebug("RegionController end call Put -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await repository.GetDuplicatedRegion(value);

            if (!repositoryResponse.ErrorList.Any())
            {
                region = mapper.Map(value, region);
                await repository.ApplyChangesAsync(region);
                var regionModel = mapper.Map<Region, RegionModel>(region);
                repositoryResponse.Content = regionModel;
            }

            logger.LogDebug("RegionController end call Put -> Updated region");

            return repositoryResponse.GetHttpResponse();
        }
    }
}