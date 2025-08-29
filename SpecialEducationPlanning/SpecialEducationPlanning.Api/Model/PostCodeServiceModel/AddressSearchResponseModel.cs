using Newtonsoft.Json;
using SpecialEducationPlanning
.Business.Model.PostCode;

namespace SpecialEducationPlanning
.Api.Model.PostCodeServiceModel
{
    public class AddressSearchResponseModel
    {
        [JsonProperty(PropertyName = "totalMatches", Order = 2)]
        public int TotalMatches { get; set; }
        [JsonProperty(PropertyName = "count", Order = 3)]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "results", Order = 1)]
        public AddressSearchResultModel[] Results { get; set; }
    }
}

