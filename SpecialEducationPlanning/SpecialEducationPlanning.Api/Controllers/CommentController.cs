using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
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
    ///     Comment Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository repository;
        private readonly ILogger<CommentController> logger;
        private readonly IUserService userService;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commentRepository"></param>
        /// <param name="logger"></param>
        /// <param name="userService"></param>
        public CommentController(ICommentRepository commentRepository,
            ILogger<CommentController> logger,
            IUserService userService,
            IObjectMapper mapper)
        {
            this.repository = commentRepository;
            this.logger = logger;
            this.userService = userService;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Updates a comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns>The comment updated or an error depending on the nature of the error</returns>
        [HttpPut]
        [Route("{id}")]
        [AuthorizeTdpFilter(PermissionType.Comment_Management)]
        public async Task<IActionResult> Put(int id, [FromBody] CommentModel comment)
        {
            logger.LogDebug("CommentController called Put -> Update comment");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("CommentController end call -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var existingComment = await repository.FindOneAsync<Comment>(id);
            var existingCommentModel = mapper.Map<Comment, CommentModel>(existingComment);
            if (existingCommentModel.IsNull())
            {
                logger.LogDebug("No comment found");

                logger.LogDebug("CommentController end call -> return Not found");

                return NotFound();
            }

            var userUniqueIdentifier = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);
            // If the user perfoming the action is the same as the user who created the comment he CAN edit the comment || user has permssions to edit
            if ((userUniqueIdentifier.Equals(existingComment.User))
                || userService.GetUserPermissions((ClaimsIdentity)User.Identity).Any(c => c.Value == PermissionType.Plan_Comment.GetDescription()))
            {
                var repositoryResponse = new RepositoryResponse<CommentModel>((await repository.UpdateComment<PlanModel>(comment, userUniqueIdentifier)).Content);

                logger.LogDebug("CommentController end call -> return Comment");

                return repositoryResponse.GetHttpResponse();
            }
            logger.LogDebug("User can't update comment");

            logger.LogDebug("CommentController end call -> return Unauthorized");

            return Unauthorized();
        }

        /// <summary>
        ///     Deletes a comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns Status 200 OK in case it works or a not found in case the model is not found by that ID.</returns>
        [HttpDelete("{id}")]
        [AuthorizeTdpFilter(PermissionType.Comment_Management)]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogDebug("CommentController called Delete");

            var existingComment = await repository.FindOneAsync<Comment>(id);
            var existingCommentModel = mapper.Map<Comment, CommentModel>(existingComment);
            if (existingCommentModel.IsNull())
            {
                logger.LogDebug("No comment found");

                logger.LogDebug("CommentController end call Delete -> return Not found");

                return NotFound();
            }

            var userUniqueIdentifier = userService.GetUserIdentifier((ClaimsIdentity)User.Identity);
            // If the user perfoming the action is the same as the user who created the comment he CAN delete the comment || user has permssions to delete
            if ((userUniqueIdentifier.Equals(existingComment.User))
                || userService.GetUserPermissions((ClaimsIdentity)User.Identity).Any(c => c.Value == PermissionType.Plan_Comment.GetDescription()))
            {
                repository.Remove(id);

                logger.LogDebug("CommentController end call Delete -> return Ok");

                return Ok();
            }
            logger.LogDebug("User can't update comment");

            logger.LogDebug("CommentController end call Delete -> return Unauthorized");

            return Unauthorized();
        }
    }
}