using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Api.Attributes;

namespace SpecialEducationPlanning
.Api.Host.Options.Swagger
{
    public class DefineHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var defineHeadersAttributes = context.MethodInfo
                .GetCustomAttributes(typeof(DefineHeaderAttribute), false)
                .Cast<DefineHeaderAttribute>()
                .ToList();

            if (!defineHeadersAttributes.Any())
            {
                return;
            }

            operation.Parameters ??= new List<OpenApiParameter>();

            foreach (var attribute in defineHeadersAttributes)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = attribute.Name,
                    In = ParameterLocation.Header,
                    Required = attribute.IsRequired,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Description = attribute.Description
                });
            }
        }
    }
}