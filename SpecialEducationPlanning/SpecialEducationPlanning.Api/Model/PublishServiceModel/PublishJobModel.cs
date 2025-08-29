using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    public class PublishJobModel
    {
        public Guid JobId { get; set; }

        public int State { get; set; }
        public string StateName { get; set; }
        public int ParentState { get; set; }
        public string ParentStateName { get; set; }

        public string VersionCode { get; set; }

        public string? AiepNo { get; set; }

        public string? BuilderId { get; set; }

        public string? QuoteNMBR { get; set; }

        public string? Name { get; set; }

        public char HdVersion { get; set; }

        public int RenderingType { get; set; }
        public string RenderingTypeName { get; set; }

        public string? SenderEmail { get; set; }

        public string? RecipientEmail1 { get; set; }

        public string? RecipientEmail2 { get; set; }

        public string? Comments { get; set; }

        public string? AiepEmail { get; set; }

        public string? UserEmail { get; set; }

        public string? FileStorageLocation { get; set; }
        public string? Country { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public IEnumerable<PublishJobActivityModel> PublishJobActivities { get; set; }
        public IEnumerable<AssetModel> Assets { get; set; }

    }
}

