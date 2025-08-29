using Microsoft.Azure.Search;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{
    public class OmniSearchVersionIndexModel : SearchBaseIndexModel
    {

        #region Properties 

        [IsFilterable] [IsSearchable] public string ExternalCode { get; set; }

        [IsFilterable] [IsSearchable] public string VersionNotes { get; set; }

        #endregion

    }
}
