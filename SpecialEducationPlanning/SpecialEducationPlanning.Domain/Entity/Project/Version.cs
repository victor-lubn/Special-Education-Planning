using Koa.Domain;
using System;
using System.Collections.Generic;


namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Version : BaseEntity<int>, IEntityWithAcl, IAuditableEntity, ISearchable<int>
    {
        #region Properties
        public string KeyName { get; set; }
        public int PlanId { get; set; }
        public virtual Plan Plan { get; set; }
        public string Preview { get; set; }
        public string Rom { get; set; }
        public string PreviewPath { get; set; }
        public string RomPath { get; set; }
        public int VersionNumber { get; set; }
        public string VersionNotes { get; set; }
        public string Range { get; set; }
        public int CatalogId { get; set; }
        public virtual Catalog Catalog { get; set; }
        public virtual ICollection<RomItem> RomItems { get; set; } = new List<RomItem>();
        public string ExternalCode { get; set; }
        public string AiepCode { get; set; }
        public string QuoteOrderNumber { get; set; }

        public int? EducationTool3DCVersionId { get; set; }

        public string EducationTool3DCPlanId { get; set; }

        public int? LastKnown3DCVersion { get; set; }
        
        public int? LastKnownCatalogId { get; set; }

        public string LastKnownPreviewPath { get; set; }

        public string LastKnownRomPath { get; set; }

        public int? FittersPackStatusId { get; set; }

        public virtual FittersPackStatus FittersPackStatus { get; set; }

        public string FittersPackPath { get; set; }

        public string FittersPack3DCJobId { get; set; }

        public DateTime? FittersPack3DCRequestTime { get; set; }

        public DateTime? FittersPack3DCEstimatedTime { get; set; }

        #endregion

        #region Properties IAuditableEntity

        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }

        #endregion
    }
}

