using Koa.Domain;
using Koa.Domain.Search.Page;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.QueryResult;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class, IEntity<int>
    {
        Task<TEntity> Add(TEntity entity);
        TEntity Update(TEntity entity);
        void Remove(TEntity entity);
        IEnumerable<TEntity> GetAll<T>() where T : class, IObject<int>;
        Task<IEnumerable<TEntity>> GetAllAsync<T>(CancellationToken cancellationToken = default) where T : class, IObject<int>;
        IEnumerable<TEntity> Where(ISpecification<TEntity> specification);
        Task<IEnumerable<TEntity>> WhereAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        bool Any(ISpecification<TEntity> specification);
        Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        int Count();
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        IPagedQueryResult<TEntity> Query(IPageDescriptor pageDescriptor);
        Task<IPagedQueryResult<TEntity>> QueryAsync(IPageDescriptor pageDescriptor, CancellationToken cancellationToken = default);
        IPagedQueryResult<TEntity> Query(IPageDescriptor pageDescriptor, ISpecification<TEntity> entitySpecification);
        Task<IPagedQueryResult<TEntity>> QueryAsync(IPageDescriptor pageDescriptor, ISpecification<TEntity> entitySpecification, CancellationToken cancellationToken = default);


        T FindOne<T>(int id) where T : class, IObject<int>;
        Task<T> FindOneAsync<T>(int id, CancellationToken cancellationToken = default) where T : class, IObject<int>;
        Task<TEntity> ApplyChangesAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Remove(int entity);
        Task RemoveAsync(int entity, CancellationToken cancellationToken = default);
        Task<bool> CheckIfExistsAsync(int id);
        Task<TEntity> GetWithNavigationsAsync<TModel>(int entityId) where TModel : class, IObject<int>;
        Task<TEntity> GetWithNavigationsAsync<TModel>(int entityId, IEnumerable<string> navigations) where TModel : class, IObject<int>;
        Task<int> CountNoFiltersAsync();
        Task<TEntity> DetachEntity(TEntity entity);
    }
}
