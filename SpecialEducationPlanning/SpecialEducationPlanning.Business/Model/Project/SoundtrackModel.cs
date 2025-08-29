using Koa.Domain;
using System;

namespace SpecialEducationPlanning
.Business.Model
{
    public class SoundtrackModel : BaseModel<int>
    {
        public DateTime CreationDate { get; set; }
        public string Display { get; set; }
        public string Code { get; set; }
    }
}