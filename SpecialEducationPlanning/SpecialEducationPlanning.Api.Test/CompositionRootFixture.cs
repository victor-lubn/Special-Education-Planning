using Microsoft.Extensions.Configuration;

namespace SpecialEducationPlanning
.Api.Test
{
    public class CompositionRootFixture
    {
        public IConfigurationRoot Configuration { get; }

        public CompositionRootFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", false)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

        }
    }
}
