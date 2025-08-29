namespace SpecialEducationPlanning
.Api.Configuration.PublishSystemService
{
    public class PublishServiceConfiguration
    {
        /// <summary>
        /// </summary>
        public const string Section = "PublishServiceConfig:PublishService";
        /// <summary>
        /// </summary>
        public string PublishJobUrl { get; set; }

        /// <summary>
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// </summary>
        public string Token { get; set; }

        public string GetPublishJobsByVersionCodesUrl { get; set; }

        public string GetPublishJobsByJobIdUrl { get; set; }

        public string HealthCheckUrl { get; set; }

    }
}
