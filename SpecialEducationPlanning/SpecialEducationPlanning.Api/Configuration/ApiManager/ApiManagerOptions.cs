namespace SpecialEducationPlanning
.Api.Configuration.ApiManager
{
    public class ApiManagerOptions
    {
        public const string Section = "ApiManager";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
        public string Scope { get; set; }
    }
}
