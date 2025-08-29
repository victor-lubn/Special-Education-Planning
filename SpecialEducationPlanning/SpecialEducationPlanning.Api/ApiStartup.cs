using Koa.Hosting.AspNetCore.Delta;
using Koa.Hosting.AspNetCore.Formatting;
using Koa.Hosting.AspNetCore.ModelBinding;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.DependencyInjection;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using SoapCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using SpecialEducationPlanning
.Api.ActionFilter;
using SpecialEducationPlanning
.Api.Configuration;
using SpecialEducationPlanning
.Api.Configuration.Authorization;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Configuration.DataMigration;
using SpecialEducationPlanning
.Api.Configuration.DistributedCache;
using SpecialEducationPlanning
.Api.Configuration.FeatureManagement;
using SpecialEducationPlanning
.Api.Configuration.FittersPack;
using SpecialEducationPlanning
.Api.Configuration.HangfireCfg;
using SpecialEducationPlanning
.Api.Configuration.HttpClient;
using SpecialEducationPlanning
.Api.Configuration.PostCode;
using SpecialEducationPlanning
.Api.Configuration.PublishProjectService;
using SpecialEducationPlanning
.Api.Configuration.PublishSystemService;
using SpecialEducationPlanning
.Api.Configuration.ServiceBus;
using SpecialEducationPlanning
.Api.Configuration.Strategy;
using SpecialEducationPlanning
.Api.Configuration.ThreeDcApi;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Middleware.ExceptionMiddleware;
using SpecialEducationPlanning
.Api.Middleware.PermissionMiddleware;
using SpecialEducationPlanning
.Api.Model.AutomaticArchiveModel;
using SpecialEducationPlanning
.Api.Model.AutomaticRemoveItems;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Api.Model.SapConfiguration;
using SpecialEducationPlanning
.Api.Model.UserInfoModel;
using SpecialEducationPlanning
.Api.Service.ApimToken;
using SpecialEducationPlanning
.Api.Service.DistributedCache;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.FileStorage;
using SpecialEducationPlanning
.Api.Service.FittersPack;
using SpecialEducationPlanning
.Api.Service.OmniSearch;
using SpecialEducationPlanning
.Api.Service.PostCode;
using SpecialEducationPlanning
.Api.Service.ProjectService;
using SpecialEducationPlanning
.Api.Service.Publish;
using SpecialEducationPlanning
.Api.Service.Sap;
using SpecialEducationPlanning
.Api.Service.ThreeDc;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Api.Services.DataMigration;
using SpecialEducationPlanning
.Api.SOAP;
using SpecialEducationPlanning
.Business;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.FileStorageModel.Base;
using SpecialEducationPlanning
.Business.Strategy.PostCode;
using SpecialEducationPlanning
.Domain;


