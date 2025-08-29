using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class AiepModel : BaseModel<int>
    {
        [StringLength(DataContext.ShortPropertyLength)]
        public string AiepCode { get; set; }
        public int? ReleaseInfoId { get; set; }
        public virtual ReleaseInfoModel ReleaseInfo { get; set; }
        public int AreaId { get; set; }
        public virtual AreaModel Area { get; set; }
        public virtual ICollection<ProjectModel> Projects { get; set; } = new Collection<ProjectModel>();
        public ICollection<BuilderEducationerAiepModel> BuilderEducationerAieps { get; set; } = new Collection<BuilderEducationerAiepModel>();

        public ICollection<UserModel> Educationers { get; set; } = new Collection<UserModel>();

        [Required]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Email { get; set; }
        [Required]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address1 { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address2 { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address3 { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address4 { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address5 { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address6 { get; set; }
        [Required]
        [StringLength(DataContext.ShortPropertyLength)]
        public string Postcode { get; set; }
        [StringLength(DataContext.TelephoneNumberPropertyLegth)]
        public string PhoneNumber { get; set; }
        [StringLength(DataContext.TelephoneNumberPropertyLegth)]
        public string FaxNumber { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string IpAddress { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string MediaServer { get; set; }
        public bool HtmlEmail { get; set; }
        public int DownloadSpeed { get; set; }
        public int DownloadLimit { get; set; }
        public int? ManagerId { get; set; }
        public virtual UserModel Manager { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateUser { get; set; }
    }
}

