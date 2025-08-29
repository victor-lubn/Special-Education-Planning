using System.ComponentModel.DataAnnotations;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    /// <summary>
    /// Publish Model for receiving a Plan ID
    /// </summary>
    public class PublishPlanPostModel
    {
        public int PlanId { get; set; }

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

        public bool CadPublishingSelected { get; set; }
        public int ProjectId { get; set; }
    }
}

