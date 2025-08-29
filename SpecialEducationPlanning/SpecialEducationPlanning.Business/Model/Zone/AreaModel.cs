using Koa.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class AreaModel : BaseModel<int>
    {
        public int RegionId { get; set; }
        public virtual RegionModel Region { get; set; }
        public virtual ICollection<AiepModel> Aieps { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }
        public int AiepCount { get; set; }
    }
}
