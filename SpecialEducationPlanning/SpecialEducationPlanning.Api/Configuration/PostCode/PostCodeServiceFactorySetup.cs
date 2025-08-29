using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using SpecialEducationPlanning
.Api.Configuration.HttpClient;
using SpecialEducationPlanning
.Api.Service.PostCode;
using SpecialEducationPlanning
.Business.IService;

namespace SpecialEducationPlanning
.Api.Configuration.PostCode
{
    public static class PostCodeServiceFactorySetup
    {
        public static void AddPostCodeServiceFactory(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IPostCodeServiceFactory, PostCodeServiceFactory>()
                    .ConfigurePolicies(config);
        }
    }
}
