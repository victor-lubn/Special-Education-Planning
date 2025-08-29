using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    public class PublishPlanRequest
    {
        public int Id { get; set; }

        [Required]
        public int PlanId { get; set; }

        public string AiepNo { get; set; }
        public string BuilderId { get; set; }
        public string QuoteNMBR { get; set; }

        [Required]
        public IFormFile PlanFile { get; set; }

        public string Name { get; set; }
        public char HdVersion { get; set; }
        public string SoundTrack { get; set; }
        public int PlanType { get; set; }
        public DateTime PublishDate { get; set; }
        public string PublishTime { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail1 { get; set; }
        public string RecipientEmail2 { get; set; }
        public string Comments { get; set; }
        public string ExternalId { get; set; }
        public string AiepEmail { get; set; }
        public string UserEmail { get; set; }
        [Required]
        public string VersionCode { get; set; }
        public string Country { get; set; }
        public DestinationEnum Destination { get; set; } = DestinationEnum.MY_KITCHEN;
        public string CrmProjectCode { get; set; }
        public string CrmHousingSpecificationCode { get; set; }
        public string CrmHousingType { get; set; }
    }  
}
