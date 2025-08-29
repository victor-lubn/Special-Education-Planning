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
    public class AreaMaterializedAreaModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Area, AreaModel>
    {
        public AreaMaterializedAreaModelPagedValueQuery(ISpecification<Area> specification, ISpecification<AreaModel> areaModel,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, areaModel, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<AreaModel> Materialize(IQueryable<Area> queryable)
        {
            var materialize = queryable.Select(c => new AreaModel
            {
                Id = c.Id,
                RegionId = c.RegionId,
                KeyName = c.KeyName,
                AiepCount = c.Aieps.Count
            });
            return materialize;
        }
    }
}

