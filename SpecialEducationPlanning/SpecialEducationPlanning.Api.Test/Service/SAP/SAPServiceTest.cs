using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Model.SapConfiguration;
using SpecialEducationPlanning
.Api.Service.Sap;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Service
{
    public class SAPServiceTest : BaseTest
    {
        private readonly ISapService service;
        private readonly IOptions<SapConfiguration> options;

        public SAPServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var sapcredentials = new Mock<IOptions<SapConfiguration>>();
            service = new SapService(
                this.LoggerFactory.CreateLogger<SapService>(),
                sapcredentials.Object, new System.Net.Http.HttpClient());
        }

        [Fact]
        public async Task GetSapBuilderByAccountNumberEmtpyStringTest()
        {
            var response = await service.GetSapBuilder(new List<string>());
            Assert.Null(response.Content);
        }

        [Fact]
        public void GetSapBuilderByBuilderMandatoryFieldsEmtpyStringTest()
        {
            var response = service.GetSapBuilderAsync("", "MZR AS1", "Buckingham palace").Result;
            Assert.Null(response.Content);
        }
    }
}
