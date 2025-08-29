using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Area
{
    public class AreaRepositoryTest : BaseTest
    {
        private readonly IAreaRepository repository;

        public AreaRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IAreaRepository>();
        }       

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task GetAreaAiepTestAsync()
        {
            var area = new Domain.Entity.Area();
            var Aiep = new Domain.Entity.Aiep();
            area.Aieps.Add(Aiep);
            area = await this.repository.Add(area);

            var result = await repository.GetAreaAieps(area.Id);
            Assert.Equal(1, result.Content.Count);
        }
    }
}
