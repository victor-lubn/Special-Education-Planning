using Koa.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class VersionModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }
        public int PlanId { get; set; }
        public virtual PlanModel Plan { get; set; }
        public int VersionNumber { get; set; }
        [StringLength(DataContext.LongPropertyLength)]
        public string VersionNotes { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Range { get; set; }
        public int CatalogId { get; set; }
        public virtual CatalogModel Catalog { get; set; }
        public virtual ICollection<RomItemModel> RomItems { get; set; } = new List<RomItemModel>();
        [StringLength(DataContext.ShortPropertyLength)]
        public string ExternalCode { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string AiepCode { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }
        public string RomPath { get; set; }
        public string PreviewPath { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string QuoteOrderNumber { get; set; }

        public int? EducationTool3DCVersionId { get; set; }

        public string EducationTool3DCPlanId { get; set; }

        public int? LastKnown3DCVersion { get; set; }

        public int? LastKnownCatalogId { get; set; }

        public string LastKnownPreviewPath { get; set; }

        public string LastKnownRomPath { get; set; }

        public string FittersPackPath { get; set; }
        
        public string FittersPackStatus { get; set; }

        public string FittersPack3DCJobId { get; set; }

        public DateTime? FittersPack3DCRequestTime { get; set; }

        public DateTime? FittersPack3DCEstimatedTime { get; set; }

    }
}

