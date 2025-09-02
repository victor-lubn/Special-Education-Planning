
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpecialEducationPlanning.Api.DtoModel.Teacher.Analytics;
using SpecialEducationPlanning.Business.IService.Teacher.Analytics;
using SpecialEducationPlanning.Data;
using SpecialEducationPlanning.Domain.Teacher.Analytics;
using System.Linq;
using System.Threading.Tasks;

namespace SpecialEducationPlanning.Business.Service.Teacher.Analytics
{
    public class TeacherAnalyticsService : ITeacherAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TeacherAnalyticsService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TeacherAnalyticsDto> GetTeacherAnalyticsAsync(int teacherId)
        {
            var analytics = await _context.TeacherAnalytics.FirstOrDefaultAsync(a => a.TeacherId == teacherId);
            return _mapper.Map<TeacherAnalyticsDto>(analytics);
        }

        public async Task UpdateTeacherAnalyticsAsync(int teacherId)
        {
            // This is a mock implementation. In a real scenario, you would calculate these values.
            var studentsCount = _context.Students.Count(s => s.TeacherId == teacherId);
            var averagePerformance = _context.StudentProgress.Where(p => p.Student.TeacherId == teacherId).Average(p => p.Score);
            var interventionsCount = _context.Interventions.Count(i => i.TeacherId == teacherId);

            var analytics = await _context.TeacherAnalytics.FirstOrDefaultAsync(a => a.TeacherId == teacherId);
            if (analytics == null)
            {
                analytics = new TeacherAnalytics { TeacherId = teacherId };
                _context.TeacherAnalytics.Add(analytics);
            }

            analytics.StudentsCount = studentsCount;
            analytics.AverageStudentPerformance = averagePerformance;
            analytics.InterventionsCount = interventionsCount;

            await _context.SaveChangesAsync();
        }
    }
}
