using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{

    public class OmniSearchBuilderIndexModel : SearchBaseIndexModel
    {

        #region Properties 

        [IsFilterable] [IsSearchable] [IsSortable] public string AccountNumber { get; set; }

        [IsFilterable] [IsSearchable] public string Address0 { get; set; }

        [IsFilterable] [IsSearchable] [IsSortable] public string Address1 { get; set; }

        [IsFilterable] [IsSearchable] public string Email { get; set; }

        [IsFilterable] [IsSearchable] public string LandLineNumber { get; set; }

        [IsFilterable] [IsSearchable] [IsSortable] public string MobileNumber { get; set; }

        [IsFilterable] [IsSearchable] public string Name { get; set; }

        [IsFilterable] [IsSearchable] [IsSortable] public string Postcode { get; set; }

        [IsFilterable] [IsSearchable] [IsSortable] public string TradingName { get; set; }

        [IsFilterable] [IsSearchable] public string Notes { get; set; }

        //[IsFilterable] [IsSearchable] public string SAPAccountStatus { get; set; }


        [IsFilterable] [IsSortable] [IsSearchable] 
        public string BuilderStatus{ get; set;}

        [JsonIgnore]
        public int Status
        {
            get
            {
                int status = 0;
                if (BuilderStatus != null)
                {
                    status = (int)System.Enum.Parse(typeof(Enum.BuilderStatus), BuilderStatus);
                }
                return status;
            }
            set { BuilderStatus = System.Enum.GetName(typeof(Enum.BuilderStatus), value); }
        }


        #endregion

    }

}