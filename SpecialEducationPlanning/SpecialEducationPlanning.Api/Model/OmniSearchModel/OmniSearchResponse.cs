using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Model.OmniSearchModel
{
    public class OmniSearchResponse
    {
        public IEnumerable<OmniSearchModel> OmniSearchItemsList { get; set; }
        public int totalCount { get; set; }
        public bool MaxExceeded { get; set; }
    }
}