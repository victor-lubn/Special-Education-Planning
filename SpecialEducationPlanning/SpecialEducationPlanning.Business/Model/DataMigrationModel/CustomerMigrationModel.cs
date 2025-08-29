using System;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class CustomerMigrationModel : MigrationBaseModel
    {
        public string AccountNumber { get; set; }

        public string AiepNumber { get; set; }

        public string AiepTdpId { get; set; }

        public int CustomerId { get; set; }

        public string Surname { get; set; }

        public string FirstName { get; set; }

        //TDP - Title 
        public string TitleInitials { get; set; }

        //TDP - HouseNameOrNumber 
        public string HouseName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public string Address5 { get; set; }

        public string Postcode { get; set; }

        //TDP - MobileNumber 
        public string PhoneWork { get; set; }

        //TDP - LandLineNumber 
        public string PhoneHome { get; set; }

        //TDP - AccountEmail 
        public string Email2 { get; set; }

        public BuilderStatus BuilderStatus { get; set; }

        #region TDP - BuilderNotes
        //TDP - ContactEmail 
        public string Email { get; set; }

        //TDP - Educationer
        public string SalesPerson { get; set; }
        #endregion

        public DateTime? PlanDate { get; set; }

        // Emergency flag
        public bool DisableSapValidation { get; set; }

    }
}


