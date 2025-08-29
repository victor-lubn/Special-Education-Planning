using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpecialEducationPlanning
.Api.Host.Options.Swagger
{
    internal class BadRequestOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //var schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);

            var hasBadRequestResponse = context.MethodInfo.GetCustomAttributes(true)
                .OfType<ProducesResponseTypeAttribute>()
                .Any(attr => attr.StatusCode == StatusCodes.Status400BadRequest);
            if (hasBadRequestResponse)
            {
                return;
            }

            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request",
                //Reference = schema.Reference
            });
        }
    }
}