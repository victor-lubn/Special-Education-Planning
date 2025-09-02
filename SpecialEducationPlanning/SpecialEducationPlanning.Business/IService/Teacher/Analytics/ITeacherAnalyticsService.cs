
using SpecialEducationPlanning.Api.DtoModel.Teacher.Analytics;
using System.Threading.Tasks;

namespace SpecialEducationPlanning.Business.IService.Teacher.Analytics
{
    public interface ITeacherAnalyticsService
    {
        Task<TeacherAnalyticsDto> GetTeacherAnalyticsAsync(int teacherId);
        Task UpdateTeacherAnalyticsAsync(int teacherId);
    }
}
