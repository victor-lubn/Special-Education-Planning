using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Area : BaseEntity<int>
    {
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Aiep> Aieps { get; set; } = new Collection<Aiep>();
        public string KeyName { get; set; }
    }
}
