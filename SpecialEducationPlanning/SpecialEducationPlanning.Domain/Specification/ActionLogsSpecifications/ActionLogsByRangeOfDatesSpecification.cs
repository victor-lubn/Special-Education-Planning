using Koa.Domain.Specification;
using System;
using SpecialEducationPlanning
.Domain.Entity.View;

namespace SpecialEducationPlanning
.Domain.Specification.AreaSpecifications
{
    public class ActionLogsByRangeOfDatesSpecification : Specification<ActionLogs>
    {
        public ActionLogsByRangeOfDatesSpecification(DateTime startDate, DateTime endDate) : base(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)
        {
        }
    }
}