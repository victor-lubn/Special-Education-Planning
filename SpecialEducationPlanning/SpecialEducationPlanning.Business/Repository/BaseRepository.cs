using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Koa.Domain;
using Koa.Domain.Search.Page;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.Query;
using Koa.Persistence.Abstractions.QueryResult;
using LaunchDarkly.Sdk;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Specification;

namespace Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class, IEntity<int>, IObject<int>
    {
        protected readonly IEntityRepository repository;

        protected readonly IDbContextAccessor dbContextAccessor;



        protected BaseRepository(
            ILogger<BaseRepository<TEntity>> logger,
            IEntityRepository repository,
            IUnitOfWork unitOfWork,
            ISpecificationBuilder specificationBuilder,
            IEntityRepository<int> repositoryInt,
            IDbContextAccessor dbContextAccessor)
        {
            this.Logger = logger;
            this.repository = repository;
            this.Repository = repositoryInt;
            this.UnitOfWork = unitOfWork;
            this.SpecificationBuilder = specificationBuilder;
            this.dbContextAccessor = dbContextAccessor;
        }

        protected ILogger<BaseRepository<TEntity>> Logger { get; }
        protected IUnitOfWork UnitOfWork { get; }
        protected ISpecificationBuilder SpecificationBuilder { get; }
        protected IEntityRepository<int> Repository { get; }

        public async Task<TEntity> DetachEntity(TEntity entity)
        {
            var context = this.dbContextAccessor.GetCurrentContext();

            if (context.Entry(entity).State == EntityState.Unchanged)
            {
                context.Entry(entity).State = EntityState.Detached;
            }
            return (await Task.FromResult(entity));
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            await this.UnitOfWork.BeginTransactionAsync();
            this.repository.Add(entity);
            await this.UnitOfWork.CommitAsync();
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            this.dbContextAccessor.GetCurrentContext().Update<TEntity>(entity);
            this.UnitOfWork.Commit();
            return entity;
        }

        public void Remove(TEntity entity) 
        {
            this.repository.Remove(entity);
        }

        public IEnumerable<TEntity> GetAll<T>() where T : class, IObject<int>
        {
            var entities = this.repository.GetAll<TEntity>().ToArray();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class, IObject<int>
        {
            var entities = await this.repository.GetAll<TEntity>().ToArrayAsync(cancellationToken);
            return entities;
        }

        public IEnumerable<TEntity> Where(ISpecification<TEntity> specification) 
        {
            var entities = this.repository.Where(specification).ToArray();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> WhereAsync(ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default)
        {
            var entities = await this.repository.Where(specification).ToArrayAsync(cancellationToken);
            return entities;
        }

        public bool Any(ISpecification<TEntity> specification)
        {
            return this.repository.Any(specification);
        }

        public Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        {
            return this.repository.AnyAsync(specification, cancellationToken);
        }

        public int Count()
        {
            return this.repository.Count<TEntity>();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return this.repository.CountAsync<TEntity>(cancellationToken);
        }

        public IPagedQueryResult<TEntity> Query(IPageDescriptor pageDescriptor)
        {
            return this.Query(pageDescriptor, null);
        }

        public async Task<IPagedQueryResult<TEntity>> QueryAsync(IPageDescriptor pageDescriptor,
            CancellationToken cancellationToken = default) 
        {
            return await this.QueryAsync(pageDescriptor, null, cancellationToken);
        }

        public IPagedQueryResult<TEntity> Query(IPageDescriptor pageDescriptor,
            ISpecification<TEntity> entitySpecification)
        { 
            if (pageDescriptor == null)
            {
                throw new ArgumentNullException(nameof(pageDescriptor));
            }

            var spec = this.SpecificationBuilder.Create<TEntity>(pageDescriptor.Filters);
            if (entitySpecification != null)
            {
                spec = entitySpecification.And(spec);
            }

            var entityQuery = new EntityPagedValueQuery<TEntity>(spec, pageDescriptor.Take, pageDescriptor.Skip)
            {
                Sorts = pageDescriptor.Sorts
            };

            var entityQueryResult = this.repository.Query(entityQuery);
            var modelsQueryResult = new PagedQueryResult<TEntity>(entityQueryResult.Result, entityQueryResult.Take, entityQueryResult.Skip,
                entityQueryResult.Total);
            return modelsQueryResult;
        }

        public async Task<IPagedQueryResult<TEntity>> QueryAsync(IPageDescriptor pageDescriptor,
            ISpecification<TEntity> entitySpecification, CancellationToken cancellationToken = default)
        {
            if (pageDescriptor == null)
            {
                throw new ArgumentNullException(nameof(pageDescriptor));
            }

            var spec = this.SpecificationBuilder.Create<TEntity>(pageDescriptor.Filters);
            if (entitySpecification != null)
            {
                spec = entitySpecification.And(spec);
            }

            var entityQuery = new EntityPagedValueQuery<TEntity>(spec, pageDescriptor.Take, pageDescriptor.Skip)
            {
                Sorts = pageDescriptor.Sorts
            };

            var queryResult = await this.repository.QueryAsync(entityQuery, cancellationToken);
            var result = new PagedQueryResult<TEntity>(queryResult.Result, queryResult.Take, queryResult.Skip, queryResult.Total);
            return result;
        }

        public T FindOne<T>(int id) where T : class, IObject<int>
        {
            var entity = this.Repository.FindOne<T>(id);
            return entity;
        }

        public async Task<T> FindOneAsync<T>(int id, CancellationToken cancellationToken = default) where T : class, IObject<int>
        {
            var entity = await this.Repository.FindOneAsync<T>(id, cancellationToken);
            return entity;
        }
        
        public async Task<TEntity> ApplyChangesAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await this.UnitOfWork.CommitAsync(cancellationToken);
            return entity;
        }

        public void Remove(int id)
        {
            this.Repository.Remove<TEntity>(id);
            this.UnitOfWork.Commit();
        }

        public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            await this.Repository.RemoveAsync<TEntity>(id, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);
        }

        public async Task<TEntity> GetWithNavigationsAsync<TModel>(int entityId) where TModel : class, IObject<int>
        {
            var spec = Specification<Country>.True;
            var entity = await repository.Where(new EntityByIdSpecification<TEntity>(entityId))
                .IncludeNavigations<TEntity, TModel>(dbContextAccessor).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<bool> CheckIfExistsAsync(int id)
        {
            var spec = new EntityByIdSpecification<TEntity>(id);
            var exists = await repository.AnyAsync(spec);
            return exists;
        }
        
        public async Task<TEntity> GetWithNavigationsAsync<TModel>(int entityId, IEnumerable<string> navigations) where TModel : class, IObject<int>
        {
            var spec = Specification<Country>.True;
            var entity = await repository.Where(new EntityByIdSpecification<TEntity>(entityId))
                .IncludeNavigations<TEntity, TModel>(dbContextAccessor, navigations).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<int> CountNoFiltersAsync()
        {
            return await repository.GetAll<TEntity>().IgnoreQueryFilters().CountAsync();
        }
    }
}


    
