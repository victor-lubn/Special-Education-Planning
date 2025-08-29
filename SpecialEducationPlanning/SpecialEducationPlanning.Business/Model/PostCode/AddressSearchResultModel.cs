using Newtonsoft.Json;

namespace SpecialEducationPlanning
.Business.Model.PostCode
{
    public class AddressSearchResultModel
    {
        [JsonProperty(PropertyName = "suggestion", Order = 1)]
        public string Suggestion { get; set; }
        [JsonProperty(PropertyName = "format", Order = 2)]
        public string Format { get; set; }
    }
}
