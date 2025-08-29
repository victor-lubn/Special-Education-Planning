namespace SpecialEducationPlanning
.Api.Middleware.ExceptionMiddleware
{
    public class ExceptionMiddlewareOptions
    {
        public const string Section = "ExceptionMiddleware";
        public bool EnableExceptionTrace { get; set; }

        public bool EnableExceptionLogging { get; set; }
    }
}