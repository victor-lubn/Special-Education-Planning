using System.ComponentModel.DataAnnotations;

namespace SpecialEducationPlanning
.Business.Model.PublishModel
{
    public class CrmDestinationDetails
    {
        [Required]
        public string CrmProjectCode { get; set; }

        [Required]
        public string CrmHousingSpecificationCode { get; set; }

        [Required]
        public string CrmHousingType { get; set; }
    }
}
