using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.AutomaticRemoveItems;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Omni Search Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LogController : Controller
    {
        private readonly ILogRepository repository;
        private readonly ILogger<LogController> logger;
        private readonly IOptions<AutomaticRemoveSystemConfiguration> automaticRemoveSystemLogOptions;

        /// <summary>
        ///     Constructor
        /// </summary>
        public LogController(ILogRepository repository,
            ILogger<LogController> logger,
            IOptions<AutomaticRemoveSystemConfiguration> automaticRemoveSystemLogOptions)
        {
            this.repository = repository;
            this.logger = logger;
            this.automaticRemoveSystemLogOptions = automaticRemoveSystemLogOptions;
        }

        /// <summary>
        ///     Get all log entries (defaults to 1000)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("LogController called GetAll");

            var repositoryResponse = await repository.GetAllLog(1000, 0);

            logger.LogDebug("LogController end call GetAll -> return Get all logs");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary> 
        ///     Gets a filtered list of log entries
        /// </summary>
        /// <param name="searchModel"></param>
        [HttpPost("GetLogsFiltered")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> GetLogsFilteredAsync([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("LogController called GetLogsFilteredAsync (search model)");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("LogController end call GetLogsFilteredAsync (search model) -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 1000)
            {
                searchModel.Take = 1000;
            }
            var models = await repository.GetLogsFiltered(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("LogController end call GetLogsFilteredAsync (search model) -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("LogController end call GetLogsFilteredAsync (search model) -> return Paged logs");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary> 
        ///     Gets a filtered list of log entries
        /// </summary>
        /// <param name="level"></param>
        /// <param name="initDate"></param>
        /// <param name="endDate"></param>
        [HttpGet("GetLogsFilteredAsync")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> GetLogsFilteredAsync(string level, DateTime? initDate, DateTime? endDate)
        {
            logger.LogDebug("LogController called GetLogsFilteredAsync (level, init date, end date)");

            var repositoryResponse = await repository.GetLogsFilteredAsync(level, initDate, endDate);

            logger.LogDebug("LogController end call GetLogsFilteredAsync  (level, init date, end date) -> return List of logs");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post(LogModel log)
        {
            logger.LogDebug("LogController called Post");

            var repositoryResponse = await repository.SaveExternalLog(log);

            logger.LogDebug("LogController end call Post -> return Log");

            return repositoryResponse.GetHttpResponse();
        }

        #region Remove Old Log Item
        /// <summary>
        ///     Automatically remove log items greater than 90 days
        /// </summary>
        /// <returns></returns>
        [HttpDelete("AutomaticRemoveOldItems")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> AutomaticRemoveOldItems()
        {
            logger.LogDebug("LogController called AutomaticRemoveOldItems");

            var delete = double.Parse(automaticRemoveSystemLogOptions.Value.Delete, CultureInfo.InvariantCulture);
            await repository.AutomaticRemoveOldItems(DateTime.UtcNow, delete);

            logger.LogDebug("LogController end call AutomaticRemoveOldItems -> return Ok");

            return Ok();
        }

        #endregion
    }

}