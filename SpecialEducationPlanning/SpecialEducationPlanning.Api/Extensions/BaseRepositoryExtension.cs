using System;
using System.Linq;
using System.Reflection;
using Koa.Domain;
using Koa.Domain.Specification.Search;
using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Business.Repository;
using TypeExtensions = System.TypeExtensions;
namespace SpecialEducationPlanning
.Api.Extensions
{
    public static class BaseRepositoryExtension
    {

        public static IServiceCollection AddBaseRepositories(this IServiceCollection services, Assembly baseRepositoryAssembly)
        {
            services.AddBaseRepositories(baseRepositoryAssembly, ServiceLifetime.Scoped);
            services.AddSingleton<ISpecificationBuilder, SpecificationBuilder>();
            return services;
        }

        public static IServiceCollection AddBaseRepositories(this IServiceCollection services, Assembly mapperRepositoryAssembly, ServiceLifetime lifetime)
        {
            var baseType = typeof(IBaseRepository<>);

            var candidates = mapperRepositoryAssembly.ExportedTypes
                .Where(x => x.GetInterfaces()
                             .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType)
                             && !x.GetTypeInfo().IsAbstract)
                .ToArray();

            foreach (var candidate in candidates)
            {
                var @interface = candidate.GetInterfaces().FirstOrDefault(i => i.Name.Equals($"I{candidate.Name}"));
                if (@interface != null)
                {
                    services.Add(new ServiceDescriptor(@interface, candidate, lifetime));
                }

            }

            return services;
        }
    }
}
