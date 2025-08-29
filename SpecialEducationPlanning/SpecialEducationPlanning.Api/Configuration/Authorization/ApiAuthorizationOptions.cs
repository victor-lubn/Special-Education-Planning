namespace SpecialEducationPlanning
.Api.Configuration.Authorization
{
    public class ApiAuthorizationOptions
    {
        public const string Section = "Authorization";
        public bool IsEnabled { get; set; }
        public string BasicToken { get; set; }
        public string BasicTokenKey { get; set; } = "basic-token";
        public bool MigrationAnonymous { get; set; }
    }
}
