using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Plan : BaseEntity<int>, IEntityWithAcl, IAuditableEntity, ISearchable<int>
    {
        #region Properties
        public string Title { get; set; }
        public DateTime LastOpen { get; set; }
        public int CatalogId { get; set; }
        public virtual Catalog Catalog { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Version> Versions { get; set; } = new Collection<Version>();
        public string KeyName { get; set; }
        public bool Survey { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public int? EndUserId { get; set; }
        public virtual EndUser EndUser { get; set; }
        public int? BuilderId { get; set; }
        public virtual Builder Builder { get; set; }
        public int? EducationerId { get; set; }
        public virtual User Educationer { get; set; }
        public PlanState PlanState { get; set; }
        public int? MasterVersionId { get; set; }
        public Version MasterVersion { get; set; }
        public bool IsStarred { get; set; }
        public bool? IsTemplate { get; set; }
        public string PlanType { get; set; }
        public string BuilderTradingName { get; set; }
        public string CadFilePlanId { get; set; }
        public DateTime? OfflineSyncDate { get; set; }
        public string PlanReference { get; set; }

        public int? HousingSpecificationId { get; set; }
        public int? HousingTypeId { get; set; }

        public int? EducationToolOriginId { get; set; }

        public EducationToolOrigin EducationToolOrigin { get; set; }

        public HousingType HousingType { get; set; }

        public HousingSpecificationTemplates HousingSpecificationTemplates { get; set; }

        public ProjectTemplates ProjectTemplates { get; set; }
        #endregion

        #region Properties IAuditableEntity

        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }


        #endregion
    }
}
