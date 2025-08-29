using Koa.Domain;
using System;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model.View
{
    public class ActionLogsModel : BaseModel<int>
    {
        public ActionType ActionType { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string User { get; set; }

        public string EntityValue { get; set; }

        public DateTime Date { get; set; }
    }
}
