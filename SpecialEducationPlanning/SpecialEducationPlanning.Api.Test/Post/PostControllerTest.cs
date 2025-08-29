//using SpecialEducationPlanning
.Api.Controllers;
//using SpecialEducationPlanning
.Business.Model;
//using SpecialEducationPlanning
.Business.Repository;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace SpecialEducationPlanning
.Api.Test
//{
//    public class PostControllerTest : BaseTest
//    {
//        private readonly Mock<IPostRepository> mockRepository;
//        private readonly PostController controller;

//        public PostControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
//        {
//            this.mockRepository = new Mock<IPostRepository>();
//            this.controller = new PostController(
//                 this.LoggerFactory.CreateLogger<PostController>(),
//                this.mockRepository.Object);
//        }

//        [Fact]
//        public async Task GetAsync()
//        {
//            //Arrange
//            var models = new[] {
//                    new PostModel() {
//                        Content = "TestContent"
//                        }
//                };
//            this.mockRepository
//                .Setup(x => x.GetAllAsync<PostModel>(CancellationToken.None))
//                .Returns(() =>
//                {
//                    return Task.FromResult(models.AsEnumerable());
//                });

//            // Act
//            var actionResult = await this.controller.GetAsync();

//            // Assert
//            var jsonResult = Assert.IsType<JsonResult>(actionResult);
//            Assert.Equal(models, jsonResult.Value);
//            var responseValue = Assert.IsType<PostModel[]>(jsonResult.Value);
//            Assert.Equal(models.Count(), responseValue.Length);
//            Assert.Equal(models[0].Content, responseValue[0].Content);

//        }
//    }
//}
