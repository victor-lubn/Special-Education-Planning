using System;
using AutoMapper;
using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Mapper
{

    /// <summary>
    /// Core AutoMapper profile provided by KOA
    /// </summary>
    /// <typeparam name="TModel">Model to map</typeparam>
    /// <typeparam name="TEntity">Entity to map </typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public abstract class BaseProfile<TModel, TEntity, TKey> : Profile
        where TModel : class, IModel<TKey>
        where TEntity : class, IEntity<TKey>

    {

        /// <summary>
        /// Creates a new instance of <see cref="BaseProfile{TModel, TEntity, TKey}"/>
        /// </summary>
        protected BaseProfile()
        {
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor
            _ = MapEntityToModel();
            _ = MapModelToEntity();
#pragma warning restore RECS0021 // Warns about calls to virtual member functions occuring in the constructor
        }

        /// <summary>
        /// Creates a <typeparamref name="TEntity"/> to <typeparamref name="TModel"/> mapping
        /// </summary>
        /// <returns>Mapping expression provided by <see cref="Automapper"/></returns>
        protected virtual IMappingExpression<TEntity, TModel> MapEntityToModel()
        {
            return CreateMap<TEntity, TModel>();
        }


        /// <summary>
        /// Creates a <typeparamref name="TModel"/> to <typeparamref name="TEntity"/> mapping
        /// During the transformation, this service invokes an EntityProvider in order to try finding an existing <typeparamref name="TEntity"/>
        /// by <typeparamref name="TKey"/>
        /// </summary>
        /// <returns>A <typeparamref name="TModel"/> to <typeparamref name="TEntity"/> mapping expression that relays on <see cref="IEntityProvider{TKey}"/></returns>
        protected virtual IMappingExpression<TModel, TEntity> MapModelToEntity()
        {
            return CreateMap<TModel, TEntity>();
        }
    }

    /// <summary>
    /// Default KOA AutoMapper profile
    /// </summary>
    /// <typeparam name="TModel">Model to map</typeparam>
    /// <typeparam name="TEntity">Entity to map</typeparam>
    public abstract class BaseProfile<TModel, TEntity> : BaseProfile<TModel, TEntity, int>
        where TModel : class, IModel<int>
        where TEntity : class, IEntity<int>
    {
    }
}