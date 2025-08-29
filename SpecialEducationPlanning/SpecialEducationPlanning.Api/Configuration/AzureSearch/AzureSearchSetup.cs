using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Api.Service.Search;
using SpecialEducationPlanning
.Domain.Service.Search;

namespace SpecialEducationPlanning
.Api.Configuration.AzureSearch
{
    public static class AzureSearchSetup
    {
        public static void AddAzureSearch(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue(AzureSearchConfiguration.SectionEnabledName, false))
            {
                //logger.LogWarning("Configuring Azure Search...");
                var azureSearchConfiguration = configuration.GetSection(AzureSearchConfiguration.SectionName).Get<AzureSearchConfiguration>();
                services.Configure<AzureSearchConfiguration>(configuration.GetSection(AzureSearchConfiguration.SectionName));


                if (azureSearchConfiguration.ManagerEnabled)
                {
                    //logger.LogInformation("Configuring Azure Search Manager...");
                    services.AddScoped<IAzureSearchManagementService, AzureSearchManagementService>();
                    services.AddSingleton<AzureSearchClientPool>();
                }
                else
                {
                    //logger.LogInformation("Azure Search Manager not enabled...");
                    services.AddScoped<IAzureSearchManagementService, DummyAzureSearchManagementService>();
                }

                services.AddScoped<IAzureSearchService, AzureSearchService>();
            }
            else
            {
                //logger.LogWarning("Azure Search not enabled...");
                services.AddScoped<IAzureSearchService, DummyAzureSearchService>();
                services.AddScoped<IAzureSearchManagementService, DummyAzureSearchManagementService>();
            }
        }
    }
}
