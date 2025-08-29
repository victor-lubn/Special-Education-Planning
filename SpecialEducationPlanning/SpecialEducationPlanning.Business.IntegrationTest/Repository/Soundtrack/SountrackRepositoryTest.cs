using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Soundtrack
{
    public class SoundtrackRepositoryTest : BaseTest
    {
        private readonly ISoundtrackRepository repository;

        public SoundtrackRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<ISoundtrackRepository>();
        }

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }
    }
}