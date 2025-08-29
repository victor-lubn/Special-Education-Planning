using Microsoft.Extensions.DependencyInjection;
using System;
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
.Business.Test.Repository.Action
{
    public class ActionRepositoryTest : BaseTest
    {
        private readonly IActionRepository repository;

        public ActionRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IActionRepository>();
        }

        [Fact]
        public async Task GetPlanActionsFoundedTestAsync()
        {
            var action = new Domain.Entity.Action()
            {
                ActionType = ActionType.Create,
                Date = DateTime.UtcNow,
                EntityId = 1,
                EntityName = "Plan",
                Id = 1
            };
            
            action = await this.repository.Add(action);

            var result = await repository.GetModelActions<PlanModel>(action.Id);
            var collectionCount = result.Content.Count();
            Assert.Equal(1, collectionCount);
        }

        [Fact]
        public async Task GetPlanActionsNotFoundedTestAsync()
        {
            var action = new Domain.Entity.Action()
            {
                ActionType = ActionType.Create,
                Date = DateTime.UtcNow,
                EntityId = 1,
                EntityName = "PlanModel",
                Id = 1
            };

            action = await this.repository.Add(action);

            var result = await repository.GetModelActions<PlanModel>(2);
            var collectionCount = result.Content.Count();
            Assert.Equal(0, collectionCount);
        }

        [Fact]
        public async Task GetPlanActionsRangeTestAsync()
        {
            var action = new Domain.Entity.Action()
            {
                ActionType = ActionType.Update,
                Date = DateTime.UtcNow,
                EntityId = 1,
                EntityName = "VersionModel",
                Id = 1
            };

            action = await this.repository.Add(action);

            var result = await repository.GetModelActions<PlanModel>(2);
            var collectionCount = result.Content.Count();
            Assert.Equal(0, collectionCount);
        }
    }
}