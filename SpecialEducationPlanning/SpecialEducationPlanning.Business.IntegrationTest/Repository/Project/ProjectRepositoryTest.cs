using Microsoft.Extensions.DependencyInjection;

using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Project
{
    public class ProjectRepositoryTest : BaseTest
    {
        public ProjectRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IProjectRepository>();
        }

        private readonly IProjectRepository repository;

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }
    }
}