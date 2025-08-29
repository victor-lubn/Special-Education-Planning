using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.ProjectService;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Api.Service.Sap;
using Xunit;
using System.Collections.ObjectModel;
using SpecialEducationPlanning
.Business.DtoModel.BuilderSapSearch;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Test.Service.ProjectServiceTests
{
    public class ProjectServiceTest : BaseTest
    {
        public readonly ProjectService service;

        private Mock<IDbContextAccessor> mockDbContextAccessor;
        private Mock<IProjectRepository> mockProjectRepo;
        private Mock<IBuilderRepository> mockBuilderRepo;
        private Mock<IAiepRepository> mockAiepRepo;
        private Mock<IHouseSpecificationRepository> mockHousespecsRepo;
        private Mock<ISapService> mockSapService;
        private Mock<IEfUnitOfWork> mockUnitOfWork;
        private CreatioProjectDto creatioDto;

        public ProjectServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockProjectRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockBuilderRepo = new Mock<IBuilderRepository>(MockBehavior.Strict);
            mockAiepRepo = new Mock<IAiepRepository>(MockBehavior.Strict);
            mockHousespecsRepo = new Mock<IHouseSpecificationRepository>(MockBehavior.Strict);
            mockSapService = new Mock<ISapService>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);

            var logger = this.LoggerFactory.CreateLogger<ProjectService>();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();
            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            var mapperMock = new Mock<IObjectMapper>();
            mapperMock.Setup(m => m.Map<BuilderDto, BuilderModel>(It.IsAny<BuilderDto>()))
              .Returns((BuilderDto builder) =>
              {
                  return new BuilderModel()
                  {
                      AccountNumber = builder.AccountNumber,
                      TradingName = builder.TradingName,
                      Address1 = builder.Address1,
                      Postcode = builder.PostCode
                  };
              });
            mapperMock.Setup(m => m.Map<BuilderModel, Builder>(It.IsAny<BuilderModel>()))
              .Returns((BuilderModel builder) =>
              {
                  return new Builder()
                  {
                      AccountNumber = builder.AccountNumber,
                      TradingName = builder.TradingName,
                      Address1 = builder.Address1,
                      Postcode = builder.Postcode
                  };
              });
            mapperMock.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>()))
              .Returns((AiepModel Aiep) =>
              {
                  return new Aiep()
                  {
                      AiepCode = Aiep.AiepCode,
                      Address1 = Aiep.Address1,
                      Postcode = Aiep.Postcode
                  };
              });

            mockBuilderRepo.Setup(x => x.GetExistingDbBuilderAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync((BuilderModel b) =>
                {
                    if (b.AccountNumber == null)
                    {
                        return null;
                    }
                    return new Builder() { AccountNumber = b.AccountNumber };
                });
            mockBuilderRepo.Setup(x => x.AddBuilder(It.IsAny<Builder>()))
                .ReturnsAsync((Builder b) => b);

            mockAiepRepo.Setup(x => x.GetAiepByCode(It.IsAny<string>(), true))
                .ReturnsAsync((string code, bool b) =>
                {
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        return new RepositoryResponse<AiepModel>(new List<string>() { "error" });
                    }
                    return new RepositoryResponse<AiepModel>(new AiepModel() { AiepCode = code });
                });
            mockProjectRepo.Setup(x => x.GetProjectByProjectCodeAsync(It.IsAny<string>()))
                .ReturnsAsync((string code) =>
                {
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        return new RepositoryResponse<Project>((Project)null);
                    }
                    return new RepositoryResponse<Project>(new Project() { Id = 1, CodeProject = code });
                });
            mockProjectRepo.Setup(x => x.GetProjectByProjectKeyNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string code) =>
                {
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        return new RepositoryResponse<Project>((Project)null);
                    }
                    return new RepositoryResponse<Project>(new Project() { Id = 1, CodeProject = code });
                });
            mockProjectRepo.Setup(x => x.Update(It.IsAny<Project>()))
                .Returns((Project b) => b);

            mockSapService.Setup(x => x.GetPosibleSapBuilder(It.IsAny<BuilderModel>()))
                .ReturnsAsync((BuilderModel builderModel) =>
                {
                    if (builderModel.AccountNumber == null)
                    {
                        var validBuilder = new ValidationBuilderModel()
                        {
                            Builders = null,
                            Type = BuilderMatchType.Exact
                        };
                        return new RepositoryResponse<ValidationBuilderModel>(validBuilder, new List<string>() { "error" });
                    }
                    var validationBuilder = new ValidationBuilderModel()
                    {
                        Builders = new Collection<BuilderSapSearch>(),
                        Type = BuilderMatchType.Exact
                    };
                    return new RepositoryResponse<ValidationBuilderModel>(validationBuilder);
                });

            var mockService = new Mock<IProjectService>(MockBehavior.Strict);
            mockService.Setup(m => m.CreateUpdateProjectForCreatio(It.IsAny<CreatioProjectDto>()))
                .ReturnsAsync((CreatioProjectDto creatio) =>
                {
                    if (string.IsNullOrWhiteSpace(creatio.CodeProject))
                    {
                        return new RepositoryResponse<string>(new List<string>() { "error" });
                    }
                    return new RepositoryResponse<string>("keyname", new List<string>());
                });
            mockHousespecsRepo.Setup(x => x.GetHousingSpecificationsByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int projectId) =>
                {
                    if (projectId == 0)
                    {
                        return new List<HousingSpecification>();
                    }
                    return new List<HousingSpecification>()
                    {
                        new HousingSpecification () { Id = 1, Code = "code spec 1", Name = "name spec 1" },
                    };
                });

            
            service = new ProjectService(
                mockUnitOfWork.Object,
                logger,
                mockBuilderRepo.Object,
                mapperMock.Object,
                mockSapService.Object,
                mockProjectRepo.Object,
                mockAiepRepo.Object,
                mockHousespecsRepo.Object);
        }

        [Fact]
        
        public async Task PostTest_WhenAccountNumberAndAiepCodeAreNull()
        {
            //Arrange
            creatioDto = GetProjectCreationData();
            creatioDto.Builder.AccountNumber = null;
            creatioDto.AiepCode = null;

            //Act
            var result = await service.CreateUpdateProjectForCreatio(creatioDto);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Content);
            Assert.Single(result.ErrorList);
            Assert.Equal("Aiep does not exist", result.ErrorList.First());
        }

        [Fact]
        public async Task PostTest_WhenAiepCodeIsNull()
        {
            //Arrange
            creatioDto = GetProjectCreationData();
            creatioDto.AiepCode = null;

            //Act
            var result = await service.CreateUpdateProjectForCreatio(creatioDto);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Content);
            Assert.Single(result.ErrorList);
            Assert.Equal("Aiep does not exist", result.ErrorList.First());
        }

        [Fact]
        public async Task PostTest_WhenEverythingIsOK()
        {
            //Arrange
            creatioDto = GetProjectCreationData();

            //Act
            var result = await service.CreateUpdateProjectForCreatio(creatioDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.Empty(result.ErrorList);
            Assert.Equal("KeyName", result.Content);
        }

        private CreatioProjectDto GetProjectCreationData()
        {
            CreatioProjectDto creatio;

            creatio = new CreatioProjectDto()
            {
                CodeProject = "CodeProject",
                KeyName = "KeyName",
                AiepCode = "D544",
                Builder = new BuilderDto()
                {
                    AccountNumber = "1234",
                    TradingName = "TradingName",
                    Address1 = "Address1",
                    PostCode = "PostCode"
                },
                HousingSpecifications = new List<HousingSpecificationDto>()
                {
                    new HousingSpecificationDto()
                    {
                        Code = "code spec 1",
                        Name = "name spec 1",
                        HousingTypes = new List<HousingTypeDto>()
                        {
                            new HousingTypeDto()
                            {
                                Code = "code type 1",
                                Name = "name type 1",
                            },
                            new HousingTypeDto()
                            {
                                Code = "code type 2",
                                Name = "name type 2",
                            }
                        }
                    },
                    new HousingSpecificationDto()
                    {
                        Code = "code spec 2",
                        Name = "name spec 2",
                        HousingTypes = new List<HousingTypeDto>()
                        {
                            new HousingTypeDto()
                            {
                                Code = "code type 3",
                                Name = "name type 3",
                            },
                            new HousingTypeDto()
                            {
                                Code = "code type 4",
                                Name = "name type 4",
                            }
                        }
                    }
                }

            };

            return creatio;
        }
    }
}

