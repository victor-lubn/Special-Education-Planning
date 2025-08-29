using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Project : BaseEntity<int>, IEntityWithAcl, IAuditableEntity, ISearchable<int>
    {
        #region Properties
        public string CodeProject { get; set; }
        public int AiepId { get; set; }
        public Aiep Aiep { get; set; }
        public virtual ICollection<Plan> Plans { get; set; } = new Collection<Plan>();
        public string KeyName { get; set; }
        public bool SinglePlanProject { get; set; }
        public int? BuilderId { get; set; }
        public bool? IsArchived { get; set; }
        public virtual Builder Builder { get; set; }
        public virtual ICollection<HousingSpecification> HousingSpecifications { get; set; } = new Collection<HousingSpecification>();
        public virtual ICollection<ProjectTemplates> ProjectTemplates { get; set; } = new Collection<ProjectTemplates>();
        #endregion

        #region Properties IAuditable
        public DateTime CreatedDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateUser { get; set; }
        #endregion
    }
}
