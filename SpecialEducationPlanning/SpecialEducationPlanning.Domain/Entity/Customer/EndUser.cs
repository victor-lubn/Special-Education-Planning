using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Entity
{

    public class EndUser : BaseEntity<int>, IEntityWithAcl, IAuditableEntity, ISearchable<int>
    {

        #region Properties 

        public string Address0 { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public string Address5 { get; set; }

        public string Comment { get; set; }

        public string ContactEmail { get; set; }

        public CountryCode? CountryCode { get; set; }

        public string FirstName { get; set; }

        public string LandLineNumber { get; set; }

        public string MobileNumber { get; set; }

        public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();

        public string Postcode { get; set; }

        public string Surname { get; set; }

        public string FullName { get; set; }

        public int? TitleId { get; set; }
        public Title Title { get; set; }

        public string UniqueIdentifier { get; set; }

        public ICollection<EndUserAiep> EndUserAieps { get; set; } = new Collection<EndUserAiep>();

        #endregion

        #region Properties IAuditableEntity

        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }

        #endregion



    }

}
