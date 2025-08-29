using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using SpecialEducationPlanning
.Api.Constants;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Controllers.Base
{
    public abstract class ControllerBaseExtended : Controller
    {
        protected readonly ILogger _logger;

        protected ControllerBaseExtended(ILogger logger)
        {
            _logger = logger;
        }

        protected IActionResult ValidateRequest()
        {
            var controllerName = ControllerContext?.ActionDescriptor?.ControllerName ?? "UnknownController";
            var actionName = ControllerContext?.ActionDescriptor?.ActionName ?? "UnknownAction";

            if (!ModelState.IsValid)
            {
                _logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(Environment.NewLine));
                _logger.LogDebug($"{controllerName} end call Post {actionName} -> return Bad request");

                return BadRequest(new ProblemDetailsModel(ErrorTitle.BadRequest, StatusCodes.Status400BadRequest, ErrorMessage.InvalidRequest)
                {
                    Issues = ModelState.GetErrorIssues()
                });
            }

            return null;
        }

        protected IActionResult ValidateResponse<T>(RepositoryResponse<T> response)
        {
            var controllerName = ControllerContext?.ActionDescriptor?.ControllerName ?? "UnknownController";
            var actionName = ControllerContext?.ActionDescriptor?.ActionName ?? "UnknownAction";

            if (response.ErrorList.Any())
            {
                _logger.LogDebug($"{controllerName} end call Post {actionName} -> return Bad request");

                return BadRequest(new ProblemDetailsModel(ErrorTitle.BadRequest, StatusCodes.Status400BadRequest, ErrorMessage.InvalidRequest)
                {
                    Issues = response.ErrorList.Select(s => new ErrorIssue
                    {
                        Error = s
                    }).ToList()
                });
            }

            if (response.Content == null)
            {
                _logger.LogDebug($"{controllerName} end call Post {actionName} -> return No found");

                return NotFound(new ProblemDetailsModel(ErrorTitle.EntityNotFound, StatusCodes.Status404NotFound, ErrorTitle.EntityNotFound));
            }

            return new OkObjectResult(response.Content);
        }
    }
}
