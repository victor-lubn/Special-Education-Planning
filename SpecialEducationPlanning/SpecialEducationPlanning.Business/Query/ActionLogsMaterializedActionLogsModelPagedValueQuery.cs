using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.Query;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Model.View;
using SpecialEducationPlanning
.Domain.Entity.View;

namespace SpecialEducationPlanning
.Business.Query
{
    public class ActionLogsMaterializedActionLogsModelPagedValueQuery : MultipleMaterializedPagedValueQuery<ActionLogs, ActionLogsModel>
    {
        public ActionLogsMaterializedActionLogsModelPagedValueQuery(ISpecification<ActionLogs> specification,
            ISpecification<ActionLogsModel> actionLogs, ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize)
            : base(specification, actionLogs, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<ActionLogsModel> Materialize(IQueryable<ActionLogs> queryable)
        {
            var materialize = queryable.Select(c => new ActionLogsModel
            {
                Id = c.Id,
                ActionType = c.ActionType,
                EntityId = c.EntityId,
                EntityName = c.EntityName,
                User = c.User,
                EntityValue = c.EntityValue,
                Date = c.Date
            });
            return materialize;
        }
    }
}
