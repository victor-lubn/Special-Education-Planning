using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using SpecialEducationPlanning
.Api.Configuration;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Model.OmniSearchModel;
using SpecialEducationPlanning
.Api.Service.OmniSearch;
using SpecialEducationPlanning
.Api.Service.Search;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Api.Test.Support;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class OmniSearchControllerTest : BaseTest
    {
        private readonly Mock<IBuilderRepository> mockBuilderRepository;
        private readonly Mock<IOmniSearchService> mockOmniSearchService;
        private readonly Mock<ILogger<OmniSearchController>> mockLogger;
        private readonly OmniSearchController controller;

        public OmniSearchControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockBuilderRepository = new Mock<IBuilderRepository>(MockBehavior.Strict);
            mockOmniSearchService = new Mock<IOmniSearchService>(MockBehavior.Strict);
            mockLogger = new Mock<ILogger<OmniSearchController>>(MockBehavior.Loose);

            var builderModel = new BuilderModel
            {
                Id = 3,
                TradingName = "McGregor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "123",
                Address1 = "Rd Holland",
                BuilderStatus = BuilderStatus.None
            };
            List<BuilderModel> builderList = new List<BuilderModel>
            {
                builderModel
            };

            mockBuilderRepository.Setup(rep => rep.GetBuildersOmniSearch(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>
                {
                    Content = new Tuple<IEnumerable<BuilderModel>, int>(new List<BuilderModel>(), 7)
                });


            mockBuilderRepository.Setup(rep => rep.GetBuildersOmniSearch(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>
                {
                    Content = new Tuple<IEnumerable<BuilderModel>, int>(new List<BuilderModel>(), 7)
                });


            IBuilderRepository builderRepository = mockBuilderRepository.Object;
            var AiepRepositoryMock = new Mock<IAiepRepository>(MockBehavior.Default);
            AiepRepositoryMock.Setup(x => x.GetAllAsync<Aiep>(CancellationToken.None)).ReturnsAsync(new List<Aiep>() { new Aiep() { Id = 1 } });

            Dictionary<Type, HashSet<int>> testData = new()
            {
                { typeof(Domain.Entity.Plan), new HashSet<int>(){1,2,3,4,5 } },
                { typeof(Domain.Entity.Builder), new HashSet<int>(){1,2,3,4,5 } },
                { typeof(Domain.Entity.Version), new HashSet<int>(){1,2,3,4,5 } },
                { typeof(Domain.Entity.Project), new HashSet<int>(){1,2,3,4,5} }
            };

            var azureSearchMock = new Mock<IAzureSearchService>(MockBehavior.Default);
            azureSearchMock.Setup(x => x.OmniSearchSearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(testData);


            mockBuilderRepository.Setup(rep => rep.GetBuildersByIdsAsync(
                    testData[typeof(Domain.Entity.Builder)],
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    null)).ReturnsAsync(new RepositoryResponse<IEnumerable<BuilderModel>>
                    {
                        Content = builderList
                    }
                    );

            var planRepositoryMock = new Mock<IPlanRepository>(MockBehavior.Strict);

            IPlanRepository planRepository = planRepositoryMock.Object;

            var planModel = new PlanModel
            {
                Title = "Mr",
                KeyName = "search"
            };
            List<PlanModel> plans = new List<PlanModel>
            {
                planModel
            };

            planRepositoryMock.Setup(rep => rep.GetPlansOmniSearchAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<Tuple<ICollection<PlanModel>, int>>
                {
                    Content = new Tuple<ICollection<PlanModel>, int>(new List<PlanModel>(), 8)
                });

            planRepositoryMock.Setup(rep => rep.GetPlansByIdsAndTypeAsync(
                  testData,
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  null,
                  null)).ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>
                  {
                      Content = plans
                  }
                  );

            planRepositoryMock.Setup(rep => rep.GetPlansByIdsAndTypeAsync(
                  testData,
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  null,
                  null)).ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>
                  {
                      Content = plans
                  }
                  );

            var projectRepositoryMock = new Mock<IProjectRepository>(MockBehavior.Strict);

            IProjectRepository projectRepository = projectRepositoryMock.Object;

            var projectModel = new ProjectModelContractHub
            {
                CodeProject = "test",
                KeyName = "search"
            };
            var projects = new List<ProjectModelContractHub>
            {
                projectModel
            };

            projectRepositoryMock.Setup(rep => rep.GetProjectsByIdsAndTypeAsync(
                  testData,
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  null,
                  null)).ReturnsAsync(new RepositoryResponse<IEnumerable<ProjectModelContractHub>>
                  {
                      Content = projects
                  }
                  );

            mockOmniSearchService.Setup(x => x.ListResults(It.IsAny<IEnumerable<BuilderModel>>(), It.IsAny<IEnumerable<PlanModel>>(), It.IsAny<IEnumerable<ProjectModelContractHub>>())).Returns(new Collection<OmniSearchModel>());
            IOmniSearchService omniSearchService = mockOmniSearchService.Object;
            var userServiceMock = new Mock<IUserService>(MockBehavior.Default);
            userServiceMock.Setup(x => x.GetUserCurrentAiepId(It.IsAny<ClaimsIdentity>())).Returns(1);
            var userService = userServiceMock.Object;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            //mocking the IOptions doesn't work for unit tests, you need to instatiate a concrete instance
            //var omniSearchConfigurationMock = new Mock<IOptions<OmniSearchConfiguration>>(MockBehavior.Default);
            var omniconfig = new OmniSearchConfiguration
            {
                MaxResults = 100,
                MaxResultsEmptySearch = 1,
                TakeEntries = 30
            };
            IOptions<OmniSearchConfiguration> omniSearchConfigurationMock = Options.Create<OmniSearchConfiguration>(omniconfig);

            controller = new OmniSearchController(
                builderRepository,
                AiepRepositoryMock.Object,
                planRepository,
                projectRepository,
                omniSearchService,
                userService,
                azureSearchMock.Object,
                omniSearchConfigurationMock,
                mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };
        }

        [Fact]
        public async void PostOmniSearchResult_IfModelStateInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request";
            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = 5,
                PageNumber = 1,
                TextToSearch = "SomeTextToSearch"
            };
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //simulate a modelstate error
            controller.ModelState.AddModelError("Key", "TestError");

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<IEnumerable<string>>(response.Value);
            Assert.Contains(value, x => x.Contains("TestError"));

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfTextToSearchIsLessThan3_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request short text";
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var omniSearchRequestModel = new OmniSearchRequestModel { TextToSearch = "tx" };

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<string>(response.Value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfTextToSearchIsNullOrEmpty_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request Page number < 1";
            var omniSearchRequestModel = new OmniSearchRequestModel();
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<string>(response.Value);
            Assert.Equal("PageNumber < 1", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfRequestModelPageSizeLessThanZero_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request Page size < 0";
            var modelStateDictionary = new ModelStateDictionary();
            modelStateDictionary.AddModelError("Key", "TestError");
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = -1,
                PageNumber = 1,
                TextToSearch = "SomeTextToSearch"
            };

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<string>(response.Value);
            Assert.Contains("PageSize < 0", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfOmniSearchRequestModelPageSizeLessThan0_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request Page size < 0";
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = -1,
                TextToSearch = "SomeTextToSearch"
            };

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<string>(response.Value);
            Assert.Equal("PageSize < 0", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfTextToSearchIsNotNull_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request Page number < 1";
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = 6,
                TextToSearch = "SomeTextToSearch"
            };

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<string>(response.Value);
            Assert.Equal("PageNumber < 1", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfRequestModelPageNumberLessThanZero_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "OmniSearchController end call PostOmniSearchResult -> return Bad request Page number < 1";
            var modelStateDictionary = new ModelStateDictionary();
            modelStateDictionary.AddModelError("Key", "TestError");
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = 1,
                PageNumber = -1,
                TextToSearch = "SomeTextToSearch"
            };

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<string>(response.Value);
            Assert.Contains("PageNumber < 1", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, errorString, times);
        }

        [Fact]
        public async void PostOmniSearchResult_IfRequestModelTextToSearchIsNull_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var modelStateDictionary = new ModelStateDictionary();
            modelStateDictionary.AddModelError("Key", "TestError");
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = 1,
                PageNumber = 1,
                TextToSearch = null
            };

            //Act.
            var result = await controller.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            Assert.IsAssignableFrom<OmniSearchResponse>(response.Value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
        }


        [Theory]
        [InlineData(30)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(0)]
        public async void PostOmniSearchResult_AzureSearchTakeValueQuery(int takeValueNo)
        {
            //Arrange.
            var takeValue = $"OmniSearchController AzureSearch takeValue -> {takeValueNo}";
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var omniSearchRequestModel = new OmniSearchRequestModel
            {
                PageSize = 30,
                PageNumber = 1,
                TextToSearch = "SomeTextToSearch"
            };

            //Act.
            var omniconfig = new OmniSearchConfiguration
            {
                MaxResults = 100,
                MaxResultsEmptySearch = 1,
                TakeEntries = takeValueNo
            };

            var controllerNew = CreateOmniSearchControllerOverRide(omniconfig);
            var result = await controllerNew.PostOmniSearchResult(omniSearchRequestModel);

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            Assert.IsAssignableFrom<OmniSearchResponse>(response.Value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "OmniSearchController called PostOmniSearchResult", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, takeValue, times);
        }

        private OmniSearchController CreateOmniSearchControllerOverRide(OmniSearchConfiguration options)
        {
            //Initalising Plan Model
            var builderModel = new BuilderModel
            {
                Id = 3,
                TradingName = "McGregor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "123",
                Address1 = "Rd Holland",
                BuilderStatus = BuilderStatus.None

            };
            List<BuilderModel> builderList = new List<BuilderModel>
            {
                builderModel
            };

            //Initalising Plan Model
            var planModel = new PlanModel
            {
                Title = "Mr",
                KeyName = "search"
            };
            List<PlanModel> plans = new List<PlanModel>
            {
                planModel
            };

            //Initalising Project Model
            var projectModel = new ProjectModelContractHub
            {
                CodeProject = "Mr",
                KeyName = "search"
            };
            var projects = new List<ProjectModelContractHub>
            {
                projectModel
            };

            // Entity Types and Id's Setup 
            Dictionary<Type, HashSet<int>> testData = new()
               {
                 { typeof(Domain.Entity.Plan), new HashSet<int>(){1,2,3,4,5 } },
                 { typeof(Domain.Entity.Builder), new HashSet<int>(){1,2,3,4,5 } },
                 { typeof(Domain.Entity.Version), new HashSet<int>(){1,2,3,4,5 } },
                 { typeof(Domain.Entity.Project), new HashSet<int>(){1,2,3,4,5} }
                };


            var AiepRepositoryMock = new Mock<IAiepRepository>(MockBehavior.Default);
            AiepRepositoryMock.Setup(x => x.GetAllAsync<Aiep>(CancellationToken.None)).ReturnsAsync(new List<Aiep>() { new Aiep() { Id = 1 } });

            var azureSearchMock = new Mock<IAzureSearchService>(MockBehavior.Default);
            azureSearchMock.Setup(x => x.OmniSearchSearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(testData);

            IBuilderRepository builderRepository = mockBuilderRepository.Object;
            mockBuilderRepository.Setup(rep => rep.GetBuildersByIdsAsync(
                  testData[typeof(Domain.Entity.Builder)],
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  null)).ReturnsAsync(new RepositoryResponse<IEnumerable<BuilderModel>>
                  {
                      Content = builderList
                  }
                  );


            var planRepositoryMock = new Mock<IPlanRepository>(MockBehavior.Strict);
            IPlanRepository planRepository = planRepositoryMock.Object;
            planRepositoryMock.Setup(rep => rep.GetPlansOmniSearchAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<Tuple<ICollection<PlanModel>, int>>
                {
                    Content = new Tuple<ICollection<PlanModel>, int>(new List<PlanModel>(), 8)
                });

            planRepositoryMock.Setup(rep => rep.GetPlansByIdsAndTypeAsync(
                  testData,
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  null,
                  null)).ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>
                  {
                      Content = plans
                  }
                  );

            var projectRepositoryMock = new Mock<IProjectRepository>(MockBehavior.Strict);
            IProjectRepository projectRepository = projectRepositoryMock.Object;

            projectRepositoryMock.Setup(rep => rep.GetProjectsByIdsAndTypeAsync(
                  testData,
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  It.IsAny<int>(),
                  null,
                  null)).ReturnsAsync(new RepositoryResponse<IEnumerable<ProjectModelContractHub>>
                  {
                      Content = projects
                  }
                  );

            mockOmniSearchService.Setup(x => x.ListResults(It.IsAny<IEnumerable<BuilderModel>>(), It.IsAny<IEnumerable<PlanModel>>(), It.IsAny<IEnumerable<ProjectModelContractHub>>())).Returns(new Collection<OmniSearchModel>());
            IOmniSearchService omniSearchService = mockOmniSearchService.Object;
            var userServiceMock = new Mock<IUserService>(MockBehavior.Default);
            userServiceMock.Setup(x => x.GetUserCurrentAiepId(It.IsAny<ClaimsIdentity>())).Returns(1);
            var userService = userServiceMock.Object;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            IOptions<OmniSearchConfiguration> omniSearchConfigurationMock = Options.Create<OmniSearchConfiguration>(options);

            return new OmniSearchController(
                builderRepository,
                AiepRepositoryMock.Object,
                planRepository,
                projectRepository,
                omniSearchService,
                userService,
                azureSearchMock.Object,
                omniSearchConfigurationMock,
                mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };
        }
    }
}

