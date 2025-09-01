
using Moq;
using SpecialEducationPlanning.Api.Controllers;
using SpecialEducationPlanning.Api.DtoModel.Teacher;
using SpecialEducationPlanning.Business.IService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpecialEducationPlanning.Api.Test.Teacher
{
    public class TeacherControllerTests
    {
        private readonly Mock<ITeacherService> _mockTeacherService;
        private readonly TeacherController _controller;

        public TeacherControllerTests()
        {
            _mockTeacherService = new Mock<ITeacherService>();
            _controller = new TeacherController(_mockTeacherService.Object);
        }

        [Fact]
        public async Task GetTeachers_ReturnsOk()
        {
            _mockTeacherService.Setup(s => s.GetTeachersAsync()).ReturnsAsync(new List<TeacherDto>());
            var result = await _controller.GetTeachers();
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
