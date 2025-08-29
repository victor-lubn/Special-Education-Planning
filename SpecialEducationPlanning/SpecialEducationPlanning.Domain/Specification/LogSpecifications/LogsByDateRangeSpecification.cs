using Koa.Domain.Specification;
using System;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.LogSpecifications
{
    public class LogsByDateRangeSpecification : Specification<Log>
    {
        public LogsByDateRangeSpecification(DateTime initDate, DateTime endDate) :
            base(x => x.TimeStamp.Date >= initDate.Date && x.TimeStamp.Date <= endDate.Date)
        {
        }
    }
}