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
.Domain.Specification.EndUserSpecifications;
using SpecialEducationPlanning
.Business.Mapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Country Controller
    /// </summary>
    [Route("api/[controller]")]
    public class CountryController : Controller
    {
        private readonly ICountryRepository repository;
        private readonly ILogger<CountryController> logger;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="logger">Logger</param>
        public CountryController(ICountryRepository repository, ILogger<CountryController> logger, IObjectMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Deletes a Country
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>OkResult if the Country is deleted.  In case the Country is not, a NotFoundResult</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public IActionResult Delete(int id)
        {
            logger.LogDebug("CountryController called Delete");

            //TODO Refactor specification
            var model = repository.FindOne<CountryModel>(id);
            if (model == null)
            {
                logger.LogDebug("No country found");

                logger.LogDebug("CountryController end call Delete -> return Not found");

                return NotFound();
            }

            repository.Remove(id);

            logger.LogDebug("CountryController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get Country by ID
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>If the ID is found returns Country Model if not, it returns a NotFoundResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("CountryController called Get");

            var entity = await repository.FindOneAsync<Country>(id);
            var model = mapper.Map<Country, CountryModel>(entity);
            RepositoryResponse<CountryModel> repositoryResponse = new RepositoryResponse<CountryModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No country found");

                logger.LogDebug("CountryController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("CountryController end call Get -> return Country");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all Countries
        /// </summary>
        /// <returns>Collection of Countries</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("CountryController called GetAll");
            var countryEntities = await repository.GetAllAsync<CountryModel>();
            var countryModels = mapper.Map<IEnumerable<Country>, IEnumerable<CountryModel>>(countryEntities);
            var repositoryResponse = new RepositoryResponse<IEnumerable<CountryModel>>
            {
                Content = countryModels
            };

            logger.LogDebug("CountryController end call -> return All countries");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Country</returns>
        [HttpGet("GetWithNavigations/{id}")]
        public async Task<IActionResult> GetWithChildrens(int id)
        {
            logger.LogDebug("CountryController called GetWithChildrens");

            RepositoryResponse<CountryModel> repositoryResponse = new RepositoryResponse<CountryModel>();
            var countryEntity = await repository.GetWithNavigationsAsync<Country>(id);
            var countryModel = mapper.Map<Country, CountryModel>(countryEntity);
            repositoryResponse.Content = countryModel;

            logger.LogDebug("CountryController end call GetWithChildrens -> return Country with children");

            return repositoryResponse.GetHttpResponse();
        }

        [HttpPost]
        [Route("GetCountriesFiltered")]
        public async Task<IActionResult> GetCountriesFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("CountryController called GetCountriesFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("CountryController end call GetCountriesFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await repository.GetCountriesFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("CountryController end call GetCountriesFiltered -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("CountryController end call GetCountriesFiltered -> return Paged Countries");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Creates a Country
        /// </summary>
        /// <param name="value">CountryModel</param>
        /// <returns>Returns the Country created, else, the error why it is not created</returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Post([FromBody] CountryModel value)
        {
            logger.LogDebug("CountryController called Post -> Create country");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("CountryController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            RepositoryResponse<CountryModel> repositoryResponse = await repository.GetDuplicatedCountry(value);

            if (!repositoryResponse.ErrorList.Any())
            {
                var country = await repository.FindOneAsync<Country>(value.Id);
                if (country.IsNull())
                {
                    country = mapper.Map(value, country);
                    await repository.Add(country);
                }
                else
                {
                    country = mapper.Map(value, country);
                    await repository.ApplyChangesAsync(country);
                }

                CountryModel countryModel = mapper.Map<Country, CountryModel>(country);
                repositoryResponse.Content = countryModel;
            }

            logger.LogDebug("CountryController end call Post -> return Created country");

            return repositoryResponse.GetHttpResponse();


        }
        /// <summary>
        ///     Updates a Country
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="value">Updated CountryModel</param>
        /// <returns>Returns the Country updated, if not, an error depending if the Country is not found by its ID or an error</returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] CountryModel value)
        {
            logger.LogDebug("CountryController called Put -> Update country");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("CountryController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            Country country = await repository.FindOneAsync<Country>(id);

            if (country is null)
            {
                logger.LogDebug("No country found");

                logger.LogDebug("CountryController end call Put -> return Not found");

                return NotFound();
            }

            var repositoryResponse = await repository.GetDuplicatedCountry(value);

            if (!repositoryResponse.ErrorList.Any())
            {
                country = mapper.Map(value, country);
                await repository.ApplyChangesAsync(country);
                var countryModel = mapper.Map<Country, CountryModel>(country);
                repositoryResponse.Content = countryModel;
            }

            logger.LogDebug("CountryController end call Put -> return Updated country");

            return repositoryResponse.GetHttpResponse();
        }
    }
}