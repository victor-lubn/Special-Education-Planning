using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    ///     Rom Item Controller
    /// </summary>
    [Route("api/[controller]")]
    public class RomItemController : Controller
    {
        private readonly IRomItemRepository romItemRepository;
        private readonly ILogger<RomItemController> logger;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="romItemRepository"></param>
        public RomItemController(IRomItemRepository romItemRepository, ILogger<RomItemController> logger, IObjectMapper mapper)
        {
            this.romItemRepository = romItemRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Get all ROM items 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Plan_Management)]
        public async Task<IActionResult> GetAllAsync()
        {
            logger.LogDebug("RomItemController called GetAllAsync");
            var list = await romItemRepository.GetAllAsync<RomItem>();
            var model = mapper.Map<RomItem,RomItemModel>(list);
            var repositoryResponse = new RepositoryResponse<IEnumerable<RomItemModel>>
            {
                Content = model
            };

            logger.LogDebug("RomItemController end call GetAllAsync -> return All rom items");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get a specific ROM item by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("RomItemController called Get");
            var entity = await romItemRepository.FindOneAsync<RomItem>(id);
            var model = mapper.Map<RomItem, RomItemModel>(entity);
            var repositoryResponse = new RepositoryResponse<RomItemModel>
            {
                Content = model
            };
            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No RomItem found");

                logger.LogDebug("RomItemController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("RomItemController end call Get -> return Rom item");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Create a new ROM item
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post([FromBody] RomItemModel value)
        {
            logger.LogDebug("RomItemController called Post");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("RomItemController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }
            if ((await romItemRepository.GetRomItemByNameAsync(value.ItemName)).Content.IsNotNull())
            {
                logger.LogDebug(ErrorCode.EntityAlreadyExist.GetDescription());

                logger.LogDebug("RomItemController end call Post -> return Bad request Entity exists");

                return BadRequest(ErrorCode.EntityAlreadyExist.GetDescription());
            }

            logger.LogDebug("RomItemController end call Post -> return Created rom item");
            var entity = mapper.Map<RomItemModel,RomItem>(value);
            var response = await romItemRepository.Add(entity);
            var model = mapper.Map<RomItem, RomItemModel>(response);
            return Json(model);
        }

        /// <summary>
        ///     Update a ROM item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]

        [AuthorizeTdpFilter(PermissionType.Plan_Modify)]
        public async Task<IActionResult> Put(int id, [FromBody] RomItemModel value)
        {
            logger.LogDebug("RomItemController called Put");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("RomItemController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            RomItem romItem = await romItemRepository.FindOneAsync<RomItem>(id);

            if (romItem.IsNull())
            {
                logger.LogDebug(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("RomItemController end call Put -> return Bad request Not found");

                return BadRequest(ErrorCode.EntityNotFound.GetDescription());
            }

            logger.LogDebug("RomItemController end call Put -> return Updated rom item");

            var entity = mapper.Map<RomItemModel, RomItem>(value, romItem);
            var response = await romItemRepository.ApplyChangesAsync(entity);
            var model = mapper.Map<RomItem, RomItemModel>(response);
            return Json(model);
        }

        /// <summary>
        ///     Delete a ROM item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("RomItemController called Delete");

            var entity = await romItemRepository.FindOneAsync<RomItem>(id);
            var model = mapper.Map<RomItem, RomItemModel>(entity);

            if (model == null)
            {
                logger.LogDebug("RomItem not found");

                logger.LogDebug("RomItemController end call Delete -> return Not found");

                return NotFound();
            }
            romItemRepository.Remove(id);

            logger.LogDebug("RomItemController end call Delete -> return Ok");

            return Ok();
        }
    }
}
