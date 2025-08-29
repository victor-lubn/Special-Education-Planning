using SpecialEducationPlanning
.Api.Configuration.Base;

namespace SpecialEducationPlanning
.Api.Configuration.PublishProjectService
{
    public class PublishProjectServiceConfiguration : ApimConfigurationBase
    {
        public static string Section = "PublishProjectServiceConfig:PublishService";

        public string PublishJobUrl { get; set; }
    }
}
