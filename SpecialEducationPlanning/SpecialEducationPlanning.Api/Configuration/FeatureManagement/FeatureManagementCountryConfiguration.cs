using System.ComponentModel.DataAnnotations;

namespace SpecialEducationPlanning
.Api.Configuration.FeatureManagement
{
    public class FeatureManagementCountryConfiguration 
    {
        public FeatureManagementCountryConfiguration(string country)
        {
            Country = country;
        }

        [Required]
        public string Country { get; set; }
    }
}
