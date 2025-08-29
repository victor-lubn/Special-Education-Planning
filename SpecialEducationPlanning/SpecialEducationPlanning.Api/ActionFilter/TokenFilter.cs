using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.Authorization;
using Microsoft.Extensions.Options;

namespace SpecialEducationPlanning
.Api.Interceptors
{
    /// <summary>
    /// A token filter for basic service integration/access to TDP
    /// </summary>
    public class TokenFilter : ActionFilterAttribute
    {

        /// <summary>
        /// Class logger
        /// </summary>
        private ILogger<TokenFilter> logger;

        /// <summary>
        /// Token to use for validation
        /// </summary>
        private readonly string token;

        /// <summary>
        /// Is this filter enabled?
        /// </summary>
        private readonly bool enabled;

        /// <summary>
        /// Flag to bypass authorization filter for migration. Set to true to bypass.
        /// </summary>
        private readonly bool migrationAnonymous;

        private readonly string tokenKey;

        /// <summary>
        /// Creates a new instance of <see cref="TokenFilter"/>
        /// </summary>
        /// <param name="logger">Class logger</param>
        /// <param name="config">Authentication configuration</param>
        public TokenFilter(ILogger<TokenFilter> logger, IOptions<ApiAuthorizationOptions> config)
        {
            this.logger = logger;
            this.token = config.Value.BasicToken;
            this.tokenKey = config.Value.BasicTokenKey;
            this.enabled = config.Value.IsEnabled && !string.IsNullOrEmpty(this.token);
            this.migrationAnonymous = config.Value.MigrationAnonymous;
        }

        /// <summary>
        /// Check request headers looking for basic-token.
        /// </summary>
        /// <param name="context">Action context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (!this.IsAuthorizedByKey(context.Controller, context.HttpContext.Request.Headers))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            base.OnActionExecuting(context);
        }


        /// <summary>
        /// Check request headers looking for basic-token.
        /// </summary>
        /// <param name="context">Current action context</param>
        /// <param name="next">Next action</param>
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!this.IsAuthorizedByKey(context.Controller, context.HttpContext.Request.Headers))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            return base.OnActionExecutionAsync(context, next);
        }

        /// <summary>
        /// Checks if a request should execute, based on its header
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <param name="headers">Request headers</param>
        /// <returns></returns>
        private bool IsAuthorizedByKey(object controller, IHeaderDictionary headers)
        {
            if (!this.enabled) return true;
            if (this.migrationAnonymous) return true;
            if (headers.TryGetValue(this.tokenKey, out var value))
            {
                return this.token.Equals(value);
            }

            return false;
        }
    }
}
