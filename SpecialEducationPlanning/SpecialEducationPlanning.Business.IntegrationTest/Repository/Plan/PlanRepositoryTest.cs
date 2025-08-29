using Microsoft.Extensions.DependencyInjection;

using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Plan
{
    public class PlanRepositoryTest : BaseTest
    {
        private readonly IPlanRepository repository;

        public PlanRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IPlanRepository>();
        }        

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }
    }
}