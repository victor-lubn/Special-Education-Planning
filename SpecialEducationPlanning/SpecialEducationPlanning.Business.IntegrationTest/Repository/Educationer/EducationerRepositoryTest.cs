using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Educationer.Educationer
{
    public class EducationerRepositoryTest : BaseTest
    {
        public EducationerRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IEducationerRepository>();
        }

        private readonly IEducationerRepository repository;

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }
    }
}
