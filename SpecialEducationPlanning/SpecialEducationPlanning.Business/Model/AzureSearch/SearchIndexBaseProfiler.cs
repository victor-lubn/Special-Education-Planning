using AutoMapper;
using Koa.Domain;
using System;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

namespace SpecialEducationPlanning
.Business.Model.AzureSearch
{

    public abstract class SearchIndexBaseProfiler<TModel, TEntity> : Profile where TModel : SearchBaseIndexModel
            where TEntity : class, ISearchable<int>
    {

        protected SearchIndexBaseProfiler()
        {
            MapEntityToModel();
            MapModelToEntity();
        }

        #region Methods Protected

        protected virtual IMappingExpression<TEntity, TModel> MapEntityToModel()
        {
            return CreateMap<TEntity, TModel>();
        }


        protected virtual IMappingExpression<TModel, TEntity> MapModelToEntity()
        {
            return CreateMap<TModel, TEntity>();
        }

        #endregion

    }

}