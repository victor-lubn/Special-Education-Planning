using System;
using System.Collections.Generic;

using Koa.Persistence.Abstractions.QueryResult;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Enum;

using Moq;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using Koa.Domain.Search.Page;
using Koa.Serialization.Json;
using Microsoft.AspNetCore.Http;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class CountryControllerTest : BaseTest
    {
        private readonly int existingCountryId = 1;
        private readonly int nonExistingCountryId = 99;

        private readonly Mock<ICountryRepository> _mockCountryRepository;
        private readonly Mock<IServiceProvider> serviceProviderMock;
        private readonly Mock<HttpContext> httpContextMock;

        private readonly CountryController countryController;

        public CountryControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockCountryRepository = new Mock<ICountryRepository>();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(CountryModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());
            this.serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            this.httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
            this.httpContextMock.SetupGet(context => context.RequestServices).Returns(this.serviceProviderMock.Object);
            countryController = new CountryController(
                _mockCountryRepository.Object,
                this.LoggerFactory.CreateLogger<CountryController>(),
                mapper.Object
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = this.httpContextMock.Object }
            };
        }

        #region Private Methods
        private CountryModel CountryExistingInstance()
        {
            var model = new CountryModel
            {
                Id = existingCountryId
            };
            return model;
        }

        private ICollection<CountryModel> CountryListInstance()
        {
            ICollection<CountryModel> countries = new List<CountryModel>
            {
                CountryExistingInstance()
            };
            return countries;
        }

        private Country CountryExistingInstanceEntity()
        {
            var entity = new Country
            {
                Id = existingCountryId
            };
            return entity;
        }

        private ICollection<Country> CountryListInstanceEntity()
        {
            ICollection<Country> countries = new List<Country>
            {
                CountryExistingInstanceEntity()
            };
            return countries;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public void Delete_NonExistingCountry_NotFound()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.FindOne<CountryModel>(nonExistingCountryId))
                .Returns(() => null);

            // Act
            var result = countryController.Delete(nonExistingCountryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistingCountry_Ok()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.FindOne<CountryModel>(existingCountryId))
                .Returns(CountryExistingInstance());

            _mockCountryRepository.Setup(cr => cr.Remove(existingCountryId))
                .Verifiable();

            // Act 
            var result = countryController.Delete(existingCountryId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Get

        #region Get by ID
        [Fact]
        public async void Get_NonExistingCountry_NotFound()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.FindOneAsync<Country>(nonExistingCountryId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await countryController.Get(nonExistingCountryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async void Get_ExistingCountry_Ok()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.FindOneAsync<Country>(existingCountryId, CancellationToken.None))
                .ReturnsAsync(CountryExistingInstanceEntity());

            // Act
            var result = await countryController.Get(existingCountryId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange 
            _mockCountryRepository.Setup(cr => cr.GetAllAsync<Country>(CancellationToken.None))
                .ReturnsAsync(CountryListInstanceEntity());

            // Act
            var result = await countryController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CountryModel>>((result as OkObjectResult).Value);
        }
        #endregion

        #region Ge with Childrens
        [Fact]
        public async void GetWithChildrens__()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.GetWithNavigationsAsync<Country>(existingCountryId))
                .ReturnsAsync(CountryExistingInstanceEntity());

            // Act
            var result = await countryController.GetWithChildrens(existingCountryId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get Countries Filtered
        [Fact]
        public async void GetCountriesFiltered_ModelError_BadRequest()
        {
            // Arrange
            countryController.ModelState.AddModelError("id", "id is null");

            // Act 
            var result = await countryController.GetCountriesFiltered(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetCountriesFiltered_ValidModel_Ok()
        {
            // Arrange
            var searchModel = new PageDescriptor(null, null)
            {
                Take = 600,
                Skip = 0,
            };

            var jsonSerializerMock = new Mock<IJsonSerializer>(MockBehavior.Strict);
            this.serviceProviderMock
                .Setup(provider => provider.GetService(typeof(IJsonSerializer)))
                .Returns(jsonSerializerMock.Object);

            _mockCountryRepository.Setup(cr => cr.GetCountriesFilteredAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<CountryModel>>()
                {
                    Content = new PagedQueryResult<CountryModel>(
                        CountryListInstance(),
                        searchModel.Take,
                        searchModel.Skip,
                        10)
                });

            // Act 
            var result = await countryController.GetCountriesFiltered(searchModel);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetCountriesFiltered_Error_BadRequest()
        {
            // Arrange
            var searchModel = new PageDescriptor(null, null)
            {
                Take = 100,
                Skip = 0,
            };

            _mockCountryRepository.Setup(cr => cr.GetCountriesFilteredAsync(searchModel))
                 .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<CountryModel>>()
                 {
                     ErrorList = { ErrorCode.GenericBusinessError.GetDescription() }
                 });

            // Act 
            var result = await countryController.GetCountriesFiltered(searchModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion

        #endregion

        #region Post

        #region Post by Model
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            countryController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await countryController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_DuplicatedError_BadRequest()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.GetDuplicatedCountry(It.IsAny<CountryModel>()))
                .ReturnsAsync(new RepositoryResponse<CountryModel>() { ErrorList = { ErrorCode.EntityAlreadyExist.GetDescription() } });

            // Act
            var result = await countryController.Post(CountryExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_Ok()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.GetDuplicatedCountry(It.IsAny<CountryModel>()))
                .ReturnsAsync(new RepositoryResponse<CountryModel>() { Content = CountryExistingInstance() });

            _mockCountryRepository.Setup(cr => cr.ApplyChangesAsync(It.IsAny<Country>(), CancellationToken.None))
                .ReturnsAsync(CountryExistingInstanceEntity());

            // Act
            var result = await countryController.Post(CountryExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CountryModel>((result as OkObjectResult).Value);
        }
        #endregion

        #endregion

        #region Put

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            countryController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await countryController.Put(It.IsAny<int>(), It.IsAny<CountryModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingArea_NotFound()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.CheckIfExistsAsync(nonExistingCountryId))
                .ReturnsAsync(false);

            // Act
            var result = await countryController.Put(nonExistingCountryId, It.IsAny<CountryModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_DuplicatedError_BadRequest()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.CheckIfExistsAsync(existingCountryId))
                .ReturnsAsync(true);

            _mockCountryRepository.Setup(cr => cr.GetDuplicatedCountry(It.IsAny<CountryModel>()))
                .ReturnsAsync(new RepositoryResponse<CountryModel>() { ErrorList = { ErrorCode.EntityAlreadyExist.GetDescription() } });

            _mockCountryRepository.Setup(cr => cr.FindOneAsync<Country>(existingCountryId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(CountryExistingInstanceEntity());

            // Act
            var result = await countryController.Put(existingCountryId, CountryExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_Ok()
        {
            // Arrange
            _mockCountryRepository.Setup(cr => cr.CheckIfExistsAsync(existingCountryId))
                .ReturnsAsync(true);

            _mockCountryRepository.Setup(cr => cr.GetDuplicatedCountry(It.IsAny<CountryModel>()))
                .ReturnsAsync(new RepositoryResponse<CountryModel>() { Content = CountryExistingInstance() });

            _mockCountryRepository.Setup(cr => cr.ApplyChangesAsync(It.IsAny<Country>(), CancellationToken.None))
                .ReturnsAsync(CountryExistingInstanceEntity());

            _mockCountryRepository.Setup(cr => cr.FindOneAsync<Country>(existingCountryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CountryExistingInstanceEntity());

            // Act
            var result = await countryController.Put(existingCountryId, CountryExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CountryModel>((result as OkObjectResult).Value);
        }
        #endregion

        #endregion

        #endregion

    }
}
