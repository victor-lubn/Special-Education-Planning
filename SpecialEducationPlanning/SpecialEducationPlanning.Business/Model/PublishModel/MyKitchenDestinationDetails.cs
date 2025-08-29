using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpecialEducationPlanning
.Business.Model.PublishModel
{
    public class MyKitchenDestinationDetails
    {
        [Required]
        public string FromEmail { get; set; }

        [Required]
        public string ToEmail { get; set; }

        public List<string> CcEmails { get; set; }

        [Required]
        public string AiepEmail { get; set; }

        [Required]
        public string ProductDataStoragePath { get; set; }

        public string Comments { get; set; }
    }
}

