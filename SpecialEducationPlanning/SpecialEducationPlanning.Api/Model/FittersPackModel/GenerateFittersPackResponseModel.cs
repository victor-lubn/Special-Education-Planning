using System;

namespace SpecialEducationPlanning
.Api.Model.FittersPackModel
{
    public class GenerateFittersPackResponseModel
    {
        public string JobId { get; set; }

        public string Status { get; set; }

        public DateTime? EstimatedCompletionTime { get; set; }
    }
}
