using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Domain.Entity
{

    public class User : BaseEntity<int>, IAuditableEntity, ISearchable<int>
    {

        #region Properties 

        public virtual Aiep CurrentAiep { get; set; }

        public int? CurrentAiepId { get; set; }

        public Aiep Aiep { get; set; }

        public int? AiepId { get; set; }

        public string FirstName { get; set; }

        public bool FullAclAccess { get; set; }

        public virtual ReleaseInfo ReleaseInfo { get; set; }

        public int? ReleaseInfoId { get; set; }

        public string Surname { get; set; }

        public string UniqueIdentifier { get; set; }

        public bool Leaver { get; set; }

        public int? DelegateToUserId { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
        public virtual IEnumerable<UserReleaseInfo> UserReleasesInfo { get; set; }

        #endregion

        #region Properties IAuditableEntity

        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }

        #endregion

    }

}
