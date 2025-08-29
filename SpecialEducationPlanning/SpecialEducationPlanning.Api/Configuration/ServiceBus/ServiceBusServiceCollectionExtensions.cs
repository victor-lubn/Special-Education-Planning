using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Api.Service.ServiceBus;

namespace SpecialEducationPlanning
.Api.Configuration.ServiceBus
{
    public static class ServiceBusServiceCollectionExtensions
    {
        public static void AddAzureServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServiceBusConfiguration>(configuration.GetSection(ServiceBusConfiguration.SectionName));
            services.AddScoped<IServiceBusService, ServiceBusService>();

            var serviceBusConfiguration = configuration.GetSection(ServiceBusConfiguration.SectionName).Get<ServiceBusConfiguration>();
             services.AddAzureClients(builder =>
             {
                 builder.UseCredential(new ManagedIdentityCredential());

                 if (!string.IsNullOrWhiteSpace(serviceBusConfiguration.ConnectionString))
                {
                    builder.AddServiceBusClient(serviceBusConfiguration.ConnectionString)
                        .ConfigureOptions((options, provider) =>
                        {
                            // options.RetryOptions.MaxRetries = serviceBusConfiguration.MaxRetries;
                        });
                }
                else if (!string.IsNullOrWhiteSpace(serviceBusConfiguration.FullyQualifiedNamespace))
                {
                    builder.AddServiceBusClientWithNamespace(serviceBusConfiguration.FullyQualifiedNamespace)
                        .ConfigureOptions((options, provider) =>
                        {
                            // options.RetryOptions.MaxRetries = serviceBusConfiguration.MaxRetries;
                        });
                }
            });
        }
    }
}
