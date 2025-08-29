namespace SpecialEducationPlanning
.Api.Configuration.FeatureManagement
{
    public class LaunchDarklyConfiguration
    {
        public string SDK_Key { get; set; }
        public LDHealthCheck HealthCheck { get; set; }
    }

    public class LDHealthCheck
    {
        public string Access_Token { get; set; }
        public string Envionment_Key { get; set; }
        public string Project_Key { get; set; }
        public string Api_BaseUrl { get; set; }
        public string Flag_Route { get; set; }
        public string User_Email { get; set; }
        public string Flag_Name { get; set; }
    }
}
