using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Entity
{

    public class Builder : BaseEntity<int>, IEntityWithAcl, IAuditableEntity, ISearchable<int>
    {

        #region Properties 

        public string AccountNumber { get; set; }

        public string Address0 { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public virtual ICollection<BuilderEducationerAiep> BuilderEducationerAieps { get; set; } =
            new Collection<BuilderEducationerAiep>();

        public string Email { get; set; }

        public string LandLineNumber { get; set; }

        public string MobileNumber { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public string OwningAiepCode { get; set; }

        public string OwningAiepName { get; set; }

        public virtual ICollection<Plan> Plans { get; set; } = new Collection<Plan>();
        public virtual ICollection<Project> Projects { get; set; } = new Collection<Project>();

        public string Postcode { get; set; }

        public string TradingName { get; set; }

        public string UniqueIdentifier { get; set; }

        public string SAPAccountStatus { get; set; }

        public BuilderStatus BuilderStatus { get; set; }        

        #endregion

        #region Properties IAuditableEntity

        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }

        #endregion


    }

}

