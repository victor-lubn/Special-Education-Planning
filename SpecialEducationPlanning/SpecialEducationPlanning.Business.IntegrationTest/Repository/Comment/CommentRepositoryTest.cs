using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Action
{
    public class CommentRepositoryTest : BaseTest
    {
        private readonly ICommentRepository repository;

        public CommentRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<ICommentRepository>();
        }

        [Fact]
        public async Task GetPlanActionsFoundedTestAsync()
        {
            var comment = new Comment()
            {
                CreatedDate = DateTime.UtcNow,
                EntityId = 1,
                EntityName = "PlanModel",
                Text = "test",
                Id = 1
            };

            comment = await this.repository.Add(comment);

            var result = await repository.GetModelComments<PlanModel>(1);
            var collectionCount = result.Content.Count();
            
            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            //Assert.Equal(1, collectionCount);
        }

        [Fact]
        public async Task GetPlanActionsNotFoundedTestAsync()
        {
            var comment = new Comment()
            {
                CreatedDate = DateTime.UtcNow,
                EntityId = 1,
                EntityName = "PlanModel",
                Text = "test",
                Id = 1
            };

            comment = await this.repository.Add(comment);

            var result = await repository.GetModelComments<PlanModel>(2);
            var collectionCount = result.Content.Count();
            Assert.Equal(0, collectionCount);
        }

        [Fact]
        public async Task CreateCommentTestAsync()
        {
            var comment = this.repository.CreateComment<PlanModel>(new CommentModel()
            {
                Text = "prueba",
                CreatedDate = DateTime.UtcNow,
                User = "User"
            }, 1, "user");

            var result = await repository.GetModelComments<PlanModel>(1);
            var collectionCount = result.Content.Count();
            Assert.Equal(1, collectionCount);
        }
    }
}