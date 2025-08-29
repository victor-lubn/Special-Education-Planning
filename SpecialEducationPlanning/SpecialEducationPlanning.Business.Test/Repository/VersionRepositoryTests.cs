using System.Threading.Tasks;
using System;
using System.Linq;
using System.Data.Entity.SqlServer;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Domain.Service.Search;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Test;

using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository;
using Koa.Domain.Specification.Search;
using Koa.Hosting.AspNetCore.Model;

using Moq;
using Xunit.Abstractions;
using Xunit;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;
using SpecialEducationPlanning
.Api.Test.Support;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    public class VersionRepositoryTests : BaseTest
    {
        private readonly int versionId = 1;
        private readonly string versionNotes = "01";
        private readonly string quoteOrderNumber = "01";
        private readonly int planId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositorykey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IFileStorageService<AzureStorageConfiguration>> mockFileStorageService;
        private readonly Mock<IAzureSearchManagementService> mockAzureSearchManagementService;
        private readonly Mock<ILogger<VersionRepository>> mockLogger;

        private readonly VersionRepository repository;

        public VersionRepositoryTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositorykey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockFileStorageService = new Mock<IFileStorageService<AzureStorageConfiguration>>(MockBehavior.Strict);
            mockAzureSearchManagementService = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);
            mockLogger = new Mock<ILogger<VersionRepository>>();

            repository = new VersionRepository(
                mockLogger.Object,
                mockEntityRepositorykey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockFileStorageService.Object,
                mockAzureSearchManagementService.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                new SqlAzureExecutionStrategy(),
                mockEntityRepository.Object
            );
        }

        #region Privates

        private static string SaveVersionTestData()
        {
            var versionModel = new MultiUploadedFileModel<VersionInfoModel>
            {
                Files = new List<StreamFileModel>(),
                Model = new VersionInfoModel()
            };
            var data = JsonSerializer.Serialize(versionModel);
            return data;
        }

        #endregion

        #region ModifyVersionNotes

        [Fact]
        public async Task ModifyVersionNotes_Valid_Id_And_Notes_Returns_VersionModel()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepositorykey.Setup(er => er.FindOneAsync<Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Version());

            mockObjectMapper.Setup(m => m.Map<Version, VersionModel>(It.IsAny<Version>()))
                .Returns(new VersionModel());

            //Act
            var response = await repository.ModifyVersionNotes(this.versionId, this.versionNotes);

            //Assert
            response.IsNotNull();
            response.GetType().Equals(typeof(VersionModel));
            response.ErrorList.Count.Equals(0);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "VersionRepository ModifyVersionNotes call Commit", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "VersionRepository end call ModifyVersionNotes -> return Repository response VersionModel", times);
        }

        [Fact]
        public async Task ModifyVersionNotes_Invalid_Id_And_Notes_Returns_Error()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepositorykey.Setup(er => er.FindOneAsync<Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Version));

            mockObjectMapper.Setup(m => m.Map<Version, VersionModel>(It.IsAny<Version>()))
                .Returns(new VersionModel());

            //Act
            var response = await repository.ModifyVersionNotes(this.versionId, this.versionNotes);

            //Assert
            response.IsNotNull();
            response.ErrorList.Count.Equals(1);
            response.ErrorList.First().Equals("EntityNotFound Version Not Found from ModifyVersionNotes");

            this.mockLogger.VerifyLogger(LogLevel.Debug, "Version Not Found", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "VersionRepository end call ModifyVersionNotes -> return Repository response Errors Entity not found", times);
        }

        #endregion

        #region ModifyVersionNotes

        [Fact]
        public async Task ModifyVersionNotesAndQuote_Valid_Id_And_Notes_Returns_VersionModel()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepositorykey.Setup(er => er.FindOneAsync<Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Version());

            mockObjectMapper.Setup(m => m.Map<Version, VersionModel>(It.IsAny<Version>()))
                .Returns(new VersionModel());

            //Act
            var response = await repository.ModifyVersionNotesAndQuote(this.versionId, this.versionNotes, this.quoteOrderNumber);

            //Assert
            response.IsNotNull();
            response.GetType().Equals(typeof(VersionModel));
            response.ErrorList.Count.Equals(0);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "VersionRepository ModifyVersionNotesAndQuote call Commit", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "VersionRepository end call ModifyVersionNotesAndQuote -> return Repository response VersionModel", times);
        }

        [Fact]
        public async Task ModifyVersionNotesAndQuote_Invalid_Id_And_Notes_Returns_Error()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepositorykey.Setup(er => er.FindOneAsync<Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Version));

            mockObjectMapper.Setup(m => m.Map<Version, VersionModel>(It.IsAny<Version>()))
                .Returns(new VersionModel());

            //Act
            var response = await repository.ModifyVersionNotesAndQuote(this.versionId, this.versionNotes, this.quoteOrderNumber);

            //Assert
            response.IsNotNull();
            response.ErrorList.Count.Equals(1);
            response.ErrorList.First().Equals("EntityNotFound Version Not Found from ModifyVersionNotes");

            this.mockLogger.VerifyLogger(LogLevel.Debug, "Version Not Found", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "VersionRepository end call ModifyVersionNotesAndQuote -> return Repository response Errors Entity not found", times);
        }

        #endregion

        #region SaveVersion

        [Fact(Skip = "Cannot mock the extension method CreateVersionAsync()")]
        public async void SaveVersion_Valid_Parameters_Returns_version()
        {
            //Arrange
            VersionInfoModel vim = new();
            string data = VersionRepositoryTests.SaveVersionTestData();
            
            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepositorykey.Setup(er => er.FindOneAsync<Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Version());

            mockEntityRepositorykey.Setup(er => er.CreateVersionAsync(It.IsAny<DbContext>(), It.IsAny<Plan>(), this.versionId, vim, this.LoggerFactory.CreateLogger<VersionRepository>()))
                .ReturnsAsync(new Version());

            //Act
            var response = await repository.SaveVersion(this.planId, this.versionId, vim);
        }

        #endregion
    }
}
