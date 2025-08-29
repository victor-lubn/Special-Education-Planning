using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Api.Attributes;
using SpecialEducationPlanning
.Api.Constants;
using SpecialEducationPlanning
.Api.Model;

namespace SpecialEducationPlanning
.Api.ActionFilter
{
    public class DefineHeaderValidationFilter : IActionFilter
    {
        private readonly ILogger<DefineHeaderValidationFilter> _logger;

        public DefineHeaderValidationFilter(
            ILogger<DefineHeaderValidationFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headerAttributes = context.ActionDescriptor.EndpointMetadata
                .OfType<DefineHeaderAttribute>()
                .ToList();

            if (headerAttributes.Any())
            {
                var missingHeaders = new List<ErrorIssue>();

                foreach (var attribute in headerAttributes)
                {
                    if (attribute.IsRequired && !context.HttpContext.Request.Headers.ContainsKey(attribute.Name))
                    {
                        missingHeaders.Add(new ErrorIssue
                        {
                            FieldName = attribute.Name,
                            Error = $"The {attribute.Name} header is required."
                        });

                        _logger.LogInformation($"Missing required header: '{attribute.Name}' for {context.HttpContext.Request.Path}");
                    }
                }

                if (missingHeaders.Any())
                {
                    context.Result = BuildBadRequestResponse(missingHeaders);
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action after execution
        }

        private ObjectResult BuildBadRequestResponse(List<ErrorIssue> issues)
        {
            var errorResponse = new ProblemDetailsModel(ErrorTitle.BadRequest, StatusCodes.Status400BadRequest, ErrorMessage.InvalidRequest)
            {
                Issues = issues
            };

            return new ObjectResult(errorResponse)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
