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
    public class RegionMaterializedRegionModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Region, RegionModel>
    {
        public RegionMaterializedRegionModelPagedValueQuery(ISpecification<Region> specification, ISpecification<RegionModel> regions,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, regions, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<RegionModel> Materialize(IQueryable<Region> queryable)
        {
            var materialize = queryable.Select(c => new RegionModel
            {
                Id = c.Id,
                CountryId = c.CountryId,
                KeyName = c.KeyName,
                AreasCount = c.Areas.Select(ur => new Area
                {
                    Id = ur.Id
                }).Count()
            });
            return materialize;
        }
    }
}
