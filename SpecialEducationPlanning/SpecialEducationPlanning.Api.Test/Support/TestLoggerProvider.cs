using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Support
{
    internal class TestLoggerProvider : ILoggerProvider
    {
        private readonly TestLogger logger;

        public TestLoggerProvider(ITestOutputHelper outputHelper)
        {
            this.logger = new TestLogger(outputHelper);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this.logger;
        }

        public void Dispose()
        {

        }
    }
}