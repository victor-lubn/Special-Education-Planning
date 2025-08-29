using SpecialEducationPlanning
.Api.Service.CsvFile;
using SpecialEducationPlanning
.Api.Service.CsvFile.Implementation;
using SpecialEducationPlanning
.Business.Repository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CsvFileExtension
    {
        /// <summary>
        /// Makes the necessary injections for all the CsvFile service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCsvFileInjections(this IServiceCollection services)
        {

            services.AddScoped<ICsv, UserCsv>();
            services.AddScoped<ICsv, PlanCsv>();
            services.AddScoped<ICsv, AiepCsv>();
            services.AddScoped<ICsvFileService, CsvFileService>();
            services.AddScoped<ICsvFileRepository, CsvFileRepository>();
            return services;
        }
    }
}

