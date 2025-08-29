using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Api.Test.Support;
using SpecialEducationPlanning
.Api.Configuration.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Business.Repository;
using Moq;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http;
using SpecialEducationPlanning
.Business.Model;
using System;
using Shouldly;

namespace SpecialEducationPlanning
.Api.Test.Service.FeatureManagement
{
    public class FeatureManagementServiceTests : BaseTest
    {
        private readonly Mock<IOptions<LaunchDarklyConfiguration>> _mockLdConfig;
        private readonly FeatureManagementCountryConfiguration _mockCountryConfig;
        private readonly Mock<ILdClient> _mockLdClient;
        private readonly Mock<ILogger<FeatureManagementService>> _mocklogger;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IAiepRepository> _mockAiepRepository;
        private HttpClient _mockHttpClient;
        private readonly ClaimsIdentity _claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "Test@Test.co.uk")
        });
        private UserModel _user = new()
        {
            Id = 1,
            UniqueIdentifier = "Test@Test.co.uk",
            AiepId = 1
        };
        private AiepModel _Aiep = new()
        {
            Id = 1,
            AiepCode = "DY01"
        };

        public FeatureManagementServiceTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            this._mockLdConfig = new Mock<IOptions<LaunchDarklyConfiguration>>();
            this._mockLdConfig.Setup(x => x.Value).Returns(new LaunchDarklyConfiguration()
            {
                SDK_Key = "FakeSdkKey",
                HealthCheck = new LDHealthCheck()
                {
                    Access_Token = "FakeAccessToken",
                    Api_BaseUrl = "https://DoesntMatterUnitTest.com",
                    Envionment_Key = "FakeEnvironmentKey",
                    Flag_Route = "FakeFlagRoute",
                    Project_Key = "FakeProjectKey",
                    User_Email = "Test@Test.co.uk",
                    Flag_Name = "dvHealthCheck"
                }
            });
            this._mockCountryConfig = new FeatureManagementCountryConfiguration("GBR");
            this._mockLdClient = new Mock<ILdClient>(MockBehavior.Strict);
            this._mocklogger = new Mock<ILogger<FeatureManagementService>>();
            this._mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            this._mockAiepRepository = new Mock<IAiepRepository>(MockBehavior.Strict);
            this._mockHttpClient = new HttpClient();
        }

        #region GetFeatureFlagAsync

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetFeatureFlagAsync_ValidTest_Returns(bool flagResult)
        {
            //Arrange
            string flagName = "UnitTestFlag";
            RepositoryResponse<AiepModel> AiepRepositoryResponse = new(content: this._Aiep);
            RepositoryResponse<UserModel> userRepositoryResponse = new(content: this._user);

            this._mockUserRepository.Setup(x => x.GetUserByEmailAsync(this._user.UniqueIdentifier)).ReturnsAsync(userRepositoryResponse).Verifiable();
            this._mockAiepRepository.Setup(x => x.GetAiepByIdIgnoreAclAsync(this._user.AiepId.Value)).ReturnsAsync(AiepRepositoryResponse).Verifiable();
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(true).Verifiable();
            this._mockLdClient.Setup(x => x.BoolVariation(flagName, It.IsNotNull<LaunchDarkly.Sdk.Context>(), false)).Returns(flagResult).Verifiable();
            this._mocklogger.MockLog(LogLevel.Debug);

            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.GetFeatureFlagAsync(flagName, this._claimsIdentity);

            //Assert
            result.ShouldBe(flagResult);
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(4), LogLevel.Debug);
            this.ResetMocks();
        }

        [Fact]
        public async Task GetFeatureFlagAsync_LdClientNotIntialized_ReturnsFalse()
        {
            //Arrange
            string flagName = "UnitTestFlag";
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(false).Verifiable();
            this._mocklogger.MockLog(LogLevel.Debug);
            this._mocklogger.MockLog(LogLevel.Warning, "LdClient is not initialized");

            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.GetFeatureFlagAsync(flagName, this._claimsIdentity);

            //Assert
            result.ShouldBeFalse();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(2), LogLevel.Debug);
            this._mocklogger.VerifyLogger(Times.Once(), LogLevel.Warning);
            this.ResetMocks();
        }

        [Fact]
        public async Task GetFeatureFlagAsync_LdClientNotValidConnectionState_ReturnsFalse()
        {
            //Arrange
            string flagName = "UnitTestFlag";
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(false).Verifiable();
            this._mocklogger.MockLog(LogLevel.Warning);

            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.GetFeatureFlagAsync(flagName, this._claimsIdentity);

            //Assert
            result.ShouldBeFalse();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Once(), LogLevel.Warning);
            this.ResetMocks();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        public async Task GetFeatureFlagAsync_InvalidFlagName_ReturnsFalse(string flagName)
        {
            //Arrange
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(true).Verifiable();
            this._mocklogger.MockLog(LogLevel.Debug);
            this._mocklogger.MockLog(LogLevel.Warning, "Invalid Flag for Launch Darkly: Null or empty");

            ClaimsIdentity claim = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "email@email.co.uk")
            });

            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.GetFeatureFlagAsync(flagName, claim);

            //Assert
            result.ShouldBeFalse();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(2), LogLevel.Debug);
            this._mocklogger.VerifyLogger(Times.Once(), LogLevel.Warning);
            this.ResetMocks();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetFeatureFlagAsync_InvalidEmail_ReturnsFalse(string email)
        {
            //Arrange
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(true).Verifiable();
            this._mocklogger.MockLog(LogLevel.Debug);
            this._mocklogger.MockLog(LogLevel.Warning, "Invalid Email: Null or empty");

            //Claim can't be created with null value. However if Claim.Name doesn't exist the value will default to null.
            //So to simulate a null name claim, we don't create a ClaimsType.Name claim. Instead we create a different one so that
            //when the code claim.Name is executed it is unable to find a claim with 'Name' and returns null.
            Claim claim = new(email is null ? ClaimTypes.Email : ClaimTypes.Name, email is null ? "DifferentClaimTypeValue" : email);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                claim
            });

            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.GetFeatureFlagAsync("TestFlag", claimsIdentity);

            //Assert
            result.ShouldBeFalse();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(2), LogLevel.Debug);
            this._mocklogger.VerifyLogger(Times.Once(), LogLevel.Warning);
            this.ResetMocks();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task GetFeatureFlagAsync_InvalidAiepCode(string AiepCode)
        {
            //Arrange
            string flagName = "UnitTestFlag";
            this._Aiep.AiepCode = AiepCode;
            RepositoryResponse<AiepModel> AiepRepositoryResponse = new(content: this._Aiep);
            RepositoryResponse<UserModel> userRepositoryResponse = new(content: this._user);
            this._mockUserRepository.Setup(x => x.GetUserByEmailAsync(this._user.UniqueIdentifier)).ReturnsAsync(userRepositoryResponse).Verifiable();
            this._mockAiepRepository.Setup(x => x.GetAiepByIdIgnoreAclAsync(this._user.AiepId.Value)).ReturnsAsync(AiepRepositoryResponse).Verifiable();
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(true).Verifiable();
            this._mocklogger.MockLog(LogLevel.Debug);
            this._mocklogger.MockLog(LogLevel.Warning, "Invalid Aiep code: Null or Empty");

            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.GetFeatureFlagAsync(flagName, this._claimsIdentity);

            //Assert
            result.ShouldBeFalse();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(2), LogLevel.Debug);
            this._mocklogger.VerifyLogger(Times.Once(), LogLevel.Warning);
            this.ResetMocks();
        }

        #endregion


        #region HealthCheck

        [Fact]
        public async Task HealthCheck_ReturnsTrue()
        {
            //Arrange

            //HealthCheck Setups
            this._mocklogger.MockLog(LogLevel.Debug);

            //GetFeatureFlagAsync Setups
            RepositoryResponse<AiepModel> AiepRepositoryResponse = new(content: this._Aiep);
            RepositoryResponse<UserModel> userRepositoryResponse = new(content: this._user);
            this._mockUserRepository.Setup(x => x.GetUserByEmailAsync(this._user.UniqueIdentifier)).ReturnsAsync(userRepositoryResponse).Verifiable();
            this._mockAiepRepository.Setup(x => x.GetAiepByIdIgnoreAclAsync(this._user.AiepId.Value)).ReturnsAsync(AiepRepositoryResponse).Verifiable();
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(true).Verifiable();
            this._mockLdClient.Setup(x => x.StringVariation(this._mockLdConfig.Object.Value.HealthCheck.Flag_Name, It.IsNotNull<LaunchDarkly.Sdk.Context>(), Enum.GetName(typeof(FeatureFlagEnum), FeatureFlagEnum.Off))).Returns(Enum.GetName(typeof(FeatureFlagEnum), FeatureFlagEnum.On));


            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.HealthCheck();

            //Assert
            result.ShouldBeTrue();
            this._mockUserRepository.Verify();
            this._mockAiepRepository.Verify();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(5), LogLevel.Debug);

            this.ResetMocks();
        }

        [Fact]
        public async Task HealthCheck_Failed_ReturnsFalse()
        {
            //Arrange
            //HealthCheck Setups
            this._mocklogger.MockLog(LogLevel.Debug);
            this._mocklogger.MockLog(LogLevel.Warning);

            //GetFeatureFlagAsync Setups
            RepositoryResponse<AiepModel> AiepRepositoryResponse = new(content: this._Aiep);
            RepositoryResponse<UserModel> userRepositoryResponse = new(content: this._user);
            this._mockUserRepository.Setup(x => x.GetUserByEmailAsync(this._user.UniqueIdentifier)).ReturnsAsync(userRepositoryResponse).Verifiable();
            this._mockAiepRepository.Setup(x => x.GetAiepByIdIgnoreAclAsync(this._user.AiepId.Value)).ReturnsAsync(AiepRepositoryResponse).Verifiable();
            this._mockLdClient.Setup(x => x.DataSourceStatusProvider.WaitForAsync(DataSourceState.Valid, TimeSpan.FromSeconds(10))).ReturnsAsync(true).Verifiable();
            this._mockLdClient.Setup(x => x.Initialized).Returns(true).Verifiable();
            this._mockLdClient.Setup(x => x.StringVariation(this._mockLdConfig.Object.Value.HealthCheck.Flag_Name, It.IsNotNull<LaunchDarkly.Sdk.Context>(), Enum.GetName(typeof(FeatureFlagEnum), FeatureFlagEnum.Off))).Returns(Enum.GetName(typeof(FeatureFlagEnum), FeatureFlagEnum.Off));


            FeatureManagementService service = new(this._mockCountryConfig, this._mockLdConfig.Object, this._mocklogger.Object, this._mockUserRepository.Object, this._mockAiepRepository.Object, this._mockLdClient.Object, this._mockHttpClient);

            //Act
            bool result = await service.HealthCheck();

            //Assert
            result.ShouldBeFalse();
            this._mockUserRepository.Verify();
            this._mockAiepRepository.Verify();
            this._mockLdClient.Verify();
            this._mocklogger.VerifyLogger(Times.Exactly(5), LogLevel.Debug);
            this._mocklogger.VerifyLogger(Times.Exactly(1), LogLevel.Warning);

            this.ResetMocks();
        }

        #endregion

        #region SetupTests

        private void ResetMocks()
        {
            this._mockLdClient.Reset();
            this._mocklogger.Reset();
            this._mockUserRepository.Reset();
            this._mockAiepRepository.Reset();
        }

        #endregion

    }
}
