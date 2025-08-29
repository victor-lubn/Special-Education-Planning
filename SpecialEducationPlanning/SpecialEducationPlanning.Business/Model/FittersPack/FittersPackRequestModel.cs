using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model.FittersPack
{
    public class FittersPackRequestModel
    {
        [Required]
        public string EducationTool3dcJobId { get; set; }

        [Required]
        public string EducationTool3dcFittersPackLocation { get; set; }

        [Required]
        public string EducationTool3dcFitterPackStatus { get; set; }
    }
}

