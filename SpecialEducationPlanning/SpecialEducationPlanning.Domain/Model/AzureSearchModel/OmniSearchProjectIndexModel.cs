using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{
    public class OmniSearchProjectIndexModel : SearchBaseIndexModel
    {
        #region Properties
        [IsFilterable] [IsSearchable] [IsSortable] public string CodeProject { get; set; }
        [IsFilterable] [IsSearchable] [IsSortable] public string KeyName { get; set; }
        [IsFilterable] public string SinglePlanProject { get; set; }
        [IsSearchable] public string BuilderName {  get; set; }
        [IsFilterable] [IsSortable] public DateTime? CreatedDate { get; set; }
        [IsFilterable] public string IsArchived { get; set; }

        #endregion
    }
}
