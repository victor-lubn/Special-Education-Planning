using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Aiep : BaseEntity<int>, IEntityWithAcl, IAuditableEntity
    {
        #region Properties 
        public int? ReleaseInfoId { get; set; }
        public virtual ReleaseInfo ReleaseInfo { get; set; }
        public string AiepCode { get; set; }
        public int AreaId { get; set; }
        public virtual Area Area { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new Collection<Project>();
        public virtual ICollection<BuilderEducationerAiep> BuilderEducationerAieps { get; set; } = new Collection<BuilderEducationerAiep>();
        public virtual ICollection<User> Educationers { get; set; } = new Collection<User>();

        public virtual ICollection<EndUserAiep> EndUserAieps { get; set; } = new Collection<EndUserAiep>();

        public string Name { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string Address6 { get; set; }
        public string Postcode { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string IpAddress { get; set; }
        public string MediaServer { get; set; }
        public bool HtmlEmail { get; set; }
        public int DownloadSpeed { get; set; }
        public int DownloadLimit { get; set; }
        public int? ManagerId { get; set; }
        public virtual User Manager { get; set; }
        #endregion

        #region Properties IAuditableEntity
        public DateTime CreatedDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateUser { get; set; }
        #endregion
    }
}

