using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.EndUser
{
    public class EndUserRepositoryTest : BaseTest
    {
        public EndUserRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IEndUserRepository>();
        }

        private readonly IEndUserRepository repository;

        private EndUserModel GetEndUserModel()
        {
            return new EndUserModel()
            {
                Id = 1,
                CountryCode = Domain.Enum.CountryCode.UK,
                FirstName = "James",
                Surname = "Bond",
                TitleId = 0
            };
        } 

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task ExistingEndUserNullTest()
        {
            var result = await repository.FindExistingEndUser(null);
            Assert.NotEmpty(result.ErrorList);
        }

        [Fact]
        public void CompareEndUsersTest() {
            var endUser1 = new EndUserModel()
            {
                FirstName = "John",
                Surname = "Guardiola"
            };
            var endUser2 = new EndUserModel()
            {
                FirstName = "Pep",
                Surname = "Guardiola"
            };
            var result = this.repository.CompareEndUsers(endUser1, endUser2) as Collection<EndUserDiffModel>;
            Assert.Single(result);
        }
    }
}