using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class CatalogModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        [CodeInjectionReject]
        public string Name { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [CodeInjectionReject]
        public string Code { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [CodeInjectionReject]
        public string Range { get; set; }
        public bool Enabled { get; set; }

        public string EducationOrigin { get; set; }

        public virtual ICollection<VersionModel> Versions { get; set; } = new Collection<VersionModel>();
        public virtual ICollection<RomItemModel> RomItems { get; set; } = new Collection<RomItemModel>();
    }
}

