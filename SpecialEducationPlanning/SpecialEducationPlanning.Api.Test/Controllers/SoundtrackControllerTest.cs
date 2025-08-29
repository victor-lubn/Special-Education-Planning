using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

using Xunit;
using Moq;
using Xunit.Abstractions;
using System.Threading;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class SoundtrackControllerTest : BaseTest
    {
        private readonly SoundtrackController soundtrackController;

        private readonly Mock<ISoundtrackRepository> mockSoundtrackRepository;
        private readonly Mock<ILogger<SoundtrackController>> mockLogger;

        const int existingId = 1;
        const int nonExistingId = 100;

        public SoundtrackControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockSoundtrackRepository = new Mock<ISoundtrackRepository>(MockBehavior.Strict);
            mockLogger = new Mock<ILogger<SoundtrackController>>();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(SoundtrackModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            soundtrackController = new SoundtrackController(
                mockSoundtrackRepository.Object,
                mockLogger.Object,
                mapper.Object
                );
        }

        #region Private Methods
        private IEnumerable<SoundtrackModel> SoundTracListInstance()
        {
            var modelList = new List<SoundtrackModel>()
                {
                    new SoundtrackModel()
                    {
                        Id = 1
                    }
                };
            return modelList;
        }

        private IEnumerable<Soundtrack> SoundTracListInstanceEntity()
        {
            var modelList = new List<Soundtrack>()
                {
                    new Soundtrack()
                    {
                        Id = 1
                    }
                };
            return modelList;
        }
        #endregion

        #region Test Methods
        #region GetAssync
        /// <summary>
        /// Return a SoundtrackModel from an existing ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAsync_DefaultTake_Ok()
        {
            // Arrange
            mockSoundtrackRepository.Setup(mock => mock.FindOneAsync<Soundtrack>(existingId, CancellationToken.None))
                .ReturnsAsync(new Soundtrack());

            //Act
            var response = await soundtrackController.GetAsync(existingId);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        /// <summary>
        /// Return a Not Found Error for no existing IDs
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAsync_RepositoryError_NotFound()
        {
            //Arrange
            mockSoundtrackRepository.Setup(mock => mock.FindOneAsync<Soundtrack>(nonExistingId, CancellationToken.None))
                .ReturnsAsync((int id, CancellationToken c) =>
                {
                    return (id == existingId) ? new Soundtrack() : null;
                });

            //Act
            var response = await soundtrackController.GetAsync(nonExistingId);

            //Assert
            Assert.IsType<NotFoundResult>(response);
        }
        #endregion

        #region GetAllAssync
        /// <summary>
        /// Return a list of all Soundtracks
        /// </summary>
        [Fact]
        public async Task GetAllAsync_DefaultTake_Ok()
        {
            //Arrange
            mockSoundtrackRepository.Setup(mock => mock.GetAllAsync<Soundtrack>(CancellationToken.None)).
                ReturnsAsync(SoundTracListInstanceEntity);

            //Act
            var response = await soundtrackController.GetAllAsync() as OkObjectResult;
            var models = response.Value as List<SoundtrackModel>;

            //Assert
            Assert.NotNull(response.Value);
        }
        #endregion
        #endregion
    }
}
