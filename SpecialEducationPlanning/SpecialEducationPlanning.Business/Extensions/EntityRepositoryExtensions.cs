using Koa.Domain;
using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public static class EntityRepositoryExtensions
    {
        /// <summary>
        /// Gets all the ids from a collection of entities without any filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityRepository"></param>
        /// <param name="take">Number of elements to get</param>
        /// <param name="skip">Number of elements to skip</param>
        /// <returns></returns>
        /// 
        private static IServiceProvider serviceProvider;


        public static IQueryable<T> GetEntitiesNoFiltersAsync<T>(this IEntityRepository<int> entityRepository,
            int take, int skip, DateTime? updateDate, int? indexerWindowInDays) where T : BaseEntity<int>, IAuditableEntity
        {

            IQueryable<T> query;

            if (updateDate.HasValue)
            {
                query = entityRepository.Where(new Specification<T>(x => x.UpdatedDate >= updateDate.Value));
            }
            else if(indexerWindowInDays.HasValue && indexerWindowInDays.Value > 0)
            {
                var dateToFilter = DateTime.Now.AddDays(-indexerWindowInDays.Value);
                query = entityRepository.Where(new Specification<T>(x => x.UpdatedDate >= dateToFilter));
            }
            else
            {
                query = entityRepository.GetAll<T>();
            }

            IQueryable<T> entities;

            if (typeof(T) == typeof(Plan))
            {
                var query2 = (IQueryable<Plan>)query;
                var entities2 = query2.Include(p => p.EndUser).Include(p => p.Educationer).IgnoreQueryFilters().Skip(skip).Take(take);
                entities = (IQueryable<T>)entities2;
            }
            else
            {
                entities = query.IgnoreQueryFilters().Skip(skip).Take(take);
            }

            return entities;
        }
    }
}

