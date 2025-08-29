using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Region : BaseEntity<int>
    {
        public int CountryId { get; set; }
        public virtual Country Country { set; get; }
        public virtual ICollection<Area> Areas { get; set; } = new Collection<Area>();
        public string KeyName { get; set; }
    }
}