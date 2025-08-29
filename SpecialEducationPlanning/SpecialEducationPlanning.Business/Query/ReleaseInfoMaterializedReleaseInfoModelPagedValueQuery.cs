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
    public class ReleaseInfoMaterializedReleaseInfoModelPagedValueQuery : MultipleMaterializedPagedValueQuery<ReleaseInfo, ReleaseInfoModel>
    {
        public ReleaseInfoMaterializedReleaseInfoModelPagedValueQuery(ISpecification<ReleaseInfo> specification, ISpecification<ReleaseInfoModel> releaseInfos,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, releaseInfos, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<ReleaseInfoModel> Materialize(IQueryable<ReleaseInfo> queryable)
        {
            var materialize = queryable.Select(c => new ReleaseInfoModel
            {
                Id = c.Id,
                Title = c.Title,
                Subtitle = c.Subtitle,
                Version = c.Version,
                FusionVersion = c.FusionVersion,
                DateTime = c.DateTime,
                Document = c.Document,
                DocumentPath = c.DocumentPath
            });
            return materialize;
        }
    }
}
