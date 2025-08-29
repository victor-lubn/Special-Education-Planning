using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;

using Moq;
using Xunit;
using SpecialEducationPlanning
.Business.IService;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Repository;
using System.Collections.Generic;
using SpecialEducationPlanning
.Business.Model.PostCode;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class PostCodeControllerTest : BaseTest
    {
        private readonly string fullAddress = "33 Hollow Lane, Ramsey, HUNTINGDON, Cambridgeshire PE26 1DQ";

        private readonly string country = "gbr";

        private readonly string validPostCode = "PE26 1DQ";
        private readonly string nonValidPostCode = "46002";

        private readonly Mock<IPostCodeServiceFactory> _mockPostCodeServiceFactory;

        private readonly PostCodeController controller;

        public PostCodeControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockPostCodeServiceFactory = new Mock<IPostCodeServiceFactory>(MockBehavior.Strict);

            controller = new PostCodeController(
                _mockPostCodeServiceFactory.Object,
                this.LoggerFactory.CreateLogger<PostCodeController>()
            );
        }

        #region Get Adresses
        [Fact]
        public async void GetAddresses_InvalidPostCode_NotFound()
        {
            // Arrange 
            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(country).IsValidPostcode(nonValidPostCode))
                .Returns(false);

            // Act
            var result = await this.controller.GetAddresses(nonValidPostCode, country);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetAddresses_ValidPostCode_Ok()
        {
            // Arrange 
            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(country).IsValidPostcode(validPostCode))
                .Returns(true);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(country).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(country).SearchAddressAsync(validPostCode))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<AddressSearchResultModel>>());

            // Act
            var result = await this.controller.GetAddresses(validPostCode, country);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion
    }
}
