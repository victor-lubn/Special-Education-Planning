using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    public class AreaDtoModel : BaseModel<int>
    {
        public int RegionId { get; set; }
        public virtual ICollection<int> AiepIds { get; set; } = new Collection<int>();
        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }
    }
}
