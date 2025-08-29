namespace SpecialEducationPlanning
.Api.Model.OmniSearchModel
{
    public class OmniSearchRequestModel
    {

        public static int MaxResults = 200;
        public static int MaxResultsEmptySearch = 100;

        public string TextToSearch { get; set; }
        public int PageSize { get; set; } = 30;
        public int PageNumber { get; set; } = 0;
    }
}