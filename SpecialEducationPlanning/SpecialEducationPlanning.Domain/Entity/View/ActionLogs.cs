using Koa.Domain;
using System;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Entity.View
{
    public class ActionLogs : BaseEntity<int>
    {
        public ActionType ActionType { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string User { get; set; }

        public string EntityValue { get; set; }

        public DateTime Date { get; set; }
    }
}
