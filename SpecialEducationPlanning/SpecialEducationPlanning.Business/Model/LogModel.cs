using Koa.Domain;
using System;


namespace SpecialEducationPlanning
.Business.Model
{

    public class LogModel : BaseModel<int>

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
