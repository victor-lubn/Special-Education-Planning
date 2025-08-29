using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model
{
    public class PlanModel : IModel<int>
    {
        public int Id { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string Title { get; set; }
        public DateTime LastOpen { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public virtual ProjectModel Project { get; set; }
        public virtual ICollection<VersionModel> Versions { get; set; } = new Collection<VersionModel>();
        public string KeyName { get; set; }
        [Required]
        public bool Survey { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string PlanCode { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string PlanName { get; set; }
        [Required]
        public int CatalogId { get; set; }
        public int? EndUserId { get; set; }
        public virtual EndUserModel EndUser { get; set; }
        public int? BuilderId { get; set; }
        public virtual BuilderModel Builder { get; set; }
        public int? EducationerId { get; set; }
        public virtual UserModel Educationer { get; set; }
        public PlanState PlanState { get; set; }
        public int? MasterVersionId { get; set; }
        public virtual VersionModel MasterVersion { get; set; }
        public bool IsStarred { get; set; }
        public bool? IsTemplate { get; set; }
        [StringLength(DataContext.LongPropertyLength)]
        public string BuilderTradingName { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string UpdateUser { get; set; }
        public string PlanType { get; set; }
        public string CadFilePlanId { get; set; }
        public DateTime? OfflineSyncDate { get; set; }

        public int? HousingTypeId { get; set; }
        public HousingTypeModel HousingTypeModel { get; set; }

        public int? HousingSpecificationTemplatesId { get; set; }
        public HousingSpecificationTemplatesModel HousingSpecificationTemplatesModel { get; set; }

        public int? ProjectTemplatesId { get; set; }
        public ProjectTemplatesModel ProjectTemplatesModel { get; set; }

        public int? HousingSpecificationId { get; set; }
        public string PlanReference { get; set; }

        public string EducationOrigin { get; set; }

        public string EducationTool3DCPlanId { get; set; }
    }
    public class PlanModel2 : BaseModel<int>
    {
        public virtual ICollection<VersionModel> Versions { get; set; } = new Collection<VersionModel>();
        public int CatalogId { get; set; }
        public int? MasterVersionId { get; set; }
    }
}

