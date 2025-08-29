using Newtonsoft.Json;
using System.Collections.Generic;
using SpecialEducationPlanning
.Business.Model.PostCode;

namespace SpecialEducationPlanning
.Api.Model.PostCodeServiceModel
{
    public class AddressResponseModel
    {
        [JsonProperty(PropertyName = "address", Order = 1)]
        public List<AddressModel> Address { get; set; }
    }
}
