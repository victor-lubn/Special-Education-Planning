using System;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Koa.Domain;
using Koa.Persistence.Abstractions;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Platform.Providers.Identity;
using Koa.Hosting.AspNetCore.Http;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.IntegrationTest
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddKoaEntityFrameworkForTest<TContext>(this IServiceCollection services, System.Reflection.Assembly businessAssembly, Action<DbContextOptionsBuilder> optionsAction = null) where TContext : DbContext
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddDbContext<TContext>(optionsAction, ServiceLifetime.Scoped);
            services.AddDbContext(typeof(TContext), ServiceLifetime.Scoped);
            services.AddDbContextAccessor(ServiceLifetime.Scoped);
            services.AddEfDbRepository(ServiceLifetime.Scoped);
            services.AddEfUnitOfWork(ServiceLifetime.Scoped);
            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, Type dbContextType, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(DbContext), provider => provider.GetRequiredService(dbContextType), lifetime));
            return services;
        }

        public static IServiceCollection AddDbContextAccessor(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(IDbContextAccessor), typeof(DbContextAccessor), lifetime));
            return services;
        }

        public static IServiceCollection AddEfDbRepository(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(IEfDbRepository), typeof(EntityRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(IEntityRepository), typeof(EntityRepository), lifetime));
            services.Add(new ServiceDescriptor(typeof(IEntityRepository<>), typeof(EntityRepository<>), lifetime));
            services.AddSingleton<IQueryableEvaluator, EfCoreQueryableEvaluator>();
            return services;
        }
        public static IServiceCollection AddEfUnitOfWork(this IServiceCollection services, ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(IEfUnitOfWork), typeof(EfUnitOfWork), lifetime));
            return services;
        }

        public static IServiceCollection AddKoaAutoMapperForTest(this IServiceCollection services, Assembly profileAssembly)
        {
            services.AddObjectMapper(ServiceLifetime.Scoped);
            services.AddAutoMapperWrapperForTest(profileAssembly);
            return services;
        }
        
        public static IServiceCollection AddTradingAutoMapperForTest(this IServiceCollection services, Assembly profileAssembly)
        {
            services.AddTradingObjectMapper(ServiceLifetime.Scoped);
            services.AddAutoMapperWrapperForTest(profileAssembly);
            return services;
        }

        /// <summary>
        ///     Creates a <a <see cref="AutoMapperObjectMapper" /> service with the given <see cref="ServiceLifetime" />
        /// </summary>
        /// <param name="services">Services to add the scoped <see cref="AutoMapperObjectMapper" /></param>
        /// <param name="lifetime">Object instance lifespan</param>
        /// <exception cref="ArgumentNullException">If <paramref name="services" /> is null</exception>
        /// <returns>Configured <paramref name="services" /></returns>
        public static IServiceCollection AddObjectMapper(this IServiceCollection services, ServiceLifetime lifetime)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(new ServiceDescriptor(typeof(SpecialEducationPlanning
.Business.Mapper.IObjectMapper), typeof(AutoMapperObjectMapper), lifetime));
            return services;
        }

        public static IServiceCollection AddTradingObjectMapper(this IServiceCollection services, ServiceLifetime lifetime)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(new ServiceDescriptor(typeof(SpecialEducationPlanning
.Business.Mapper.IObjectMapper), typeof(AutoMapperObjectMapper), lifetime));
            return services;
        }

        public static IServiceCollection AddIdentityProviderForTest(this IServiceCollection services, IServiceProvider serviceProvider)
        {
            return services.AddIdentityProvider(ServiceLifetime.Scoped, serviceProvider);
        }

        public static IServiceCollection AddIdentityProvider(this IServiceCollection services, ServiceLifetime lifetime, IServiceProvider serviceProvider)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(new ServiceDescriptor(typeof(IIdentityProvider), typeof(DummyIdentityProvider), lifetime));
            services.Add(new ServiceDescriptor(typeof(IPrincipalProvider), typeof(PrincipalProvider), lifetime));
            return services;
        }
    }
}
