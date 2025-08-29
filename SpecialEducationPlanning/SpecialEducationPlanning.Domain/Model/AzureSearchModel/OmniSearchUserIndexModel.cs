using Microsoft.Azure.Search;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{
    public class OmniSearchUserIndexModel : SearchBaseIndexModel
    {

        #region Properties 

        [IsFilterable] [IsSearchable] public string FirstName { get; set; }

        [IsFilterable] [IsSearchable] public string Surname { get; set; }

        [IsFilterable] [IsSearchable] public string UniqueIdentifier { get; set; }

        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable] [IsSearchable] public override string Id { get; set; }
        #endregion

    }
}
