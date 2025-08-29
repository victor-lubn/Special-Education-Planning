using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class HousingSpecificationTemplates : BaseEntity<int>, IEntityWithAcl, ISearchable<int>
    {
        #region Properties
        public int HousingSpecificationId { get; set; }
        public int PlanId { get; set; }
        public virtual HousingSpecification HousingSpecification { get; set; }
        public virtual Plan Plan { get; set; }
        #endregion
    }
}