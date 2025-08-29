using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.Query;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Query
{
    public class CountryMaterializedCountryModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Country, CountryModel>
    {
        public CountryMaterializedCountryModelPagedValueQuery(ISpecification<Country> specification, ISpecification<CountryModel> countries,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, countries, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<CountryModel> Materialize(IQueryable<Country> queryable)
        {
            var materialize = queryable.Select(c => new CountryModel
            {
                Id = c.Id,
                KeyName = c.KeyName,
                RegionsCount = c.Regions.Select(ur => new Region
                {
                    Id = ur.Id
                }).Count()
            });
            return materialize;
        }
    }
}
