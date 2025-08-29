using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class HousingSpecification : BaseEntity<int>, IEntityWithAcl, ISearchable<int>
    {
        #region Properties
        
        public string Code { get; set; }
        public string Name { get; set; }
        public int PlanState { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public virtual ICollection<HousingType> HousingTypes { get; set; } = new Collection<HousingType>();
        public virtual ICollection<HousingSpecificationTemplates> HousingSpecificationTemplates { get; set; } = new Collection<HousingSpecificationTemplates>();
        #endregion
    }
}