using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpecialEducationPlanning
.Business.Repository
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> IncludeNavigations<T, TModel>(this IQueryable<T> queryable,
            IDbContextAccessor contextAccessor)
            where T : class
        {
            var context = contextAccessor.GetCurrentContext();
            var modelProperties = typeof(TModel).GetProperties();
            foreach (var property in context.Model.FindEntityType(typeof(T)).GetNavigations()
                .Where(nav => modelProperties.Any(modelProp => modelProp.Name == nav.Name)))
            {
                queryable = queryable.Include(property.Name);
            }
            return queryable;
        }

        public static IQueryable<T> IncludeNavigations<T, TModel>(this IQueryable<T> queryable,
            IDbContextAccessor contextAccessor, IEnumerable<string> navigations)
            where T : class
        {
            var context = contextAccessor.GetCurrentContext();
            var modelProperties = typeof(TModel).GetProperties();
            foreach (var property in context.Model.FindEntityType(typeof(T)).GetNavigations()
                .Where(nav =>
                    modelProperties.Any(modelProp => modelProp.Name == nav.Name) &&
                    navigations.Any(n => n == nav.Name)))
            {
                queryable = queryable.Include(property.Name);
            }
            return queryable;
        }

        public static IOrderedQueryable<T> OrderByExtension<T>(this IQueryable<T> queryable, string propertyName) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { queryable, lambda });

            return (IOrderedQueryable<T>)result;
        }

        public static IOrderedQueryable<T> OrderByDescendingExtension<T>(this IQueryable<T> queryable, string propertyName) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { queryable, lambda });

            return (IOrderedQueryable<T>)result;
        }
    }
}