using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Report;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class ReportControllerTest : BaseTest
    {
        private readonly ReportController controller;

        public ReportControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var repoMock = new Mock<IReportRepository>(MockBehavior.Strict);
            repoMock.Setup(x => x.GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(GetMockedResponse());

            controller = new ReportController(
                repoMock.Object,
                this.LoggerFactory.CreateLogger<ReportController>());
        }

        private RepositoryResponse<ICollection<Business.Report.AiepReportModel>> GetMockedResponse()
        {
            var deportReportModel = new List<AiepReportModel>();
            return new RepositoryResponse<ICollection<AiepReportModel>>(deportReportModel);
        }

        [Fact]
        public async void GetReportWithWrongDate()
        {
            DateTime to = DateTime.UtcNow;
            DateTime from = to.AddMonths(-4);
            var result = await this.controller.GetReport(from, to) as BadRequestObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void GetReportWitValidDate()
        {
            DateTime to = DateTime.UtcNow;
            DateTime from = to.AddMonths(-2);
            var result = await this.controller.GetReport(from, to) as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }
    }
}

