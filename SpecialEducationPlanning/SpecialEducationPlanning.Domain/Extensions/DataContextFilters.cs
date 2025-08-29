using Koa.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class DataContextStoreProcedures
    {

        #region Methods Public

        public static void ApplyFilters(this DataContext dataContext, ModelBuilder modelBuilder)
        {
            var clrTypes = modelBuilder.Model.GetEntityTypes().Select(et => et.ClrType).ToList();


            foreach (var entityTypeCurrent in clrTypes)
            {
                var filters = new List<LambdaExpression>();

                if (typeof(IEntityWithAcl).IsAssignableFrom(entityTypeCurrent))
                {
                    filters.Add(LambdaAcl(dataContext, entityTypeCurrent));
                }

                if (typeof(ISoftDeleteEntity).IsAssignableFrom(entityTypeCurrent))
                {
                    filters.Add(LambdaSoftDelete(dataContext, entityTypeCurrent));
                }

                if (filters.Any())
                {
                    var queryFilter = CombineQueryFilters(entityTypeCurrent, filters);
                    modelBuilder.Entity(entityTypeCurrent).HasQueryFilter(queryFilter);
                }
            }
        }

        #endregion

        #region Methods Private

        private static LambdaExpression CombineQueryFilters(Type entityType,
            IEnumerable<LambdaExpression> andAlsoExpressions)
        {
            var newParam = Expression.Parameter(entityType);
            var andAlsoExprBase = (Expression<Func<IEntity, bool>>)(_ => true);

            var andAlsoExpr = ReplacingExpressionVisitor.Replace(andAlsoExprBase.Parameters.Single(), newParam, andAlsoExprBase.Body);

            foreach (var expressionBase in andAlsoExpressions)
            {
                var expression = ReplacingExpressionVisitor.Replace(expressionBase.Parameters.Single(), newParam,
                    expressionBase.Body);

                andAlsoExpr = Expression.AndAlso(andAlsoExpr, expression);
            }

            return Expression.Lambda(andAlsoExpr, newParam);
        }

        private static LambdaExpression ConvertFilterExpression<TInterface>(
            Expression<Func<TInterface, bool>> filterExpression,
            Type entityType)
        {
            var newParam = Expression.Parameter(entityType);

            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam,
                filterExpression.Body);

            return Expression.Lambda(newBody, newParam);
        }

        private static LambdaExpression LambdaAcl(DataContext dataContext, Type entityTypeCurrent)
        {
            return ConvertFilterExpression<IEntityWithAcl>(e => dataContext.CurrentUserFullAclAccess ||
                                                                dataContext.Set<Acl>().Any(acl =>
                                                                    acl.EntityId == e.Id &&
                                                                    acl.EntityType == entityTypeCurrent.Name &&
                                                                    acl.UserId == dataContext.CurrentUserId),
                entityTypeCurrent);
        }

        private static LambdaExpression LambdaSoftDelete(DataContext dataContext, Type entityTypeCurrent)
        {
            return ConvertFilterExpression<ISoftDeleteEntity>(e => dataContext.CurrentUserFullAclAccess || !e.IsDeleted,
                entityTypeCurrent);
        }

        #endregion

    }

}