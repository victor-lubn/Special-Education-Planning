
using Microsoft.AspNetCore.Mvc;
using SpecialEducationPlanning.Business.IService.Teacher.Analytics;
using System.Threading.Tasks;

namespace SpecialEducationPlanning.Api.Controllers.Teacher.Analytics
{
    [Route("api/teacher/{teacherId}/analytics")]
    [ApiController]
    public class TeacherAnalyticsController : ControllerBase
    {
        private readonly ITeacherAnalyticsService _analyticsService;

        public TeacherAnalyticsController(ITeacherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalytics(int teacherId)
        {
            var analytics = await _analyticsService.GetTeacherAnalyticsAsync(teacherId);
            if (analytics == null)
            {
                return NotFound();
            }
            return Ok(analytics);
        }
    }
}
