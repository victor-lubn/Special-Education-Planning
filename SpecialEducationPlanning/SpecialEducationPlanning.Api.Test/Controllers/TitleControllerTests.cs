using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

using Moq;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class TitleControllerTests : BaseTest
    {
        private readonly Mock<ITitleRepository> mockRepository;
        private readonly TitleController titleController;
        private readonly Mock<ILogger<TitleController>> mockLogger;

        public TitleControllerTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockRepository = new Mock<ITitleRepository>();
            mockLogger = new Mock<ILogger<TitleController>>();
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(TitleModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            titleController = new TitleController(mockRepository.Object, mockLogger.Object,mapper.Object);
        }

        [Fact]
        public async void GetAllAsync_ReturnsRepositoryResponseWithTitleModels()
        {
            //Arrange.
            var titleModelCollection = new List<TitleModel>
            {
                new TitleModel(),
                new TitleModel(),
                new TitleModel()
            };

            var titleModelCollectionEntity= new List<Title>
            {
                new Title(),
                new Title(),
                new Title()
            };

            mockRepository.Setup(rep => rep.GetAllAsync<Title>(System.Threading.CancellationToken.None)).ReturnsAsync(titleModelCollectionEntity);

            //Act.
            var result = await titleController.GetAllAsync();

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<TitleModel>>(response.Value);
            Assert.True(value.All(x => x.GetType() == typeof(TitleModel)));
        }
    }
}
