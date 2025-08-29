using System.Net.Http;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Strategy.PostCode;
using SpecialEducationPlanning
.Api.Service.PostCode;
using SpecialEducationPlanning
.Api.Configuration.PostCode;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Service
{
    public class IAspPostCodeServiceTest : BaseTest
    {
        private readonly IPostCodeService service;

        public IAspPostCodeServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var strategy = new GbrPostCodeStrategy(new Mock<ILogger<GbrPostCodeStrategy>>().Object);
            var httpClient = new Mock<HttpClient>();
            var postCodeConfig = new PostCodeConfiguration();
            var logger = this.LoggerFactory.CreateLogger<PostCodeService>();
            service = new PostCodeService(postCodeConfig, logger, strategy, httpClient.Object);
        }

        [Fact]
        public void TestConstructor()
        {
            Assert.NotNull(service);
        }
    }
}
