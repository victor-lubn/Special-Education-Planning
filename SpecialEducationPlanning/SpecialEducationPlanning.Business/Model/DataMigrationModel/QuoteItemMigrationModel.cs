namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class QuoteItemMigrationModel : MigrationBaseModel
    {
        public int QuoteId { get; set; }

        public int QuoteNmbr { get; set; }

        public int SerialNmbr { get; set; }

        public string SKU { get; set; }

        public string Range { get; set; }

        public string Colour { get; set; }

        public int? Quantity { get; set; }

        public int QuoteTdpId { get; set; }
    }
}
