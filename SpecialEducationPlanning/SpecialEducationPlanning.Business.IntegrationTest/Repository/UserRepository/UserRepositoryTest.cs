using Microsoft.Extensions.DependencyInjection;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Test;

using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.IntegrationTest.Repository.UserRepository
{
    public class UserRepositoryTest : BaseTest
    {
        private readonly IUserRepository Repository;

        public UserRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            Repository = fixture.ServiceProvider.GetRequiredService<IUserRepository>();
        }

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(Repository);
        }
    }
}
