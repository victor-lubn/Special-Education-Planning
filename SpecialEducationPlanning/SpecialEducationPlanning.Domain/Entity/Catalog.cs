using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Catalog : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Range { get; set; }
        public bool Enabled { get; set; }

        public int? EducationToolOriginId { get; set; }

        public EducationToolOrigin EducationToolOrigin { get; set; }

        public virtual ICollection<Version> Versions { get; set; } = new Collection<Version>();
        public virtual ICollection<RomItem> RomItems { get; set; } = new Collection<RomItem>();
    }
}

