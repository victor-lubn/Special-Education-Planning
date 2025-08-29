using Koa.Domain;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    public class BuilderSapModel : BaseModel<int>
    {
        public string Name { get; set; }
        public string TradingName { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string LandLineNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Notes { get; set; }
        public string AiepID { get; set; }
        public string AiepName { get; set; }
        public BuilderStatus BuilderStatus { get; set; }
        public string SAPK8Status { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public string DiallingCode { get; set; }
    }
}

