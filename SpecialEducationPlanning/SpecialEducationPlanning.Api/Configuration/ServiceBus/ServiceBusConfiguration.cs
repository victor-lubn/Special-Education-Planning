namespace SpecialEducationPlanning
.Api.Configuration.ServiceBus
{
    public class ServiceBusConfiguration
    {
        public const string SectionName = "ServiceBus";

        public int MaxRetries { get; set; } = 3;

        public string ConnectionString { get; set; }

        public string FullyQualifiedNamespace { get; set; }
    }
}
