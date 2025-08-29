using System;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class QuoteMigrationModel : MigrationBaseModel
    {
        public int CustomerId { get; set; }

        public int CustomerTdpId { get; set; }

        public string Educationer { get; set; }

        public int AiepTdpId { get; set; }

        #region EndUser

        //TDP - Title
        public string ConsumerTitleInitials { get; set; }

        //TDP - FirstName
        public string ConsumerInitials { get; set; }

        public string ConsumerSurname { get; set; }

        //TDP - Address1
        #region TDP - Address1
        public string ConsumerHouseNo { get; set; }

        public string ConsumerHouse { get; set; }

        public string ConsumerAddress1 { get; set; }
        #endregion

        public string ConsumerAddress2 { get; set; }

        public string ConsumerAddress3 { get; set; }

        public string ConsumerAddress4 { get; set; }

        public string ConsumerAddress5 { get; set; }

        public string ConsumerPlot { get; set; }

        public string ConsumerPostCode { get; set; }

        public string ConsumerEmail { get; set; }

        //TDP - MobilePhone
        public string ConsumerTele1 { get; set; }

        //TDP - LandLineNumber
        public string ConsumerTele2 { get; set; }

        public string ConsumerComments { get; set; }

        public string ConsumerPlanType { get; set; }
        #endregion

        public string Catalogue { get; set; }

        public string Range { get; set; }

        //TDP - PlanLastModify
        public DateTime? PlanDate { get; set; }

        public DateTime? PlanCreationDate { get; set; }

        public int QuoteNmbr { get; set; }

        //TDP - PreviewFile
        public string PlanPreview { get; set; }

        public bool? Survey { get; set; }

        //TDP - RomFile
        public string Plan { get; set; }

        public int TdpVersionId { get; set; }

        public string AiepFile { get; set; }
    }
}


