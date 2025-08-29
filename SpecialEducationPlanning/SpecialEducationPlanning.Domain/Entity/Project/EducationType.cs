using Koa.Domain;
using System.Collections;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class HousingType : BaseEntity<int>, IEntityWithAcl, ISearchable<int>
    {
        #region Properties      
        public string Code { get; set; }
        public string Name { get; set; }

        public int HousingSpecificationId { get; set; }
        public virtual HousingSpecification HousingSpecification { get; set; }

        //public int? PlanId { get; set; }
        public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();
        #endregion
    }
}