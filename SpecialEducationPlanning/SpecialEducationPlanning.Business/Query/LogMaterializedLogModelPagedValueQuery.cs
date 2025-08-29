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
    public class LogMaterializedLogModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Log, LogModel>
    {
        public LogMaterializedLogModelPagedValueQuery(ISpecification<Log> specification, ISpecification<LogModel> logs,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, logs, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<LogModel> Materialize(IQueryable<Log> queryable)
        {
            var materialize = queryable.Select(c => new LogModel
            {
                Id = c.Id,
                Message = c.Message,
                MessageTemplate = c.MessageTemplate,
                Level = c.Level,
                TimeStamp = c.TimeStamp,
                Exception = c.Exception,
                Properties = c.Properties
            });
            return materialize;
        }
    }
}
