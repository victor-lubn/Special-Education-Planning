using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Interceptors;
using SpecialEducationPlanning
.Api.Services.DataMigration;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using SpecialEducationPlanning
.Business.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     DataMigration Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [AllowAnonymous]
    [TypeFilter(typeof(TokenFilter))]
    public class DataMigrationController : Controller
    {

        /// <summary>
        /// Aiep migration repository
        /// </summary>
        private AiepMigrationService AiepMigrationService;

        /// <summary>
        /// Builder migration repository
        /// </summary>
        private BuilderMigrationService builderMigrationService;

        /// <summary>
        /// Plan migration service
        /// </summary>
        private PlanMigrationService planMigrationService;

        /// <summary>
        /// Plan item migration service
        /// </summary>
        private PlanItemMigrationService planItemMigrationService;

        /// <summary>
        /// Class logger
        /// </summary>
        ILogger<DataMigrationController> logger;

        /// <summary>
        /// Is this service refreshing ACLs?
        /// </summary>
        private static bool refreshingAcls = false;

        /// <summary>
        /// Aiep repository
        /// </summary>
        private IAiepRepository AiepRepository;

        /// <summary>
        /// Creates a new instance of <see cref="DataMigrationController"/>
        /// </summary>
        /// <param name="AiepMigrationService">Aiep migration service</param>
        /// <param name="builderMigrationService">Builder migration service</param>
        /// <param name="planMigrationService">Plan migration service</param>
        /// <param name="planItemMigrationService">Plan item migration service</param>
        /// <param name="logger">Class logger</param>
        /// <param name="AiepRepository">Aiep repository</param>
        public DataMigrationController(AiepMigrationService AiepMigrationService, BuilderMigrationService builderMigrationService,
            PlanMigrationService planMigrationService, PlanItemMigrationService planItemMigrationService, ILogger<DataMigrationController> logger,
            IAiepRepository AiepRepository)
        {
            this.AiepMigrationService = AiepMigrationService;
            this.builderMigrationService = builderMigrationService;
            this.planMigrationService = planMigrationService;
            this.planItemMigrationService = planItemMigrationService;
            this.logger = logger;
            this.AiepRepository = AiepRepository;
        }

        /// <summary>
        /// Exodus notification that a migration process has Started
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Begin")]
        public IActionResult BeginMigration()
        {
            logger.LogDebug("DataMigrationController called BeginMigration");

            logger.LogInformation("Data migration process started...");

            logger.LogDebug("DataMigrationController end call BeginMigration -> return Ok");

            return Ok();
        }

        /// <summary>
        /// Exodus notification that a migration process has finished
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("End")]
        public async Task<IActionResult> EndMigration()
        {
            logger.LogDebug("DataMigrationController called EndMigration");

            if (refreshingAcls)
            {
                logger.LogDebug("DataMigrationController end call EndMigration -> return Ok refreshing Acls true");

                return Ok("Data migration process finished...");
            }
            refreshingAcls = true;
            try
            {
                //var result = await AiepRepository.UpdateAllAiepAclAsync();
                logger.LogInformation("Migration for entity finished");

                logger.LogDebug("DataMigrationController end call EndMigration -> return Ok refreshing Acls false");

                return Ok();
            }
            finally
            {
                refreshingAcls = false;
            }
        }

        /// <summary>
        /// Executes the Aiep migration to TDP 
        /// </summary>
        /// <param name="request">JSON array of Aiep migration models</param>
        /// <returns>Migration model</returns>
        [HttpPost]
        [Route("Aiep")]
        public IActionResult MigrateAiep([FromBody] IEnumerable<AiepMigrationModel> request)
        {
            logger.LogDebug("DataMigrationController called MigrateAiep");

            try
            {
                var response = AiepMigrationService.Migrate(request);

                logger.LogDebug("DataMigrationController end call MigrateAiep -> return Ok");

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogDebug("DataMigrationController end call MigrateAiep -> exception 500");

                return this.StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Executes the Customer migration to TDP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Customer")]
        public IActionResult MigrateCustomer([FromBody] IEnumerable<CustomerMigrationModel> request)
        {
            logger.LogDebug("DataMigrationController called MigrateAiep");

            try
            {
                var response = builderMigrationService.Migrate(request);

                logger.LogDebug("DataMigrationController end call MigrateAiep -> return Ok");

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogDebug("DataMigrationController end call MigrateAiep -> exception 500");

                return this.StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Executes the Quotes migration to TDP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Quote")]
        public IActionResult MigrateQuote([FromBody] IEnumerable<QuoteMigrationModel> request)
        {
            logger.LogDebug("DataMigrationController called MigrateQuote");

            try
            {
                var response = planMigrationService.Migrate(request);

                logger.LogDebug("DataMigrationController end call MigrateQuote -> return Ok");

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogDebug("DataMigrationController end call MigrateQuote -> exception 500");

                return this.StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// Executes the QuoteItems migration to TDP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("QuoteItem")]
        public IActionResult MigrateQuoteItem([FromBody] IEnumerable<QuoteItemMigrationModel> request)
        {
            logger.LogDebug("DataMigrationController called MigrateQuote");

            try
            {
                var response = planItemMigrationService.Migrate(request);

                logger.LogDebug("DataMigrationController end call MigrateQuote -> return Ok");

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogDebug("DataMigrationController end call MigrateQuote -> exception 500");

                return this.StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}

