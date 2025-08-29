using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Business.Constants;
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
    ///     Catalog Controller
    /// </summary>
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        private readonly ICatalogRepository _repository;
        private readonly ILogger<CatalogController> _logger;
        private readonly IObjectMapper _mapper;
        private readonly IFeatureManagementService _featureManagementService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository"></param>
        public CatalogController(
            ICatalogRepository repository,
            ILogger<CatalogController> logger,
            IObjectMapper mapper,
            IFeatureManagementService featureManagementService)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _featureManagementService = featureManagementService;
        }

        /// <summary>
        ///     Retrieve all catalogs
        /// </summary>
        /// <param name="EducationOrigin"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]
        public async Task<IActionResult> GetAll([FromQuery] string EducationOrigin)
        {
            _logger.LogDebug($"{nameof(CatalogController)} called {nameof(GetAll)}");

            _logger.LogDebug("CatalogController end call GetAll -> return All catalogs");

            if (string.IsNullOrEmpty(EducationOrigin))
            {
                var userIdentity = ((ClaimsIdentity)User.Identity);
                var dvProToolEnabledForUser = await _featureManagementService.GetFeatureFlagAsync(FeatureManagementFlagNames.dvProToolEnabled, userIdentity);

                EducationOrigin = dvProToolEnabledForUser ? EducationOriginType.ThreeDc.GetDescription() : EducationOriginType.Fusion.GetDescription();
            }
            var catalogsResponse = await _repository.GetCatalogs(EducationOrigin);

            return catalogsResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieve a specific catalog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]

        public async Task<IActionResult> Get(int id)
        {
            var repositoryResponse = await _repository.GetCatalogById(id);
            if (repositoryResponse.Content == null)
            {
                _logger.LogDebug("No catalog found");

                _logger.LogDebug("CatalogController end call Get -> return Catalog not found");

                return NotFound();
            }

            _logger.LogDebug("CatalogController end call Get -> return Catalog");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates a catalog
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> PostAsync([FromBody] CatalogModel value)
        {
            _logger.LogDebug($"{nameof(CatalogController)} called {nameof(PostAsync)}");

            if (!ModelState.IsValid)
            {
                _logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                _logger.LogDebug("CatalogController end call PostAsync -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            _logger.LogDebug("CatalogController end call PostAsync -> return Catalog");

            value.EducationOrigin = string.IsNullOrEmpty(value.EducationOrigin) ? EducationOriginConstants.DefaultEducationOrigin : value.EducationOrigin;

            var repositoryResponse = await _repository.CreateCatalog(value);

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Update a catalog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] CatalogModel value)
        {
            _logger.LogDebug($"{nameof(CatalogController)} called {nameof(PutAsync)}");

            if (!ModelState.IsValid)
            {
                _logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                _logger.LogDebug("CatalogController end call PutAsync -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var catalog = await _repository.FindOneAsync<Catalog>(id);

            if (catalog is null)
            {
                _logger.LogDebug("No catalog found");

                _logger.LogDebug("CatalogController end call PutAsync -> return Not found");

                return NotFound();
            }

            _logger.LogDebug("CatalogController end call PutAsync -> return Catalog");

            catalog = _mapper.Map<CatalogModel, Catalog>(value, catalog);
            await _repository.ApplyChangesAsync(catalog);

            var resultCatalog = await _repository.GetCatalogById(id);

            return resultCatalog.GetHttpResponse();
        }

        /// <summary>
        ///     Delete a catalog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogDebug($"{nameof(CatalogController)} called {nameof(DeleteAsync)}");

            var entity = await _repository.FindOneAsync<Catalog>(id);
            var model = _mapper.Map<Catalog, CatalogModel>(entity);

            if (model == null)
            {
                _logger.LogDebug("No catalog found");

                _logger.LogDebug("CatalogController end call DeleteAsync -> return Not found");

                return NotFound();
            }
            _repository.Remove(id);

            _logger.LogDebug("CatalogController end call DeleteAsync -> return Ok");

            return Ok();
        }

        /// <summary>
        /// Receives a catalog. i.e., Kitchens(18503)
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="EducationOrigin"></param>
        /// <returns>FusionCode related to catalog</returns>
        [HttpGet("{catalog}/GetCode")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]

        public async Task<IActionResult> GetCodeFromCatalogue(
            string catalog,
            [FromQuery] string EducationOrigin)
        {
            _logger.LogDebug($"{nameof(CatalogController)} called {nameof(GetCodeFromCatalogue)}");

            EducationOrigin = string.IsNullOrEmpty(EducationOrigin) ? EducationOriginConstants.DefaultEducationOrigin : EducationOrigin;

            _logger.LogDebug("CatalogController end call GetCodeFromCatalogue -> return Call to get code");

            return (await _repository.GetCodeFromCatalogue(catalog, EducationOrigin)).GetHttpResponse();
        }

        /// <summary>
        /// Receives FusionCode related to a catalog
        /// </summary>
        /// <param name="code"></param>
        /// <param name="EducationOrigin"></param>
        /// <returns>Name</returns>
        [HttpGet("WithCode/{code}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]
        public async Task<IActionResult> GetCatalogueFromCode(
            string code,
            [FromQuery] string EducationOrigin)
        {
            _logger.LogDebug($"{nameof(CatalogController)} called {nameof(GetCatalogueFromCode)}");

            EducationOrigin = string.IsNullOrEmpty(EducationOrigin) ? EducationOriginConstants.DefaultEducationOrigin : EducationOrigin;

            _logger.LogDebug("CatalogController end call GetCatalogueFromCode -> return Call to get catalog");

            return (await _repository.GetCatalogueFromCode(code, EducationOrigin)).GetHttpResponse();
        }
    }
}

