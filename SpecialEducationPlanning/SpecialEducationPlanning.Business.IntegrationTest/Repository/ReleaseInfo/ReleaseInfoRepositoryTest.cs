using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Role
{
    public class ReleaseInfoRepositoryTest : BaseTest
    {
        private readonly IReleaseInfoRepository repository;

        public ReleaseInfoRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IReleaseInfoRepository>();
        }

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }
    }
}
