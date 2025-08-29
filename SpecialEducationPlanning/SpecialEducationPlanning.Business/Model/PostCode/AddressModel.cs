using Koa.Domain;
using Newtonsoft.Json;

namespace SpecialEducationPlanning
.Business.Model.PostCode
{
    public class AddressModel
    {
        [JsonProperty(PropertyName = "addressLine1", Order = 1)]
        public string AddressLine1 { get; set; }
        [JsonProperty(PropertyName = "addressLine2", Order = 2)]
        public string AddressLine2 { get; set; }
        [JsonProperty(PropertyName = "addressLine3", Order = 3)]
        public string AddressLine3 { get; set; }
        [JsonProperty(PropertyName = "locality", Order = 4)]
        public string Locality { get; set; }
        [JsonProperty(PropertyName = "province", Order = 5)]
        public string Province { get; set; }
        [JsonProperty(PropertyName = "postalCode", Order = 6)]
        public string PostalCode { get; set; }
        [JsonProperty(PropertyName = "country", Order = 7)]
        public string Country { get; set; }
    }
}
