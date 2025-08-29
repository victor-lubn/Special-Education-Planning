using Koa.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model
{
    public class EndUserModel : BaseModel<int>
    {
        public CountryCode? CountryCode { get; set; }
        public virtual ICollection<PlanModel> Plans { get; set; } = new List<PlanModel>();
        [StringLength(DataContext.LongPropertyLength)]
        public string Comment { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string FirstName { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Surname { get; set; }
        [StringLength(DataContext.LongPropertyLength)]
        public string FullName { get; set; }


        public int? TitleId { get; set; }
        public Domain.Entity.Title Title { get; set; }
        [StringLength(DataContext.ShortPropertyLength)]
        public string Postcode { get; set; }
        [StringLength(DataContext.TelephoneNumberPropertyLegth)]
        public string MobileNumber { get; set; }
        [StringLength(DataContext.TelephoneNumberPropertyLegth)]
        public string LandLineNumber { get; set; }
        [StringLength(DataContext.LongPropertyLength)]
        public string Address0 { get; set; }
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
        public string ContactEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string UpdateUser { get; set; }
        public string UniqueIdentifier { get; set; }

    }
}