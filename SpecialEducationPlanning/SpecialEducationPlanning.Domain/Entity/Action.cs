using Koa.Domain;
using System;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Action : BaseEntity<int>
    {
        public ActionType ActionType { get; set; }

        public string AdditionalInfo { get; set; }

        public DateTime Date { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string User { get; set; }



    }
}
