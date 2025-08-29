using System;
using System.Collections.Generic;
using System.Text;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    public class PublishJobActivityModel
    {
        public Guid JobId { get; set; }
        public int Trigger { get; set; }
        public string? TriggerName { get; set; }
        public int Source { get; set; }
        public string? SourceName { get; set; }
        public int Destination { get; set; }
        public string? DestinationName { get; set; }

        public string? Message { get; set; }

        public DateTime TimeStamp { get; set; }

        public int PublishJobId { get; set; }
    }
}
