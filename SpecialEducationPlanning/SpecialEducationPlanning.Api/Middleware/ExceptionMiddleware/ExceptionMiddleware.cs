using Koa.Serialization.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model;

namespace SpecialEducationPlanning
.Api.Middleware.ExceptionMiddleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IJsonSerializer serializer;
        private readonly ExceptionMiddlewareOptions options;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IOptions<ExceptionMiddlewareOptions> options, IJsonSerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
            this.options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (this.options.EnableExceptionLogging)
                {
                    this.logger.LogError(ex, "Exception throw calling to {action}", context.Request?.Path);
                }

                await this.HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            if (exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
            }
            else if (exception is EndpointNotFoundException || exception is CommunicationException)
            {
                code = HttpStatusCode.ServiceUnavailable;
            }
            else if (exception is ArgumentException)
            {
                code = HttpStatusCode.BadRequest;
            }
            else if (exception.Source == "Microsoft.AspNetCore.Authentication.Core")
            {
                code = HttpStatusCode.Forbidden;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var message = this.options.EnableExceptionTrace ?
                (object)new TraceErrorDetail
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error.",
                    Exception = this.GetExceptionDetails(exception)
                } :
                new ProblemDetailsModel
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error."
                };
            var body = await this.serializer.SerializeAsync(message);
            await context.Response.WriteAsync(body);
        }

        private ExceptionDetail GetExceptionDetails(Exception exception)
        {
            var ex = exception.InnerException == null
                ? new ExceptionDetail
                {
                    Type = exception.GetType().FullName,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                }
                : new TraceExceptionDetail
                {
                    Type = exception.GetType().FullName,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerException = this.GetExceptionDetails(exception.InnerException)
                };
            return ex;
        }

        internal class TraceErrorDetail : ProblemDetailsModel
        {
            public ExceptionDetail Exception { get; set; }
        }

        internal class ExceptionDetail
        {
            public string Type { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
        }

        internal class TraceExceptionDetail : ExceptionDetail
        {
            public ExceptionDetail InnerException { get; set; }
        }
    }
}
