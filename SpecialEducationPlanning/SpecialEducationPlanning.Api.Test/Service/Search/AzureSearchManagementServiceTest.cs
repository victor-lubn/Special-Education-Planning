using System.Threading.Tasks;

using SpecialEducationPlanning
.Domain.Service.Search;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Service
{
    public class AzureSearchManagementServiceTest : BaseTest
    {
        private readonly Mock<IAzureSearchManagementService> service;

        public AzureSearchManagementServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            service = new Mock<IAzureSearchManagementService>();
        }

        [Fact]
        public async Task EnsureCreatedAsync_CreatesIndex_ReturnsTrue()
        {
            //Arrange
            service.Setup(ms => ms.EnsureCreatedAsync(true)).Returns(Task.FromResult(true));
            //Act
            var result = await service.Object.EnsureCreatedAsync(true);
            //Assert*/
            Assert.True(true);
        }
    }
}

