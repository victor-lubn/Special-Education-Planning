using Microsoft.Azure.Search;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{
    public class OmniSearchEndUserIndexModel : SearchBaseIndexModel
    {

        #region Properties 

        [IsFilterable] [IsSearchable] public string Address0 { get; set; }

        [IsFilterable] [IsSearchable] public string Address1 { get; set; }

        [IsFilterable] [IsSearchable] public string Comment { get; set; }

        [IsFilterable] [IsSearchable] public string ContactEmail { get; set; }

        [IsFilterable] [IsSearchable] public string FirstName { get; set; }

        [IsFilterable] [IsSearchable] public string FullName { get; set; }

        [IsFilterable] [IsSearchable] public string LandLineNumber { get; set; }

        [IsFilterable] [IsSearchable] public string MobileNumber { get; set; }

        [IsFilterable] [IsSearchable] public string Postcode { get; set; }

        [IsFilterable] [IsSearchable] public string Surname { get; set; }

        #endregion

    }
}
