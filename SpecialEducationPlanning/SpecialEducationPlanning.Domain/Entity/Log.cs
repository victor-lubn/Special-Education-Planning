using Koa.Domain;
using System;


namespace SpecialEducationPlanning
.Domain.Entity
{

    public class Log : BaseEntity<int>
    {
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        public bool ExternalSource { get; set; }
    }
}
