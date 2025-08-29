using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
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
    public class AzureFileSystemTest : BaseTest
    {
        private readonly IFileStorageService<AzureStorageConfiguration> azureFileSystemService;
        private readonly string path = "blobcontainer\\demo.txt";

        public AzureFileSystemTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var azureStorageConfiguration = new AzureStorageConfiguration();

            var configurationRoot = fixture.Configuration;
            configurationRoot.GetSection("Azurestorage").Bind(azureStorageConfiguration);
            var options = Options.Create(azureStorageConfiguration);

            var logger = this.LoggerFactory.CreateLogger<IFileStorageService<AzureStorageConfiguration>>();
            azureFileSystemService = new FileStorageService<AzureStorageConfiguration>(fixture.Configuration, logger);
        }

        private async Task UploadFile()
        {
            //Arrange            
            var directory = Directory.GetCurrentDirectory();
            var stream = File.Create(directory + "/Resources/demo.txt");
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            //Act
            await azureFileSystemService.UploadAsync<object>(stream);
        }

        [Fact]
        public void TestConstructor()
        {
            //Assert
            Assert.NotNull(azureFileSystemService);
        }
    }
}