using System.ComponentModel.DataAnnotations;

namespace SpecialEducationPlanning
.Business.Model.PlanDetails
{
    public class PlanDetailsRequestModel
    {
        [Required]
        public string EducationViewUserId { get; set; }

        [Required]
        public string EducationViewPlanUniqueId { get; set; }
    }
}

