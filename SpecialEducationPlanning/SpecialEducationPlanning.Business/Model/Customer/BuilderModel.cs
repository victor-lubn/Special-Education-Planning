using Koa.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;

namespace SpecialEducationPlanning
.Business.Model
{

    public class BuilderModel : IModel<int>
    {
        private string accountNumber;

        public int Id { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value.NormaliseAccountNumber(); }
        }

        [StringLength(DataContext.LongPropertyLength)]
        public string Address0 { get; set; }

        [Required]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address1 { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address2 { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string Address3 { get; set; }

        public virtual ICollection<BuilderEducationerAiepModel> BuilderEducationerAieps { get; set; } =
            new Collection<BuilderEducationerAiepModel>();

        [StringLength(DataContext.DefaultPropertyLength)]
        public string Email { get; set; }

        [StringLength(DataContext.TelephoneNumberPropertyLegth)]
        public string LandLineNumber { get; set; }

        [StringLength(DataContext.TelephoneNumberPropertyLegth)]
        public string MobileNumber { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string Name { get; set; }

        [StringLength(DataContext.LongPropertyLength)]
        public string Notes { get; set; }

        public virtual ICollection<PlanModel> Plans { get; set; } = new Collection<PlanModel>();

        [Required]
        [StringLength(DataContext.ShortPropertyLength)]
        public string Postcode { get; set; }

        [Required]
        [StringLength(DataContext.DefaultPropertyLength)]
        public string TradingName { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string UpdateUser { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string OwningAiepCode { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string OwningAiepName { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string SAPAccountStatus { get; set; }

        public BuilderStatus BuilderStatus { get; set; }

    }

}

