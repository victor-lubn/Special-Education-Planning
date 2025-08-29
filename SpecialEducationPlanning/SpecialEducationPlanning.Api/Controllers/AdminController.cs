using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;

using Koa.Domain.Search.Page;
using MediatR;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Admin Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAiepRepository _AiepRepository;
        private readonly ILogRepository _logRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAzureSearchManagementService _azureSearchManagementService;
        private readonly IFeatureManagementService _featureManagementService;
        private readonly ILogger<AdminController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="AiepRepository"></param>
        /// <param name="logRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="azureSearchManagementService"></param>
        /// <param name="featureManagementService"></param>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public AdminController(IServiceProvider serviceProvider,
            IAiepRepository AiepRepository,
            ILogRepository logRepository,
            IRoleRepository roleRepository,
            IAzureSearchManagementService azureSearchManagementService,
            IFeatureManagementService featureManagementService,
            ILogger<AdminController> logger, 
            IMediator mediator)
        {
            this._serviceProvider = serviceProvider;
            this._AiepRepository = AiepRepository;
            this._logRepository = logRepository;
            this._roleRepository = roleRepository;
            this._azureSearchManagementService = azureSearchManagementService;
            this._featureManagementService = featureManagementService;
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        ///     Mock builder Educationer seed
        /// </summary>
        /// <returns>Areas</returns>
        [HttpGet]
        [Route("dbSeed")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> GetDbSeed()
        {
            this._logger.LogDebug("AdminController called GetDbSeed");

            var context = this._serviceProvider.GetService(typeof(DataContext)) as DataContext;
            new DataMockup(context).SeedMockup();

            this._logger.LogDebug("AdminController end call GetDbSeed -> return Ok");

            return this.Ok();
        }

        /// <summary>
        ///     Import zones - not implemented
        /// </summary>
        /// <returns>Areas</returns>
        [HttpPost]
        [Route("ImportZones")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> ImportZones()
        {
            this._logger.LogDebug("AdminController called ImportZones");

            this._logger.LogDebug("AdminController end call ImportZones -> Not implemented exception");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     Update Aiep ACL
        /// </summary>
        /// <returns>Areas</returns>
        [HttpGet]
        [Route("Aiep/{id}/RefreshAcl")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> RefreshAiepAcl(int id)
        {
            this._logger.LogDebug("AdminController called RefreshAiepAcl");

            this._logger.LogDebug("AdminController end call RefreshAiepAcl -> return Call UpdateAiepAclAsync");

            return (await this._AiepRepository.UpdateAiepAclAsync(id)).GetHttpResponse();
        }

        /// <summary>
        ///     Update Aiep ACL using the Aiep code
        /// </summary>
        /// <returns>Areas</returns>
        [HttpGet]
        [Route("Aiep/{code}/RefreshAclWithAiepCode")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> RefreshAiepAclWithAiepCode(string code)
        {
            this._logger.LogDebug("AdminController called RefreshAiepAclWithAiepCode");

            this._logger.LogDebug("AdminController end call RefreshAiepAclWithAiepCode -> return Call UpdateAiepAclByAiepCodeAsync");

            return (await this._AiepRepository.UpdateAiepAclByAiepCodeAsync(code)).GetHttpResponse();
        }

        /// <summary>
        ///     Update all Aiep ACLs
        /// </summary>
        /// <returns>Areas</returns>
        [HttpGet]
        [Route("Aiep/RefreshAcl")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> RefreshAllAiepAcl()
        {
            this._logger.LogDebug("AdminController called RefreshAllAiepAcl");

            this._logger.LogDebug("AdminController end call RefreshAllAiepAcl -> return Call UpdateAllAiepAclAsync");

            return (await this._AiepRepository.UpdateAllAiepAclAsync()).GetHttpResponse();
        }

        /// <summary>
        ///     Refresh permission list
        /// </summary>
        /// <returns>Areas</returns>
        [HttpGet]
        [Route("Aiep/RefreshPermission")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> RefreshPermissions()
        {
            this._logger.LogDebug("AdminController called RefreshPermissions");

            this._logger.LogDebug("AdminController end call RefreshPermissions -> return Call RefreshPermissionListAsync");

            return (await this._roleRepository.RefreshPermissionListAsync()).GetHttpResponse();
        }

        /// <summary>
        ///     Retrieves log entries for specified take and skip values
        /// </summary>
        /// <returns>Areas</returns>
        [HttpGet]
        [Route("Log")]
        [AuthorizeTdpFilter(PermissionType.Log_Request)]
        public async Task<IActionResult> GetAllLog(int take, int skip)
        {
            this._logger.LogDebug("AdminController called GetAllLog");

            this._logger.LogDebug("AdminController end call GetAllLog -> return Call GetAllLog");

            return (await this._logRepository.GetAllLog(take, skip)).GetHttpResponse();
        }


        /// <summary>
        /// Ensures all of the components of Azure Search are created
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("EnsureAzureSearchCreated")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> EnsureAzureSearchCreated(bool deleteAndRecreate = false)
        {
            this._logger.LogDebug("AdminController called EnsureAzureSearchCreated");

            var result = await this._azureSearchManagementService.EnsureCreatedAsync(deleteAndRecreate);

            if (result)
            {
                this._logger.LogDebug("AdminController end call EnsureAzureSearchCreated -> return Ok");

                return new OkResult();
            }
            else
            {
                this._logger.LogDebug("AdminController end call EnsureAzureSearchCreated -> return Bad request");

                return new BadRequestObjectResult("Failed to ensure the Azure Search components are created...");
            }
        }
        /// <summary>
        /// Ensures all of the components of Azure Search are deleted
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAzureSearch")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> DeleteAzureSearch()
        {
            this._logger.LogDebug("AdminController called DeleteAzureSearch");

            var result = this._azureSearchManagementService.DeleteAzureSearch();

            if (result)
            {
                this._logger.LogDebug("AdminController end call DeleteAzureSearch -> return Ok");

                return new OkResult();
            }
            else
            {
                this._logger.LogDebug("AdminController end call DeleteAzureSearch -> return Bad request");

                return new BadRequestObjectResult("Failed to delete the Azure Search components...");
            }
        }

        /// <summary>
        ///     Returns the datetime of the latest log entry as a simple health check
        /// </summary>
        /// <returns>Success or failure</returns>
        [HttpGet]
        [Route("HealthCheckLog")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHealthCheckLog()
        {
            this._logger.LogDebug("AdminController called GetHealthCheckLog");

            this._logger.LogDebug("AdminController end call GetHealthCheckLog -> return Call GetHealthCheckLog");

            try
            {
                string result = DateTime.MinValue.ToString();
                var searchModel = new PageDescriptor(null, null)
                {
                    Take = 1,
                    Skip = 0
                };

                var response = await this._logRepository.GetLogsFiltered(searchModel);
                var content = response.Content;
                if (content != null && content.Result != null)
                {
                    using IEnumerator<Business.Model.LogModel> iter = content.Result.GetEnumerator();
                    iter.MoveNext();
                    var current = iter.Current;
                    if (current != null)
                    {
                        result = current.TimeStamp.ToString();
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("HealthCheck")]
        [AllowAnonymous]
        public async Task<IActionResult> HealthCheck()
        {
            this._logger.LogDebug("AdminController called HealthCheck");

            this._logger.LogDebug("AdminController end call HealthCheck -> return Ok");

            return this.Ok(new { Status = "ok`" });
        }

        [HttpPost]
        [Route("RunCustomIndexer")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> RunCustomIndexer(DateTime? updateDate, int? pageSize, int? indexerDelay, int? indexerWindowInDays)
        {
            this._logger.LogInformation("AdminController RunCustomIndexer -> Hit");

            RunIndexerEvent @event = new(updateDate, pageSize, indexerDelay, indexerWindowInDays);
            _ = this._mediator.Publish(@event);

            this._logger.LogInformation("AdminController RunCustomIndexer -> Fired RunIndexer Event");

            this._logger.LogInformation("AdminController RunCustomIndexer -> Exit");

            return Ok();
        }

        [HttpGet]
        [Route("HealthCheckLaunchDarkly")]
        [AllowAnonymous]
        public async Task<IActionResult> HealthCheckLaunchDarkly()
        {
            this._logger.LogDebug($"AdminController called {nameof(HealthCheckLaunchDarkly)}");

            bool healthCheckSuccesful = await this._featureManagementService.HealthCheck();

            if (!healthCheckSuccesful)
            {
                this._logger.LogWarning($"{nameof(HealthCheckLaunchDarkly)} has Failed.");
                return BadRequest($"{nameof(HealthCheckLaunchDarkly)} has Failed");
            }

            this._logger.LogDebug($"{nameof(HealthCheckLaunchDarkly)} was successful");
            return Ok();
        }
    }
}

