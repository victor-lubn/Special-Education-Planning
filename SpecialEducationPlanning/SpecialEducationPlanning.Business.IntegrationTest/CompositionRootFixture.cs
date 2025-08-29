using System;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Business.IntegrationTest;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Repository.ReleaseNoteRepository;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Service.Search;
using Koa.Domain.Specification.Search;

using Moq;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Business.Test
{
    public class CompositionRootFixture
    {
        protected readonly IServiceCollection Services;

        public CompositionRootFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", false);
            Configuration = builder.Build();
            Services = new ServiceCollection();
            ConfigureServices();
        }

        public IServiceProvider ServiceProvider { get; private set; }
        public IConfigurationRoot Configuration { get; }
        
        protected ILoggerFactory LoggerFactory;

        public void Init()
        {
            ServiceProvider = Services.BuildServiceProvider();
            Configure();
        }

        private void Configure()
        {
        }

        private void ConfigureServices()
        {
            // Add Koa services
            var dbContextType = typeof(DataContext);
            var domainAssembly = dbContextType.GetTypeInfo().Assembly;
            var repositoryType = typeof(IEducationerRepository);
            var businessAssembly = repositoryType.GetTypeInfo().Assembly;
            Services.AddDateTimeProvider();
            Services.AddIdentityProviderForTest(this.ServiceProvider);
            Services.AddKoaEntityFrameworkForTest<DataContext>(businessAssembly,
                opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            Services.AddKoaAutoMapperForTest(businessAssembly);
            Services.AddTradingAutoMapperForTest(businessAssembly);
            Services.AddTransient(x => ServiceProvider);
            Services.AddHttpContextAccessor();

            //add services
            var mockFileStorage = new Mock<IFileStorageService<AzureStorageConfiguration>>(MockBehavior.Default);
            Services.Add(new ServiceDescriptor(typeof(IFileStorageService<AzureStorageConfiguration>), mapper => mockFileStorage.Object, ServiceLifetime.Scoped));

            var mockPostcode = new Mock<IPostCodeServiceFactory>(MockBehavior.Default);
            Services.Add(new ServiceDescriptor(typeof(IPostCodeServiceFactory), mapper => mockPostcode.Object, ServiceLifetime.Scoped));

            //add repositories
            Services.AddScoped<IBuilderRepository, BuilderRepository>();
            Services.AddScoped<ICsvFileRepository, CsvFileRepository>();
            Services.AddScoped<IAreaRepository, AreaRepository>();
            Services.AddScoped<IReleaseInfoRepository, ReleaseInfoRepository>();
            Services.AddScoped<IRoleRepository, RoleRepository>();
            Services.AddScoped<IPermissionRepository, PermissionRepository>();
            Services.AddScoped<IEducationerRepository, EducationerRepository>();
            Services.AddScoped<IVersionRepository, VersionRepository>();
            Services.AddScoped<IReportRepository, ReportRepository>();
            Services.AddScoped<ISoundtrackRepository, SoundtrackRepository>();
            Services.AddScoped<IAiepRepository, AiepRepository>();
            Services.AddScoped<IProjectRepository, ProjectRepository>();
            Services.AddScoped<IPlanRepository, PlanRepository>();
            Services.AddScoped<IRegionRepository, RegionRepository>();
            Services.AddScoped<IActionRepository, ActionRepository>();
            Services.AddScoped<ICommentRepository, CommentRepository>();
            Services.AddScoped<IEndUserRepository, EndUserRepository>();
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<IDbExecutionStrategy, SqlAzureExecutionStrategy>();
            Services.AddScoped<IHouseSpecificationRepository, HouseSpecificationRepository>();

            //configure the logging
            this.LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            Services.Add(new ServiceDescriptor(typeof(ILogger<BuilderRepository>), logger => this.LoggerFactory.CreateLogger<BuilderRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<AreaRepository>), logger => this.LoggerFactory.CreateLogger<AreaRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<DataContext>), logger => this.LoggerFactory.CreateLogger<DataContext>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<DataContext>), logger => this.LoggerFactory.CreateLogger<DataContext>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<ReleaseInfoRepository>), logger => this.LoggerFactory.CreateLogger<ReleaseInfoRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<RoleRepository>), logger => this.LoggerFactory.CreateLogger<RoleRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<PermissionRepository>), logger => this.LoggerFactory.CreateLogger<PermissionRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<EducationerRepository>), logger => this.LoggerFactory.CreateLogger<EducationerRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<VersionRepository>), logger => this.LoggerFactory.CreateLogger<VersionRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<ReportRepository>), logger => this.LoggerFactory.CreateLogger<ReportRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<SoundtrackRepository>), logger => this.LoggerFactory.CreateLogger<SoundtrackRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<AiepRepository>), logger => this.LoggerFactory.CreateLogger<AiepRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<ProjectRepository>), logger => this.LoggerFactory.CreateLogger<ProjectRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<PlanRepository>), logger => this.LoggerFactory.CreateLogger<PlanRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<RegionRepository>), logger => this.LoggerFactory.CreateLogger<RegionRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<ActionRepository>), logger => this.LoggerFactory.CreateLogger<ActionRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<CommentRepository>), logger => this.LoggerFactory.CreateLogger<CommentRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<CsvFileRepository>), logger => this.LoggerFactory.CreateLogger<CsvFileRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<EndUserRepository>), logger => this.LoggerFactory.CreateLogger<EndUserRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<UserRepository>), logger => this.LoggerFactory.CreateLogger<UserRepository>(), ServiceLifetime.Scoped));
            Services.Add(new ServiceDescriptor(typeof(ILogger<HouseSpecificationRepository>), logger => this.LoggerFactory.CreateLogger<HouseSpecificationRepository>(), ServiceLifetime.Scoped));

            //configure Azure Search
            Services.AddScoped<IAzureSearchManagementService, DummyAzureSearchManagementService>();

            //configure SpecificationBuilder
            Services.Add(new ServiceDescriptor(typeof(ISpecificationBuilder), spec => new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()), ServiceLifetime.Scoped));
        }
    }
}

