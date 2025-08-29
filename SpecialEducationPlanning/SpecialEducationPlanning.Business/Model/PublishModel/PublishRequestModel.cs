using System;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model.PublishModel
{
    public class PublishRequestModel
    {
        [Required]
        public Guid RenderingTransactionId { get; set; }

        [Required]
        public string PlanCode { get; set; }

        [Required]
        public string VersionCode { get; set; }

        [Required]
        public string PlanName { get; set; }

        [Required]
        public string Range { get; set; }

        [Required]
        public string Educationer { get; set; }

        [Required]
        public string AiepCode { get; set; }

        [Required]
        public string InputStoragePath { get; set; }

        [Required]
        public string RequestedByUser { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public object DestinationDetails { get; set; }

        [Required]
        public string PlanQuality { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string CadPlatform { get; set; }

        [Required]
        public DateTime RequestTimestamp { get; set; }

        public bool? AdditionalProperties { get; set; } = false;
    }
}


