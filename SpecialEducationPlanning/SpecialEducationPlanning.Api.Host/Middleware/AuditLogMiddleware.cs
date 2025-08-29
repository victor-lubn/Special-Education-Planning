using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Middlewares
{
    /// <summary>
    ///     A middleware to monitor requests
    /// </summary>
    public class AuditLogMiddleware
    {
        private readonly string excludeRegex;

        private readonly bool ignoreAuth;

        private readonly ILogger<AuditLogMiddleware> logger;

        private readonly bool logToDatabase;

        private readonly bool logToLogger;
        private readonly RequestDelegate next;

        /// <summary>
        ///     Create a new instance of <see cref="AuditLogMiddleware" />
        /// </summary>
        /// <param name="next">Next delegate in the pipeline</param>
        /// <param name="logger">Logger</param>
        /// <param name="config">Application configuration</param>
        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger, IConfiguration config)
        {
            this.next = next;
            this.logger = logger;
            excludeRegex = config.GetValue("Middleware:AuditLog:ExcludeRegex", ".*swagger");
            logToDatabase = config.GetValue("Middleware:AuditLog:DatabaseEnabled", true);
            logToLogger = config.GetValue("Middleware:AuditLog:LoggerEnabled", true);
            ignoreAuth = config.GetValue("Middleware:AuditLog:IgnoreAuth", false);
        }

        /// <summary>
        ///     Evaluates the action invoked and logs relevant information
        /// </summary>
        /// <param name="context">HttpContext of the invocation</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var ignore = Regex.IsMatch(context.Request.Path, excludeRegex);

            // If we should ignore this route, audit is not enabled or we don't have anything to log to...
            if (ignore || !logToDatabase && !logToLogger)
            {
                await next.Invoke(context);
                return;
            }


            var dateTimeUtcNow = DateTime.UtcNow;
            var httpQuery = string.Join(" ", context.Request.Method, context.Request.Path);
            var remoteAddress = string.Join(" ", context.Connection.RemoteIpAddress, context.Connection.RemotePort);
            var localAddress = string.Join(" ", context.Connection.LocalIpAddress, context.Connection.LocalPort);
            var user = context.User?.Identity.Name;

            string content = null;
            context.Request.EnableBuffering();
            if (context.Request.ContentLength > 0)
            {
                // Leave the body open so the next middleware can read it.
                using (var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: -1,
                    leaveOpen: true))
                {
                    content = await reader.ReadToEndAsync();
                    // Reset the request body stream position so the next middleware can read it
                    context.Request.Body.Position = 0;
                }
            }

            if (logToLogger)
            {
                var level = ignoreAuth || user != null ? LogLevel.Debug : LogLevel.Warning;
                logger.Log(level,
                    "AuditLog: {httpQuery} Authenticated: {authenticated} {remoteAddress} {localAddress} {user} {response}",
                    httpQuery, user != null, remoteAddress, localAddress, user, content);
            }

            await next.Invoke(context);
        }
    }
}