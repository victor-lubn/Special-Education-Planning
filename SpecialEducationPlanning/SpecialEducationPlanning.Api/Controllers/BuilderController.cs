using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Koa.Persistence.Abstractions.QueryResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.SapConfiguration;
using SpecialEducationPlanning
.Api.Service.Sap;
using SpecialEducationPlanning
.Api.Service.Search;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpecialEducationPlanning
.Api.Controllers
{

    /// <summary>
    ///     Builder Controller
    /// </summary>
    [Route("api/[controller]")]
    public class BuilderController : Controller
    {

        private readonly IBuilderEducationerAiepRepository builderEducationerAiepRepository;

        private readonly IBuilderRepository builderRepository;

        private readonly IAiepRepository AiepRepository;

        private readonly ILogger<BuilderController> logger;

        private readonly IUserService userService;

        private readonly IOptions<SapConfiguration> options;

        private readonly IPlanRepository planRepository;

        private readonly IPostCodeServiceFactory postCodeServiceFactory;

        private readonly ISapService sapService;

        private readonly IAzureSearchService azureSearchService;

        private readonly AzureSearchConfiguration azureSearchConfiguration;

        private readonly IObjectMapper mapper;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="builderEducationerAiepRepository"></param>
        /// <param name="builderRepository"></param>
        /// <param name="AiepRepository"></param>
        /// <param name="planRepository"></param>
        /// <param name="sapService"></param>
        /// <param name="postCodeServiceFactory"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="userService"></param>
        public BuilderController(IBuilderEducationerAiepRepository builderEducationerAiepRepository,
            IBuilderRepository builderRepository, IAiepRepository AiepRepository,
            IPlanRepository planRepository, ISapService sapService, IPostCodeServiceFactory postCodeServiceFactory,
            IOptions<SapConfiguration> options, ILogger<BuilderController> logger, IUserService userService,
            IAzureSearchService azureSearchService, IOptions<AzureSearchConfiguration> configuration, IObjectMapper mapper)
        {
            this.builderEducationerAiepRepository = builderEducationerAiepRepository;
            this.builderRepository = builderRepository;
            this.AiepRepository = AiepRepository;
            this.planRepository = planRepository;
            this.sapService = sapService;
            this.postCodeServiceFactory = postCodeServiceFactory;
            this.options = options;
            this.logger = logger;
            this.userService = userService;
            this.azureSearchService = azureSearchService;
            this.azureSearchConfiguration = configuration.Value;
            this.mapper = mapper;
        }

        #region Methods Public

        /// <summary>
        ///     Assign the specified builder to the current user's Aiep
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AssignBuilderToCurrentUserAiep")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> AssignBuilderToCurrentUserAiep(int builderId)
        {
            logger.LogDebug("BuilderController called AssignBuilderToCurrentUserAiep");

            //Get
            var AiepId = this.userService.GetUserAiepId((ClaimsIdentity)this.User.Identity);

            var repositoryResponse =
                await builderEducationerAiepRepository.GetBuilderEducationerAiepModelRelation(builderId, AiepId);

            if (repositoryResponse.HasError())
            {
                var repositoryResponseCreateBuilderEducationerAiepModelRelation =
                     await builderEducationerAiepRepository.CreateBuilderEducationerAiepModelRelation(builderId, AiepId);

                repositoryResponseCreateBuilderEducationerAiepModelRelation.Content = null;

                logger.LogDebug("BuilderController end call AssignBuilderToCurrentUserAiep -> return Created relation");

                return repositoryResponseCreateBuilderEducationerAiepModelRelation.GetHttpResponse();

            }
            repositoryResponse.Content = null;

            logger.LogDebug("BuilderController end call AssignBuilderToCurrentUserAiep -> return Relation");

            return repositoryResponse.GetHttpResponse();
        }


        /// <summary>
        /// Delete a Builder by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("BuilderController called Delete -> Delete builder");

            // TODO Refactor specification
            if (!await builderRepository.CheckIfExistsAsync(id))
            {
                logger.LogDebug("Builder not found");

                logger.LogDebug("BuilderController end call Delete -> return Not Found");

                return NotFound();
            }

            builderRepository.Remove(id);

            logger.LogDebug("BuilderController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Delete the value of account number from a builder
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/AccountNumber")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> DeleteAccountNumberFromBuilder(string accountNumber, int idBuilder)
        {
            logger.LogDebug("BuilderController called DeleteAccountNumberFromBuilder");

            var normalisedAccountNumber = accountNumber.NormaliseAccountNumber();

            if (normalisedAccountNumber.IsNullOrEmpty() || idBuilder.IsNull())
            {
                logger.LogDebug("Invalid Account Nº or builder");

                logger.LogDebug("BuilderController end call DeleteAccountNumberFromBuilder -> return Bad request");

                return BadRequest("Invalid Account Nº or builder");
            }

            logger.LogDebug("BuilderController end call DeleteAccountNumberFromBuilder -> return Response");

            return (await builderRepository.DeleteAccountNumberAsync(idBuilder, normalisedAccountNumber)).GetHttpResponse();
        }

        /// <summary>
        ///     Fetch and return a Builder by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Builder Model</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("BuilderController called Get -> Gets Builder by Id");


            var entity = await builderRepository.FindOneAsync<Builder>(id);
            var model = mapper.Map<Builder, BuilderModel>(entity);
            var repositoryResponse = new RepositoryResponse<BuilderModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("Builder not found");

                logger.LogDebug("BuilderController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("BuilderController end call Get -> return Builder");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Fetch for all builders
        /// </summary>
        /// <returns>Array of Builder Models</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("BuilderController called GetAll -> Gets all builders");
            IEnumerable<Builder> builderEntities =  await builderRepository.GetAllAsync<Builder>();
            IEnumerable<BuilderModel> builderModels = mapper.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(builderEntities);

            var repositoryResponse = new RepositoryResponse<IEnumerable<BuilderModel>>
            {
                Content = builderModels
            };

            logger.LogDebug("BuilderController end call GetAll -> return All builders");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get a SAP builder
        /// </summary>
        /// <returns>A SAP builder</returns>
        [HttpPost]
        [Route("BuilderFromSap")]
        [AuthorizeTdpFilter(PermissionType.Plan_Create, PermissionType.Plan_Modify)]
        public async Task<IActionResult> GetBuilderFromSap([FromBody] List<string> accountNumber)
        {
            logger.LogDebug("BuilderController called GetBuilderFromSap (Account number)");

            var response = await sapService.GetSapBuilder(accountNumber);

            logger.LogDebug("BuilderController end call GetBuilderFromSap  (Account number)");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Get a SAP builder
        /// </summary>
        /// <param name="surname"></param>
        /// <param name="postcode"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBuilderFromSap/ByMandatoryFields")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify, PermissionType.Plan_Create)]
        public async Task<IActionResult> GetBuilderFromSap(string surname, string postcode, string address)
        {
            logger.LogDebug("BuilderController called GetBuilderFromSap (Surname, Postcode, Address)");
            
            var normalizedPostCode = postCodeServiceFactory.GetService(null).RepresentPostcode(postcode);
            var response = await sapService.GetSapBuilderAsync(surname, normalizedPostCode, address);

            logger.LogDebug("BuilderController end call GetBuilderFromSap (Surname, Postcode, Address)");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Get a SAP builder
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBuilderModelIfExist")]
        public async Task<IActionResult> GetBuilderModelIfExistAsync([FromBody] BuilderModel value)
        {
            logger.LogDebug("BuilderController called GetBuilderModelIfExistAsync -> Gets SAP builder");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("BuilderController end call GetBuilderModelIfExistAsync -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var response = await builderRepository.GetExistingBuilderAsync(value);

            logger.LogDebug("BuilderController end call GetBuilderModelIfExistAsync -> return Builder");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Get the builders paginated and filtered
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetBuildersFiltered")]
        public async Task<IActionResult> GetBuildersFilteredPaged([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("BuilderController called GetBuildersFilteredPaged");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("BuilderController end call GetBuildersFilteredPaged -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var currentAiepId = userService.GetUserCurrentAiepId((ClaimsIdentity)this.User.Identity);

            if (azureSearchConfiguration.LefthandSearchEnabled && azureSearchService.GetType() == typeof(AzureSearchService))
            {
                var idsFilterd = await azureSearchService.GetBuilderIdsFilteredAsync<OmniSearchBuilderIndexModel>(searchModel, currentAiepId);
                var builderIds = idsFilterd.BuilderFilterdIds;
                var builders = await builderRepository.GetBuildersByIdsAsync(builderIds, searchModel.Skip.Value, searchModel.Take.Value, currentAiepId, idsFilterd.Sort);
                var pagedQuery = new PagedQueryResult<BuilderModel>(builders.Content, searchModel.Take.Value, searchModel.Skip.Value, idsFilterd.TotalCount);

                logger.LogDebug("BuilderController end call GetBuildersFilteredPaged -> return Query with AzureSearch left hand enabled");

                return this.PagedJsonResult(pagedQuery, true);
            }
            else
            {
                var repositoryResponse = await builderRepository.GetBuildersFiltered(searchModel, currentAiepId);
                if (repositoryResponse.ErrorList.Any())
                {
                    logger.LogDebug("BuilderController end call GetBuilderFilteredPaged -> return Error");

                    return repositoryResponse.GetHttpResponse();
                }

                logger.LogDebug("BuilderController end call GetBuilderFilteredPaged -> return Query with AzureSearch left hand disabled");

                return this.PagedJsonResult(repositoryResponse.Content, true);
            }
        }

        /// <summary>
        ///     Method that gets the posible matches of a given builder (one builder if it matches perfectly with the mandatory
        ///     fields,
        ///     a list of builders if exists some of them with the same postcode, and noone if the other conditions are not
        ///     acomplished)
        /// </summary>
        /// <param name="builderModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetPosibleMatchingBuilders")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify, PermissionType.Plan_Create)]
        public async Task<IActionResult> GetPossibleMatchingBuilders([FromBody] BuilderModel builderModel)
        {
            logger.LogDebug("BuilderController called GetPossibleMatchingBuilders");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("BuilderController end call GetPossibleMatchingBuilders -> return Bad Request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (!String.IsNullOrEmpty(builderModel.Postcode))
                builderModel.Postcode = postCodeServiceFactory.GetService(null).NormalisePostcode(builderModel.Postcode);

            // Check if Builder is in TDP
            var responseTdpMatch = await builderRepository.GetPosibleTdpMatchingBuilders(builderModel);

            if (responseTdpMatch.HasError() || responseTdpMatch.Content.IsNull())
            {
                logger.LogDebug("BuilderController end call GetPossibleMatchingBuilders -> return No matching builder");

                return responseTdpMatch.GetHttpResponse();
            }

            if (responseTdpMatch.Content.Type == BuilderMatchType.Exact)
            {
                logger.LogDebug("BuilderController end call GetPossibleMatchingBuilders -> return One builder exact match in TDP");

                return responseTdpMatch.GetHttpResponse();
            }

            // Check if builder is in SAP
            RepositoryResponse<ValidationBuilderModel> responseSap;

            try
            {
                if (!String.IsNullOrEmpty(builderModel.Postcode))
                    builderModel.Postcode = postCodeServiceFactory.GetService(null).RepresentPostcode(builderModel.Postcode);

                responseSap = await sapService.GetPosibleSapBuilder(builderModel);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in SAP");
                responseSap = new RepositoryResponse<ValidationBuilderModel>();
            }

            if (responseSap.HasError() || responseSap.Content.IsNull())
            {
                logger.LogError("Error in SAP retrievig information");
            }

            if (responseSap.Content.IsNotNull() && responseSap.Content.Type == BuilderMatchType.Exact)
            {
                logger.LogDebug("BuilderController end call GetPossibleMatchingBuilders -> return One builder exact match in SAP");

                return responseSap.GetHttpResponse();
            }

            logger.LogDebug("BuilderController end call GetPossibleMatchingBuilders -> return List of builders from TDP and SAP");

            return builderRepository.MergeBuilderTdpAndSapSearch(responseTdpMatch.Content, responseSap.Content)
                .GetHttpResponse();
        }

        /// <summary>
        ///     Method that gets the possible matches of a given builder (one builder if it matches perfectly with the account
        ///     number and none if not)
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetPossibleMatchingBuilderByAccountNumber")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify, PermissionType.Plan_Create)]
        public async Task<IActionResult> GetPossibleMatchingBuilderByAccountNumber(string accountNumber)
        {
            logger.LogDebug("BuilderController called GetPossibleMatchingBuilderByAccountNumber");
            var normalisedAccountNumber = accountNumber.NormaliseAccountNumber();
            // Check if Builder is in TDP
            var responseTdpMatch = await builderRepository.GetPossibleTdpMatchingBuilderByAccountNumberAsync(normalisedAccountNumber);

            if (responseTdpMatch.HasError() || responseTdpMatch.Content.IsNull() || responseTdpMatch.Content.Builders.Count > 0)
            {
                logger.LogDebug("BuilderController end call GetPossibleMatchingBuilderByAccountNumber -> return Error, null content or more than one Builder");

                return responseTdpMatch.GetHttpResponse();
            }

            // Check if builder is in SAP
            RepositoryResponse<ValidationBuilderModel> responseSap;

            try
            {
                responseSap = await sapService.GetPossibleSapBuilderByAccountNumber(normalisedAccountNumber);
            }
            catch (Exception e)
            {
                logger.LogError("Error in SAP");
                logger.LogError("Skipping SAP search");
                logger.LogError(e, "Error in SAP");
                responseSap = new RepositoryResponse<ValidationBuilderModel>();
            }

            if (responseSap.HasError())
            {
                logger.LogError("Error in SAP retrievig information");
                logger.LogDebug("BuilderController end call GetPossibleMatchingBuilderByAccountNumber -> return Error with SAP information");
                return responseSap.GetHttpResponse();
            }

            logger.LogDebug("BuilderController end call GetPossibleMatchingBuilderByAccountNumber -> return Builder");

            return responseSap.GetHttpResponse();
        }

        /// <summary>
        ///     Move a builder from one Aiep to another
        /// </summary>
        /// <param name="builderId"></param>
        /// <param name="oldAiepId"></param>
        /// <param name="newAiepId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{builderId}/TransferFromAiep/{oldAiepId}/To/{newAiepId}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> MoveBuilder(int builderId, int oldAiepId, int newAiepId)
        {
            logger.LogDebug("BuilderController called MoveBuilder");

            if (!await builderRepository.CheckIfExistsAsync(builderId) ||
                !await AiepRepository.CheckIfExistsAsync(newAiepId) ||
                !await AiepRepository.CheckIfExistsAsync(oldAiepId))
            {
                logger.LogDebug("BuilderController end call MoveBuilder -> return Not found builder, old Aiep or new Aiep");

                return NotFound();
            }

            var builder = await builderRepository.GetAssignedPlansAsync(builderId);

            var builderEducationerAiepModel =
                (await builderEducationerAiepRepository.GetBuilderEducationerAiepModelByBuilderIdAiepId(builderId,
                    oldAiepId)).Content;

            if ((await builderEducationerAiepRepository.GetBuilderEducationerAiepModelByBuilderIdAiepId(builderId,
                newAiepId)).Content.IsNull())
            {
                builderEducationerAiepModel.AiepId = newAiepId;
                var getAiepEntity = await AiepRepository.FindOneAsync<Aiep>(newAiepId);
                var getAiepModel = mapper.Map<Aiep, AiepModel>(getAiepEntity);
                builderEducationerAiepModel.Aiep = getAiepModel;
                var builderEducationerAiepEntity = mapper.Map<BuilderEducationerAiepModel, BuilderEducationerAiep>(builderEducationerAiepModel);
                await builderEducationerAiepRepository.ApplyChangesAsync(builderEducationerAiepEntity);
            }
            else
            {
                await builderEducationerAiepRepository.RemoveAsync(builderEducationerAiepModel.Id);
            }

            foreach (var plan in builder.Content.Plans)
            {
                await planRepository.AssignPlanToAiepAsync(plan.Id, newAiepId);
            }

            logger.LogDebug("BuilderController end call MoveBuilder -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Create Builder
        /// </summary>
        /// <param name="builderModel"></param>
        /// <returns>Returns the builder created or </returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post([FromBody] BuilderModel builderModel)
        {
            logger.LogDebug("BuilderController called Post -> create builder");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("BuilderController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var match = await builderRepository.GetExistingBuilderOrEmptyAsync(builderModel);

            if (match.Content != null)
            {
                if (match.ErrorList.Any())
                {
                    logger.LogDebug("BuilderController end call Post -> return Error with existing builder");

                    return match.GetHttpResponse();
                }

                logger.LogDebug("BuilderController end call Post -> return Builder already exists");

                return new BadRequestObjectResult(ErrorCode.EntityAlreadyExist.GetDescription());
            }

            var AiepId = userService.GetUserAiepId((ClaimsIdentity)this.User.Identity);
            var userId = userService.GetUserId((ClaimsIdentity)this.User.Identity);

            var responsePostCode = postCodeServiceFactory.GetService(null).GetPostCode(builderModel.Postcode);
            if (responsePostCode.HasError() || responsePostCode.Content.IsNull())
            {
                logger.LogError("BuilderController: Error bad PostCode");
                responsePostCode.AddError(ErrorCode.GenericBusinessError, "Bad PostCode");

                logger.LogDebug("BuilderController end call Post -> return Bad postcode");

                return responsePostCode.GetHttpResponse();
            }

            builderModel.Postcode = responsePostCode.Content;

            var response = await builderRepository.CreateAsync(builderModel, userId, AiepId);

            if (response.ErrorList.Any())
            {
                logger.LogDebug("BuilderController end call Post -> return Error creating");

                return response.GetHttpResponse();
            }

            logger.LogDebug("BuilderController end call Post -> return Builder created");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Update a builder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="builderModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> Put(int id, [FromBody] BuilderModel builderModel)
        {
            logger.LogDebug("BuilderController called Put -> Update builder");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("BuilderController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (builderModel.Id.IsNull() || builderModel.Id == 0)
            {
                logger.LogDebug("No builder found");

                logger.LogDebug("BuilderController end call Put -> return Not found");

                return NotFound();
            }

            builderModel.Postcode = postCodeServiceFactory.GetService(null).NormalisePostcode(builderModel.Postcode);
            var match = await builderRepository.GetExistingBuilderOrEmptyAsync(builderModel);

            if (match.Content != null && match.Content.Id != id)
            {
                if (match.ErrorList.Any())
                {
                    logger.LogDebug("BuilderController end call Put -> return Error with existing builder");

                    return match.GetHttpResponse();
                }

                logger.LogDebug("BuilderController end call Put -> return Builder already exist");

                return new BadRequestObjectResult(ErrorCode.EntityAlreadyExist.GetDescription());
            }

            var responsePostCode = postCodeServiceFactory.GetService(null).GetPostCode(builderModel.Postcode);
            if (responsePostCode.HasError() || responsePostCode.Content.IsNull())
            {
                logger.LogError("BuilderController: Error bad PostCode");
                responsePostCode.AddError(ErrorCode.GenericBusinessError, "Bad PostCode");

                logger.LogDebug("BuilderController end call Put -> return Error bad postcode");

                return responsePostCode.GetHttpResponse();
            }

            builderModel.Postcode = responsePostCode.Content;

            var builder = await builderRepository.FindOneAsync<Builder>(id);
            builder = mapper.Map(builderModel, builder);
            await builderRepository.ApplyChangesAsync(builder);

            var builderModelMapped = mapper.Map<Builder, BuilderModel>(builder);

            var plansTradingNameUpdated = await planRepository.UpdateBuilderPlansTradingName(builder.Id, builder.TradingName);

            if (!plansTradingNameUpdated)
            {
                logger.LogError("Unable to update the trading name of the plans assigned to " + builderModelMapped.Name + " with ID " + builderModelMapped.Id + " and trading name " + builderModelMapped.TradingName);
            }

            logger.LogDebug("BuilderController end call Put -> return Ok");

            return Ok(builderModelMapped);
        }

        /// <summary>
        ///     Update a builder's notes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="builderNotes"></param>
        /// <returns></returns>
        [HttpPut("{id}/modifycreditbuildernotes")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> ModifyCreditBuilderNotes(int id, [FromBody] string builderNotes)
        {
            logger.LogDebug("BuilderController called ModifyCreditBuilderNotes");

            var response = await builderRepository.ModifyBuilderNotes(id, builderNotes);

            logger.LogDebug("BuilderController end call ModifyCreditBuilderNotes -> return Response");

            return response.GetHttpResponse();
        }

        /// <summary>
        ///     Removes users identifications
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("BuilderRightToBeForgottenTwoYearAfterAsync/{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> BuilderRightToBeForgottenTwoYearAfterAsync(int id)
        {
            logger.LogDebug("BuilderController called BuilderRightToBeForgottenTwoYearAfterAsync");

            var repositoryResponse = await builderRepository.BuilderCleanManagment(id);
            if (repositoryResponse.Content == null)
            {
                logger.LogDebug("Error anonymising Builder");

                logger.LogDebug("BuilderController end call BuilderRightToBeForgottenTwoYearAfterAsync -> return Bad request");

                return BadRequest();
            }

            logger.LogDebug("BuilderController end call BuilderRightToBeForgottenTwoYearAfterAsync -> return Builder cleaned");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     SAP health check 
        /// </summary>
        /// <returns></returns>
        [HttpGet("HealthCheck/Sap")]
        public async Task<IActionResult> SapHealthCheck()
        {
            logger.LogDebug("BuilderController called SapHealthCheck");

            string message = await sapService.HealthCheck();

            if (message.IsNullOrEmpty())
            {
                logger.LogInformation("BuilderController end call SapHealthCheck -> return Ok");

                return Ok();
            }

            ContentResult resp = Content(message);
            resp.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

            logger.LogInformation("BuilderController end call SapHealthCheck -> return Error");

            return resp;
        }

        /// <summary>
        ///     Update builder with
        /// </summary>
        /// <param name="builderTDP"></param>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateBuilderFromSapByAccountNumber")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify, PermissionType.Plan_Create)]
        public async Task<IActionResult> UpdateBuilderFromSapByAccountNumber(BuilderModel builderTDP, string accountNumber)
        {
            logger.LogDebug("BuilderController called UpdateBuilderFromSapByAccountNumber");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Bad request invalid model");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (accountNumber.IsNullOrEmpty())
            {
                logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Bad request no Account number");

                return BadRequest(ErrorCode.ArgumentErrorBusiness.GetDescription());
            }

            List<string> accountNumberArr = new List<string>() { accountNumber };
            var sapBuilderList = sapService.GetSapBuilder(accountNumberArr);
            if (sapBuilderList.Result == null)
            {
                if (sapBuilderList.Result.Content == null)
                {
                    logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Bad request Null content from SAP");

                    return BadRequest();
                }

                logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Bad request Null result from SAP");

                return BadRequest();
            }

            var sapBuilder = sapBuilderList.Result.Content.FirstOrDefault();
            if (string.IsNullOrEmpty(sapBuilder.Address1))
                sapBuilder.Address1 = builderTDP.Address1?? builderTDP.Address0 ?? "N/A";

            var tdpBuilderEdited = await builderRepository.UpdateBuilderFromSAPByAccountNumberAsync(sapBuilder, builderTDP);
            if (tdpBuilderEdited == null)
            {              

                logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Bad request Null Updated builder");

                return BadRequest();
            }
            else if (tdpBuilderEdited.Content == null)
            {
                logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Bad request Null content Updated builder");

                return BadRequest();
            }

            logger.LogDebug("BuilderController end call UpdateBuilderFromSapByAccountNumber -> return Ok");

            return Ok(tdpBuilderEdited.Content);
        }

        /// <summary>
        ///     Delete the value of account number from a builder
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateSapBuilders")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify, PermissionType.Plan_Create)]
        public async Task<IActionResult> UpdateSapBuilders([FromBody] IEnumerable<BuilderSapModel> builderModels)
        {
            logger.LogDebug("BuilderController called UpdateSapBuilders");

            foreach (BuilderSapModel builder in builderModels)
            {
                var responsePostCode = postCodeServiceFactory.GetService(null).GetPostCode(builder.Postcode);
                if (responsePostCode.HasError() || responsePostCode.Content.IsNull())
                {
                    logger.LogError("BuilderController: Error bad PostCode");
                    responsePostCode.AddError(ErrorCode.GenericBusinessError, "Bad PostCode");

                    logger.LogDebug("BuilderController end call UpdateSapBuilders -> return Bad postcode");

                    return responsePostCode.GetHttpResponse();
                }

                builder.Postcode = responsePostCode.Content;
            }

            logger.LogDebug("BuilderController end call UpdateSapBuilders -> return Update builders from SAP");

            return (await builderRepository.UpdateBuildersFromSapAsync(builderModels)).GetHttpResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="idBuilder"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UnassignAccountNumber")]
        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> UnassignAccountNumber(string accountNumber, int idBuilder)
        {
            logger.LogDebug("BuilderController called UnassignAccountNumber");

            var normalisedAccountNumber = accountNumber.NormaliseAccountNumber();

            if (normalisedAccountNumber.IsNullOrEmpty() || idBuilder.IsNull())
            {
                logger.LogDebug("BuilderController end call UnassignAccountNumber -> return No builder Id or Account number");

                return BadRequest(ErrorCode.ArgumentErrorBusiness.GetDescription());
            }

            return (await builderRepository.DeleteAccountNumberAsync(idBuilder, normalisedAccountNumber)).GetHttpResponse();
        }

        #endregion
    }

}

