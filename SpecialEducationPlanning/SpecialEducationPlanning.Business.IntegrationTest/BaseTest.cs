using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.IntegrationTest.Support;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test
{
    public class BaseTest : IClassFixture<CompositionRootFixture>
    {
        protected readonly CompositionRootFixture Fixture;

        protected readonly ILoggerFactory LoggerFactory;

        public BaseTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper)
        {
            Fixture = fixture;
            Fixture.Init();
            this.LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            this.LoggerFactory.AddProvider(new TestLoggerProvider(outputHelper));
        }
    }
}