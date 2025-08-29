using System;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    public class AssetModel
    {
        public Guid JobId { get; set; }
        public AssetType Type { get; set; }
        public string Group { get; set; }
        public string? Uri { get; set; }
        public string? AccessToken { get; set; }

        public string? Path { get; set; }

        public DateTime CreationDate { get; set; }
        public PublishJobModel PublishJob { get; set; }
    }
}
