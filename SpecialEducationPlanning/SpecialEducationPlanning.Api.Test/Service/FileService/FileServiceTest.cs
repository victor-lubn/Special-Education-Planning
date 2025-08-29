using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SpecialEducationPlanning
.Api.Model.FileStorageModel;
using SpecialEducationPlanning
.Api.Service.FileStorage;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Service
{
    public class FileStorageServiceTest : BaseTest
    {
        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;
        public FileStorageServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var azureFileServiceConfiguration = new AzureStorageConfiguration();
            var configurationRoot = fixture.Configuration;
            configurationRoot.GetSection("Azurestorage").Bind(azureFileServiceConfiguration);

            var options = Options.Create(azureFileServiceConfiguration);

            fileStorageService = new FileStorageService<AzureStorageConfiguration>(fixture.Configuration, this.LoggerFactory.CreateLogger<FileStorageService<AzureStorageConfiguration>>());
        }

        [Fact]
        public void TestConstructor()
        {
            Assert.NotNull(fileStorageService);
        }
    }
}
