using Koa.Domain.Specification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Model.AutomaticArchiveModel;
using SpecialEducationPlanning
.Api.Service.Publish;
using SpecialEducationPlanning
.Api.Service.Search;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Mapper;
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
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class ProjectControllerTest : BaseTest
    {
        private readonly Mock<IOptions<AzureSearchConfiguration>> _mockAzureSearchConfiguration;
        private readonly Mock<IAzureSearchService> _mockAzureSearchService;
        private readonly Mock<IPublishProjectService> _mockPublishProjectService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IOptions<AutomaticArchiveConfiguration>> _mockOptions;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<HttpContext> httpContextMock;

        private readonly string userFullName = "Stephen Dunn";

        private const int existingProjectId = 1;
        private const int nonExistingProjectId = 99;

        public ProjectControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockProjectRepository = new Mock<IProjectRepository>(MockBehavior.Default);
            _mockProjectRepository.Setup(m => m.GetAllAsync<Project>(CancellationToken.None)).ReturnsAsync(GetListEntity());
            _mockProjectRepository.Setup(m => m.CheckIfExistsAsync(It.IsAny<int>())).ReturnsAsync((int x) => {
                if (x == 1)
                {
                    return true;
                }
                return false;
            });
            _mockProjectRepository.Setup(m => m.FindOneAsync<Project>(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((int x, CancellationToken token) =>
            {
                if (x == 1)
                {
                    return GetInstanceEntity();
                }
                return null;
            });
            _mockProjectRepository.Setup(m => m.Remove(It.IsAny<int>())).Verifiable();
            _mockProjectRepository.Setup(repo => repo.ApplyChangesAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project b, CancellationToken token) => b);


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(ProjectModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            _mockAzureSearchConfiguration = new Mock<IOptions<AzureSearchConfiguration>>();
            _mockAzureSearchService = new Mock<IAzureSearchService>(MockBehavior.Strict);
            _mockPublishProjectService = new Mock<IPublishProjectService>(MockBehavior.Strict);
            _mockOptions = new Mock<IOptions<AutomaticArchiveConfiguration>>(MockBehavior.Default);
            _mockOptions.Setup(o => o.Value).Returns(new AutomaticArchiveConfiguration { Archive = "600", Delete = "600"});
            this.httpContextMock = new Mock<Microsoft.AspNetCore.Http.HttpContext>(MockBehavior.Strict);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            httpContextMock.SetupGet(context => context.User).Returns(user);

            controller = new ProjectController(_mockProjectRepository.Object,
                _mockUserService.Object,
                _mockAzureSearchService.Object,
                _mockAzureSearchConfiguration.Object,
                this.LoggerFactory.CreateLogger<ProjectController>(),
                mapper.Object,
                _mockPublishProjectService.Object,
                _mockOptions.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = this.httpContextMock.Object }
            };
        }

        public readonly ProjectController controller;

        private ProjectModel GetInstance()
        {
            var model = new ProjectModel
            {
                Id = 1
            };
            return model;
        }
        private Project GetInstanceEntity()
        {
            var model = new Project
            {
                Id = 1
            };
            return model;
        }

        private ICollection<ProjectModel> GetList()
        {
            ICollection<ProjectModel> models = new List<ProjectModel>
            {
                new ProjectModel()
            };
            return models;
        }
        private ICollection<Project> GetListEntity()
        {
            ICollection<Project> models = new List<Project>
            {
                new Project()
            };
            return models;
        }

        private List<ProjectModel> GetTestProjects()
        {
            var project = new List<ProjectModel>
            {
                new ProjectModel {Id = 1, CodeProject = "Demo1"},
                new ProjectModel {Id = 2, CodeProject = "Demo2"},
                new ProjectModel {Id = 3, CodeProject = "Demo3"},
                new ProjectModel {Id = 4, CodeProject = "Demo4"},
                new ProjectModel {Id = 5, CodeProject = "Demo5"}
            };
            return project;
        }

        private Project ProjectExistingInstanceEntity()
        {
            var model = new Project
            {
                Id = existingProjectId,
                CodeProject = "Demo1",
                AiepId = 1
            };
            return model;
        }

        private ProjectModel ProjectExistingInstance()
        {
            var model = new ProjectModel
            {
                Id = existingProjectId,
                CodeProject = "Demo1",
                AiepId = 1
            };
            return model;
        }

        private Project ProjectWithOldPlans()
        {
            var model = new Project
            {
                Plans = new List<Plan> {
                    new Plan
                    {
                        PlanName = "Demo1",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1))
                    },
                    new Plan
                    {
                        PlanName = "Demo2",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1))
                    }
                }
            };

            return model;
        }
        
        private Project ProjectWithNewPlans()
        {
            var model = new Project
            {
                Plans = new List<Plan> {
                    new Plan
                    {
                        PlanName = "Demo1",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now
                    },
                    new Plan
                    {
                        PlanName = "Demo2",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now
                    }
                }
            };

            return model;
        }
        
        private Project ProjectWithPlansWithOldVersions()
        {
            var model = new Project
            {
                Plans = new List<Plan> {
                    new Plan
                    {
                        PlanName = "Demo1",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1)),
                        Versions = new List<Version>
                        {
                            new Version
                            {
                                VersionNumber = 1,
                                UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1))
                            },
                            new Version
                            {
                                VersionNumber = 2,
                                UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1))
                            }
                        }
                    },
                    new Plan
                    {
                        PlanName = "Demo2",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1)),
                        Versions = new List<Version>
                        {
                            new Version
                            {
                                VersionNumber = 3,
                                UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1))
                            },
                            new Version
                            {
                                VersionNumber = 4,
                                UpdatedDate = DateTime.Now.Subtract(new TimeSpan(600, 1, 1, 1))
                            }
                        }
                    }
                }
            };

            return model;
        }

        private Project ProjectWithPlansWithNewVersions()
        {
            var model = new Project
            {
                Plans = new List<Plan> {
                    new Plan
                    {
                        PlanName = "Demo1",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now,
                        Versions = new List<Version>
                        {
                            new Version
                            {
                                VersionNumber = 1,
                                UpdatedDate = DateTime.Now
                            },
                            new Version
                            {
                                VersionNumber = 2,
                                UpdatedDate = DateTime.Now
                            }
                        }
                    },
                    new Plan
                    {
                        PlanName = "Demo2",
                        PlanState = PlanState.Active,
                        UpdatedDate = DateTime.Now,
                        Versions = new List<Version>
                        {
                            new Version
                            {
                                VersionNumber = 3,
                                UpdatedDate = DateTime.Now
                            },
                            new Version
                            {
                                VersionNumber = 4,
                                UpdatedDate = DateTime.Now
                            }
                        }
                    }
                }
            };

            return model;
        }

        [Fact]
        public async Task DeleteNotFoundTest()
        {
            //Arrange
            var idProject = 6;

            // Act  
            var result = await controller.Delete(idProject) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteTest()
        {
            //Arrange
            var idProject = 1;

            // Act 
            var result = await controller.Delete(idProject) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetAllTest()
        {
            //Act
            var result = await controller.GetAll();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdNotFoundTest()
        {
            //Arrange
            var idProject = 6;

            //Act
            var result = await controller.Get(idProject) as NotFoundResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetByIdTest()
        {
            //Arrange
            var idProject = 1;

            // Act
            var result = await controller.Get(idProject) as OkObjectResult;
            var model = result.Value as ProjectModel;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(model.Id, idProject);
        }

        [Fact]
        public async Task PostTest()
        {
            //Arrange
            var projectModel = new ProjectModel { Id = 6, CodeProject = "Test" };

            //Act
            var result = await controller.Post(projectModel) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task PutNotFoundTest()
        {
            //Arrange
            var projectModel = new ProjectModel { Id = 6, CodeProject = "Test" };

            // Act  
            var result = await controller.Put(projectModel.Id, projectModel) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task PutTest()
        {
            //Arrange
            var projectModel = new ProjectModel { Id = 1, CodeProject = "TestUpdate" };

            //Act
            var result = await controller.Put(projectModel.Id, projectModel) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        #region Change Project State
        [Fact]
        public async void ChangeProjectState_NonExistingProject_NotFound()
        {
            // Arrange
            _mockProjectRepository.Setup(pr => pr.FindOneAsync<Project>(nonExistingProjectId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act  
            var result = await controller.ChangeProjectState(nonExistingProjectId, PlanState.Active);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void ChangeProjectState_NoAction_Ok()
        {
            // Arrange
            var planState = PlanState.Archived;

            _mockProjectRepository.Setup(pr => pr.FindOneAsync<Project>(existingProjectId, CancellationToken.None))
                .ReturnsAsync(ProjectExistingInstanceEntity);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockProjectRepository.Setup(pr => pr.ChangeProjectStateAsync(It.IsAny<ProjectModel>(), planState))
                .ReturnsAsync(new RepositoryResponse<ProjectModel>() { Content = null });

            // Act  
            var result = await controller.ChangeProjectState(existingProjectId, planState);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void ChangeProjectState_Ok()
        {
            // Arrange
            var planState = PlanState.Archived;

            _mockProjectRepository.Setup(pr => pr.FindOneAsync<Project>(existingProjectId, CancellationToken.None))
                .ReturnsAsync(ProjectExistingInstanceEntity());

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockProjectRepository.Setup(pr => pr.ChangeProjectStateAsync(It.IsAny<ProjectModel>(), planState))
                .ReturnsAsync(new RepositoryResponse<ProjectModel>() { Content = ProjectExistingInstance() });

            // Act  
            var result = await controller.ChangeProjectState(existingProjectId, planState);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async void ChangeProjectState_Restore_NoAction_Ok()
        {
            // Arrange
            var planState = PlanState.Active;

            _mockProjectRepository.Setup(pr => pr.FindOneAsync<Project>(existingProjectId, CancellationToken.None))
                .ReturnsAsync(ProjectExistingInstanceEntity);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockProjectRepository.Setup(pr => pr.ChangeProjectStateAsync(It.IsAny<ProjectModel>(), planState))
                .ReturnsAsync(new RepositoryResponse<ProjectModel>() { Content = null });

            // Act  
            var result = await controller.ChangeProjectState(existingProjectId, planState);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void ChangeProjectState_Restore_Ok()
        {
            // Arrange
            var planState = PlanState.Active;

            _mockProjectRepository.Setup(pr => pr.FindOneAsync<Project>(existingProjectId, CancellationToken.None))
                .ReturnsAsync(ProjectExistingInstanceEntity());

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockProjectRepository.Setup(pr => pr.ChangeProjectStateAsync(It.IsAny<ProjectModel>(), planState))
                .ReturnsAsync(new RepositoryResponse<ProjectModel>() { Content = ProjectExistingInstance() });

            // Act  
            var result = await controller.ChangeProjectState(existingProjectId, planState);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void AutomaticArchiveTest_OldPlans()
        {
            // Arrange
            _mockProjectRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { ProjectWithOldPlans() });

            // Act
            var result = await controller.AutomaticArchive(600, null);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        
        [Fact]
        public async void AutomaticArchiveTest_NewPlans()
        {
            // Arrange
            _mockProjectRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { ProjectWithNewPlans() });

            // Act
            var result = await controller.AutomaticArchive(600, null);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        
        [Fact]
        public async void AutomaticArchiveTest_OldPlans_OldVersions()
        {
            // Arrange
            _mockProjectRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { ProjectWithPlansWithOldVersions() });

            // Act
            var result = await controller.AutomaticArchive(600, null);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void AutomaticArchiveTest_NewPlans_NewVersions()
        {
            // Arrange
            _mockProjectRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { ProjectWithPlansWithNewVersions() });

            // Act
            var result = await controller.AutomaticArchive(600, null);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion
    }
}
