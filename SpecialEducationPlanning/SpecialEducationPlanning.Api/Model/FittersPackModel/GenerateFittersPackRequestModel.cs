namespace SpecialEducationPlanning
.Api.Model.FittersPackModel
{
    public class GenerateFittersPackRequestModel
    {
        public string EducationViewReferenceJobId { get; set; }

        public string CountryCode { get; set; }

        public string CallbackUrl { get; set; }

        public string PlanId { get; set; }

        public int VersionId { get; set; }

        public GenerateFitterPackEducationerDetails EducationerDetails { get; set; }

        public GenerateFitterPackAiepDetails AiepDetails { get; set; }
    }
}


