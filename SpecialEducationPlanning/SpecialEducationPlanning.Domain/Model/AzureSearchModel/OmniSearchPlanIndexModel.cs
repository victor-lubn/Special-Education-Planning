
using Microsoft.Azure.Search;
using Newtonsoft.Json;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{

    public class OmniSearchPlanIndexModel : SearchBaseIndexModel
    {

        #region Properties
        [IsFilterable] [IsSearchable] public string CadFilePlanId { get; set; }

        [IsFilterable] [IsSearchable] [IsSortable] public string PlanCode { get; set; }

        [IsFilterable] [IsSearchable] [IsSortable] [JsonProperty(NullValueHandling = NullValueHandling.Include)] public string BuilderTradingName { get; set; }

        [IsFilterable] [IsSearchable] public string PlanName { get; set; }

        [IsFilterable] [IsSearchable] public string EducationerId { get; set; }

        [IsSortable] public string EducationerSurname { get; set; }

        [IsFilterable] public string PlanState { get; set; }

        [IsFilterable] [JsonProperty(NullValueHandling = NullValueHandling.Include)] public string BuilderId { get; set; }

        [IsSearchable] public string EndUserAddress { get; set; }

        [IsSearchable] public string EndUserPostcode { get; set; }

        [IsSearchable] [IsSortable] public string EndUserSurname { get; set; }

        [IsSortable] public string MasterVersionId { get; set; }
        #endregion

    }
}
