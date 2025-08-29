using Koa.Domain.Specification;
using System;

namespace SpecialEducationPlanning
.Domain.Specification.LogSpecifications
{
    public class ActionLogsToDeleteAutomaticallyByDateSpecification : Specification<Domain.Entity.Action>
    {
        public ActionLogsToDeleteAutomaticallyByDateSpecification(DateTime dateTime, double date) :
            base(x =>
                    (dateTime - x.Date).TotalDays > date
                )
        {
        }
    }
}