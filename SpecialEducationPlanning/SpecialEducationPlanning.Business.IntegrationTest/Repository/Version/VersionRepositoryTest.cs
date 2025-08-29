using Microsoft.Extensions.DependencyInjection;

using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Education
{
    public class VersionRepositoryTest : BaseTest
    {
        public VersionRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IVersionRepository>();
        }

        private readonly IVersionRepository repository;

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }
    }
}
