using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Service.CsvFile;
using SpecialEducationPlanning
.Api.Service.CsvFile.Implementation;
using SpecialEducationPlanning
.Business.Repository;
using Koa.Hosting.AspNetCore.Model;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Service
{
    public class CsvFileServiceTest : BaseTest
    {
        private readonly ICsvFileService service;

        public CsvFileServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var CsvRepo = new Mock<ICsvFileRepository>();
            var CsvFactory = new Mock<ICsv>();
            var CsvFactoryEnum = new List<ICsv>() { CsvFactory.Object };
            service = new CsvFileService(CsvFactoryEnum,
                CsvRepo.Object,
                this.LoggerFactory.CreateLogger<CsvFileService>());

            CsvFactory.Setup(x => x.IsEntity(It.IsAny<string>())).Returns((string x) =>
            {
                if (x.Contains("User"))
                    return true;
                return false;
            });

            CsvFactory.Setup(x => x.LoadCsv(It.IsAny<Stream>())).ReturnsAsync((Stream x) =>
            {
                var resp = new RepositoryResponse<int>();
                resp.Content = 1;
                return resp;
            });
        }

        [Fact]
        public void DumpCsvOkTest()
        {
            var response = service.DumpCsv("User", GetModel());
            Assert.NotNull(response);
            Assert.Equal(1, response.Result.Content);
        }

        [Fact]
        public void DumpCsvNoModelTest()
        {
            var response = service.DumpCsv("", GetModel());
            Assert.NotNull(response);
            Assert.Equal(1, response.Result.ErrorList.Count);
        }

        private MultiUploadedFileModel GetModel()
        {
            var csvText = "1,2,3,4,5,6";
            MultiUploadedFileModel model = new MultiUploadedFileModel();
            byte[] byteArray = Encoding.UTF8.GetBytes(csvText);
            MemoryStream stream = new MemoryStream(byteArray);

            model.Files = new StreamFileModel[]  {
                new StreamFileModel(stream, "filename", "contentType")
            };

            return model;
        }
    }
}
