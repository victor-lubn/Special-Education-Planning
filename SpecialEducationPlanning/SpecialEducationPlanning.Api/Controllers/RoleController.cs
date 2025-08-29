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
    ///     Role Controller
    /// </summary>
    [Route("api/[controller]")]
    public class RoleController : Controller
    {

        private readonly ILogger<RoleController> logger;

        private readonly IRoleRepository roleRepository;

        private readonly IUserService userService;

        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="roleRepository">Repository</param>
        /// <param name="logger">Logger</param>
        public RoleController(IRoleRepository roleRepository
            , ILogger<RoleController> logger, IUserService userService, IObjectMapper mapper)
        {
            this.roleRepository = roleRepository;
            this.logger = logger;
            this.userService = userService;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Deletes a Role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>OkResult if the Role is deleted. In case the Role is not deleted, NotFound</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("RoleController called Delete");

            var entity = await roleRepository.FindOneAsync<Role>(id);
            var model = mapper.Map<Role, RoleModel>(entity);
            if (model == null)
            {
                logger.LogDebug("No role found");

                logger.LogDebug("RoleController end call Delete -> return Not found");

                return NotFound();
            }

            roleRepository.Remove(id);
            await userService.ResetClaimsCacheAsync();

            logger.LogDebug("RoleController end call Delete -> return Ok");

            return Ok();
        }

        /// <summary>
        ///     Get Role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>If the ID is found returns the model, if not, it returns a NotFound</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("RoleController called Get");

            var entity = await roleRepository.FindOneAsync<Role>(id);
            var model = mapper.Map<Role, RoleModel>(entity);
            var repositoryResponse = new RepositoryResponse<RoleModel>
            {
                Content = model
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No role found");

                logger.LogDebug("RoleController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("RoleController end call Get -> return Role");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all Roles
        /// </summary>
        /// <returns>Collection of Roles</returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("RoleController called GetAll");
            var entities = await roleRepository.GetAllAsync<Role>();
            var models = mapper.Map<IEnumerable<Role>, IEnumerable<RoleModel>>(entities);  
            var repositoryResponse = new RepositoryResponse<IEnumerable<RoleModel>>
            {
                Content = models
            };

            logger.LogDebug("RoleController end call GetAll -> return All roles");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get Permissions Assigned and Available for a certain Role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>Model with collections of Assigned permissions and Available Permissions</returns>
        [HttpGet("{roleId}/PermissionsAssignedAvailable")]
        public async Task<IActionResult> GetPermissionsAssignedAvailable(int roleId)
        {
            logger.LogDebug("RoleController called GetPermissionsAssignedAvailable");

            var repositoryResponse = await roleRepository.GetPermissionsAssignedAvailable(roleId);

            logger.LogDebug("RoleController end call GetPermissionsAssignedAvailable -> return Permissions assigned available");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get Permissions associated to a Role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>Collection of Permissions associated to the Role</returns>
        [HttpGet("{roleId}/PermissionsFromRole")]
        public async Task<IActionResult> GetPermissionsFromRole(int roleId)
        {
            var repositoryResponse = await roleRepository.GetPermissionsFromRole(roleId);

            return repositoryResponse.GetHttpResponse();
        }

        [HttpPost]
        [Route("GetRolesFiltered")]
        public async Task<IActionResult> GetRolesFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("RoleController called GetRolesFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("RoleController end call GetRolesFiltered -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await roleRepository.GetRolesFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("RoleController end call GetRolesFiltered -> return Errors");

                return models.GetHttpResponse();
            }

            logger.LogDebug("RoleController end call GetRolesFiltered -> return Paged roles");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Get Permissions by User ID related by the Role
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Collection of Permissions associated to a User</returns>
        [HttpGet("{userId}/roleuserpermissions")]
        public async Task<IActionResult> GetRoleUserPermissionsAsync(int userId)
        {
            logger.LogDebug("RoleController called GetRoleUserPermissionsAsync");

            var repositoryResponse = await roleRepository.GetUserPermissionsAsync(userId);

            logger.LogDebug("RoleController end call GetRoleUserPermissionsAsync -> return List of permissions");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates a Role
        /// </summary>
        /// <param name="roleModel">RoleModel to Create</param>
        /// <returns>Returns the Role created, else, the error why it is not created</returns>
        [HttpPost]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> Post([FromBody] RoleModel roleModel)
        {
            logger.LogDebug("RoleController called Post -> Create role");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("RoleController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var roleEntity = mapper.Map<RoleModel,Role>(roleModel);
            var roleApply = await roleRepository.ApplyChangesAsync(roleEntity);
            var roleModelReturn = mapper.Map<Role,RoleModel>(roleApply);

            var repositoryResponse = new RepositoryResponse<RoleModel>
            {
                Content = roleModelReturn
            };

            await userService.ResetClaimsCacheAsync();

            logger.LogDebug("RoleController end call Post -> return Created role");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Updates a Role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="roleModel">RoleModel to Update</param>
        /// <returns>Returns the Role updated, if not, an error depending if the Role is NotFound or an error</returns>
        [HttpPut("{id}")]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] RoleModel roleModel)
        {
            logger.LogDebug("RoleController called Put");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}",
                    ModelState.GetErrorMessages().Join(Environment.NewLine));

                logger.LogDebug("RoleController end call Put -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var roleEntity = await roleRepository.FindOneAsync<Role>(id);
            var role = mapper.Map<Role,RoleModel>(roleEntity);

            if (role == null)
            {
                logger.LogDebug("No role found");

                logger.LogDebug("RoleController end call Put -> return Not found");

                return NotFound();
            }
            var roleApply = await roleRepository.ApplyChangesAsync(roleEntity);
            var roleModelReturn = mapper.Map<Role, RoleModel>(roleApply);

            var repositoryResponse = new RepositoryResponse<RoleModel>
            {
                Content = roleModelReturn
            };

            await userService.ResetClaimsCacheAsync();

            logger.LogDebug("RoleController end call Put -> return Updated role");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Creates a Role with the selected Permissions
        /// </summary>
        /// <param name="rolePermission">Model to Create with Name of Role and array of PermissionsModels</param>
        /// <returns></returns>
        [HttpPost("RolePermission")]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> SetRolePermissions([FromBody] RolePermissionModel rolePermission)
        {
            logger.LogDebug("RoleController called SetRolePermissions -> Create a role with permisions");

            var repositoryResponse = await roleRepository.SetPermissions(rolePermission);
            await userService.ResetClaimsCacheAsync();

            logger.LogDebug("RoleController end call SetRolePermissions -> return Created role with permissions");

            return repositoryResponse.GetHttpResponse();
        }

        [HttpPost("ResetClaims")]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> ResetClaims()
        {
            logger.LogDebug("RoleController called ResetClaims");

            if (await userService.ResetClaimsCacheAsync())
            {
                logger.LogDebug("RoleController end call ResetClaims -> return Ok");

                return Ok();
            }
            else
            {
                logger.LogDebug("RoleController end call ResetClaims -> return Bad request");

                return BadRequest("Error ResetClaimsCache");
            }


        }


        /// <summary>
        ///     Update a set of Permissions to the given Role.  This will remove the current Permissions and set the new ones
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rolePermission">Permissions to set to the Role</param>
        /// <returns></returns>
        [HttpPut("{id}/RolePermissions")]
        [AuthorizeTdpFilter(PermissionType.Role_Management)]
        public async Task<IActionResult> UpdateRolePermissions(int id, [FromBody] RolePermissionModel rolePermission)
        {
            logger.LogDebug("RolePermission called UpdateRolePermissions");

            var roleModel = new RoleModel { Id = id, Name = rolePermission.Name };
            var repositoryResponse = await roleRepository.UpdatePermissions(roleModel, rolePermission.Permissions);
            await userService.ResetClaimsCacheAsync();

            logger.LogDebug("RolePermission end call UpdateRolePermissions -> return Generic response");

            return repositoryResponse.GetHttpResponse();
        }
    }

}