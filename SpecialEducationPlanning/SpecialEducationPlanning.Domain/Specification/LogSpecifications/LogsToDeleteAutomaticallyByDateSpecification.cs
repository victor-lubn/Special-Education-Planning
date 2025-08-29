using Koa.Domain.Specification;
using System;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.LogSpecifications
{
    public class LogsToDeleteAutomaticallyByDateSpecification : Specification<Log>
    {
        public LogsToDeleteAutomaticallyByDateSpecification(DateTime dateTime, double date) :
            base(x =>
                    (dateTime - x.TimeStamp).TotalDays > date
                )
        {
        }
    }
}