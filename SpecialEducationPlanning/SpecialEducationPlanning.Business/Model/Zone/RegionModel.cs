using Koa.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class RegionModel : BaseModel<int>
    {
        public int RegionManagersId { get; set; }
        public int RegionTrainersId { get; set; }
        public int CountryId { get; set; }
        public virtual CountryModel Country { set; get; }
        public virtual ICollection<AreaModel> Areas { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }

        public int? AreasCount { get; set; }
    }
}