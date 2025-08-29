using Koa.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class CountryModel : BaseModel<int>
    {
        public virtual ICollection<RegionModel> Regions { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }

        public int? RegionsCount { get; set; }
    }
}