namespace SpecialEducationPlanning
.Api
{
    public static class ApiStartup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public static void AddApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> configureDataContext)
        {
            services.AddTransient<IProjectService, ProjectService>();

            // Add AspNetCore framework services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc(setup =>
            {
                setup.Filters.Add<DefineHeaderValidationFilter>();

                // configure mvc options
                setup.ModelBinderProviders.Insert(0, new WebApiPagedModelBinderProvider());
                setup.InputFormatters.Insert(0, new MultiUploadedFileModelFormatter());
                setup.InputFormatters.Insert(0, new MultiUploadedFileModelFormatter<VersionInfoModel>());
            })
                .AddApplicationPart(typeof(ApiStartup).Assembly)
                .AddControllersAsServices()
                .AddDelta()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddApiAuthorization();
            services.AddExceptionMiddleware();

            // Add Koa services
            var businessPointerType = typeof(BusinessPointer);
            var businessAssembly = businessPointerType.GetTypeInfo().Assembly;

            services.AddDateTimeProvider();
            services.AddIdentityProvider();
            services.AddKoaEntityFramework<DataContext>(configureDataContext);
            services.AddAutoMapperWrapper(businessAssembly);
            services.AddBaseRepositories(businessAssembly);
            services.AddHttpContextDependencies();
            services.AddTextJson();

            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(ApiStartup).Assembly);
            });

            services.RegisterAzureStorageConfigurations(config, businessAssembly);

            //aiep services
            services.AddScoped<ISoapSapService, SoapSapService>();
            services.Configure<OracleConfiguration>(config.GetSection("PublishServiceConfig:CadRendering"));
            services.Configure<DistributedCacheUserConfiguration>(config.GetSection("DistributedCache:User"));
            services.Configure<AutomaticArchiveConfiguration>(config.GetSection("Archive"));
            services.Configure<AutomaticRemoveActionConfiguration>(config.GetSection("ActionLogs"));
            services.Configure<AutomaticRemoveSystemConfiguration>(config.GetSection("SystemLogs"));
            services.Configure<List<PostCodeConfiguration>>(config.GetSection("PostCode"));
            services.Configure<SapConfiguration>(config.GetSection("Sap"));
            services.Configure<OmniSearchConfiguration>(config.GetSection("OmniSearch"));
            services.Configure<UserInfoUrlConfiguration>(config.GetSection("UserInfoUrl"));
            services.Configure<DataMigrationConfiguration>(config.GetSection("DataMigration"));
            services.Configure<PublishServiceConfiguration>(config.GetSection("PublishServiceConfig:PublishService"));
            services.Configure<PublishProjectServiceConfiguration>(config.GetSection(PublishProjectServiceConfiguration.Section));
            services.Configure<CountryConfiguration>(config.GetSection("Country"));
            services.Configure<Publish3DcServiceConfiguration>(config.GetSection("PublishServiceConfig:Publish3DcService"));
            services.Configure<FittersPackConfiguration>(config.GetSection("FittersPack"));
            services.Configure<ThreeDcApiRequestConfiguration>(config.GetSection(ThreeDcApiRequestConfiguration.Section));

            //Add LaunchDarkly Feature Management
            services.Configure<LaunchDarklyConfiguration>(config.GetSection("LaunchDarkly"));
            services.AddScoped<FeatureManagementCountryConfiguration>(s => new FeatureManagementCountryConfiguration(config.GetValue<string>("Country:StrategyIdentifier").ToUpper()));
            services.AddSingleton<ILdClient, LdClient>(s => new LdClient(config.GetValue<string>("LaunchDarkly:SDK_Key")));
            services.AddHttpClient<IFeatureManagementService, FeatureManagementService>().ConfigurePolicies(config);

            //---Publish Services
            //services.AddHttpClient<PublishService>().ConfigurePolicies(config);
            services.AddScoped<IPublishService, CadRendering>();
            services.AddHttpClient<IPublishProjectService, PublishProjectService>().ConfigurePolicies(config);

            // 3DC Api Service
            services.Configure<ThreeDcApiServiceConfiguration>(config.GetSection(ThreeDcApiServiceConfiguration.Section));
            services.AddHttpClient<IThreeDcApiService, ThreeDcApiService>().ConfigurePolicies(config);
            
            // GenerateApimTokenService
            services.AddHttpClient<IGenerateApimTokenService, GenerateApimTokenService>();

            services.AddAzureSearch(config);
            services.AddAzureServiceBus(config);
            services.AddScoped(typeof(IFileStorageService<>), typeof(FileStorageService<>));
            services.AddHttpClient<ISapService, SapService>().ConfigurePolicies(config);
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOmniSearchService, OmniSearchService>();
            services.AddScoped<AiepMigrationService, AiepMigrationService>();
            services.AddScoped<BuilderMigrationService, BuilderMigrationService>();
            services.AddScoped<PlanMigrationService, PlanMigrationService>();
            services.AddScoped<PlanItemMigrationService, PlanItemMigrationService>();
            services.AddScoped<IUserDistributedCacheService, UserDistributedCacheService>();
            services.AddStrategy<IPostCodeStrategy>(config.GetValue<string>("Country:StrategyIdentifier"));
            services.AddHttpClient<IPostCodeServiceFactory, PostCodeServiceFactory>().ConfigurePolicies(config);
            services.AddCsvFileInjections();
            services.AddSoapCore();
            services.AddUserClaimsMiddleware();
            services.AddHangfire(config);
            services.AddPostCodeServiceFactory(config);
            services.AddScoped<IDbExecutionStrategy, SqlAzureExecutionStrategy>();
            services.AddScoped<IPublish3DcService, Publish3DcService>();
            services.AddScoped<IFittersPackService, FittersPackService>();
            services.AddScoped<FittersPackHangfireJobs>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void UseApi(this IApplicationBuilder app, ILogger logger, IConfiguration config, Action<IEndpointRouteBuilder> configureEndpoints = default)
        {
            logger.LogDebug("Configure routing...");
            app.UseRouting();
            app.UseAuthentication();
            app.UseUserClaimsMiddleware();

            //Add Hangfire
            logger.LogDebug("Adding Hangfire....");
            app.UseHangfire(config, logger);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        public static void RegisterAzureStorageConfigurations(this IServiceCollection services, IConfiguration config, Assembly assembly)
        {
            var baseType = typeof(AzureStorageConfigurationBase);

            var configTypes = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

            foreach (var type in configTypes)
            {
                var sectionName = type.Name[..^"Configuration".Length];

                var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(IConfiguration) })!
                    .MakeGenericMethod(type);

                method.Invoke(null, new object[] { services, config.GetSection(sectionName) });
            }
        }
    }
}
