using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Country : BaseEntity<int>
    {
        public virtual ICollection<Region> Regions { get; set; } = new Collection<Region>();
        public string KeyName { get; set; }
    }
}