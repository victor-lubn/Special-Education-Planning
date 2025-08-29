using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class ProjectTemplates : BaseEntity<int>, IEntityWithAcl, ISearchable<int>
    {
        #region Properties
        public int ProjectId { get; set; }
        public int PlanId { get; set; }
        public virtual Project Project { get; set; }
        public virtual Plan Plan { get; set; }
        #endregion
    }
}