using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Region
{
    public class RegionRepositoryTest : BaseTest
    {
        public RegionRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IRegionRepository>();
        }

        private readonly IRegionRepository repository;

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetRegionAreasTestAsync() {
            var region = new Domain.Entity.Region();
            var area = new Domain.Entity.Area();
            region.Areas.Add(area);
            region = await this.repository.Add(region);

            var areas = await repository.GetRegionAreas(region.Id);
            Assert.Equal(1, areas.Content.Count);
        }
    }
}