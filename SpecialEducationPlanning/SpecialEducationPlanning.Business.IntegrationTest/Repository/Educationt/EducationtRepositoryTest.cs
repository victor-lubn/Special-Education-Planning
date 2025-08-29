using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Specification.AiepSpecifications;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Aiep
{
    public class AiepRepositoryTest : BaseTest
    {
        private readonly IAiepRepository repository;

        public AiepRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IAiepRepository>();
        }

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }

        public async Task GetAiepProjectsTestAsync()
        {
            var Aiep = new Domain.Entity.Aiep();
            Aiep.Projects.Add(new Domain.Entity.Project());
            Aiep = await repository.ApplyChangesAsync(Aiep);

            var projects = await repository.GetAiepProjectsAsync(Aiep.Id);
            Assert.Equal(1, projects.Content.Count);
        }
    }
}
