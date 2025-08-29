using com.sun.tools.javac.resources;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    /// <summary>
    /// Publish Model for receiving a Plan ID
    /// </summary>
    public class PublishVersionModel
    {
        public int VersionId { get; set; }

        //[Required]
        [EmailAddress]
        public string EducationerEmail { get; set; }

        [EmailAddress]
        public string ReceipientEmail1 { get; set; }

        [EmailAddress]
        public string ReceipientEmail2 { get; set; }

        public string Comments { get; set; }

        public string SelectedMusic { get; set; }

        public bool RequiredVideo { get; set; }

        public bool RequiredVirtualShowRoom { get; set; }

        public bool RequiredHd { get; set; }

        public string UserEmail { get; set; }

        public string Country { get; set; }
        public string VersionCode { get; set; }
        public string CrmProjectCode { get; set; }
        public string CrmHousingType { get; set; }
        public string CrmHousingSpecificationCode { get; set; }
        public DestinationEnum Destination { get; set; }
        public bool IsCHTPrequest { get; set; }
        public Version Version { get;set; }
        public string PlanCode { get; set; }

        public bool IsCycles { get; set; } = false;
    }
}

