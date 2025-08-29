using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Api.Constants;
using SpecialEducationPlanning
.Api.Model;

namespace SpecialEducationPlanning
.Api.Configuration.Authorization
{
    public class ApiKeyAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyAuthorizeFilter> _logger;
        private readonly string _configurationKey;

        public ApiKeyAuthorizeFilter(
            IConfiguration configuration,
            ILogger<ApiKeyAuthorizeFilter> logger,
            string configurationKey)
        {
            _configuration = configuration;
            _configurationKey = configurationKey;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(CustomHeaders.aiepApiKey, out var providedApiKey))
            {
                var message = $"Missing required API key header: '{CustomHeaders.aiepApiKey}'";
                _logger.LogWarning(message);
                context.Result = BuildUnauthorizedResponse(message);
                return;
            }

            var expectedApiKey = _configuration.GetValue<string>(_configurationKey);
            if (string.IsNullOrWhiteSpace(expectedApiKey) || providedApiKey != expectedApiKey)
            {
                var message = $"Invalid API key provided for header: '{CustomHeaders.aiepApiKey}'";
                _logger.LogWarning(message);
                context.Result = BuildUnauthorizedResponse(message);
                return;
            }

            _logger.LogDebug($"API key validated successfully for endpoint: {context.HttpContext.Request.Path}");
        }

        private ObjectResult BuildUnauthorizedResponse(string message)
        {
            var errorResponse = new ProblemDetailsModel
            {
                Title = "Unauthorized",
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = message
            };

            return new ObjectResult(errorResponse)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }
}
