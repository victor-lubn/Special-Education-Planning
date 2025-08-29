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
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Query
{
    public class
        BuilderMaterializedBuilderModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Builder, BuilderModel>
    {
        public BuilderMaterializedBuilderModelPagedValueQuery(ISpecification<Builder> specification, ISpecification<BuilderModel> builders,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, builders, sortDescriptor, pageSize)
        {
        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<BuilderModel> Materialize(IQueryable<Builder> queryable)
        {
            var materialize = queryable.Select(c => new BuilderModel
            {
                Id = c.Id,
                Name = c.Name,
                AccountNumber = c.AccountNumber,
                Address0 = c.Address0,
                Address1 = c.Address1,
                Address2 = c.Address2,
                Address3 = c.Address3,
                Email = c.Email,
                LandLineNumber = c.LandLineNumber,
                CreatedDate = c.CreatedDate,
                MobileNumber = c.MobileNumber,
                OwningAiepCode = c.OwningAiepCode,
                OwningAiepName = c.OwningAiepName,
                Postcode = c.Postcode,
                TradingName = c.TradingName,
                SAPAccountStatus = c.SAPAccountStatus,
                BuilderStatus=(BuilderStatus)c.BuilderStatus
            });
            return materialize;
        }
    }
}
