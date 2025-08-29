using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Report
{
    public class BuilderReportModel
    {
        public string AccountNumber { get; set; }
        public string Address0 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Email { get; set; }
        public string LandLineNumber { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Postcode { get; set; }
        public string TradingName { get; set; }
        public int Id { get; set; }
        public string SAPAccountStatus { get; set; }
        public BuilderStatus BuilderStatus { get; set; }
    }
}