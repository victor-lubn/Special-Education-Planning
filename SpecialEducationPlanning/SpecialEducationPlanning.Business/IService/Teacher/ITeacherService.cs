
using SpecialEducationPlanning.Api.DtoModel.Teacher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpecialEducationPlanning.Business.IService
{
    public interface ITeacherService
    {
        Task<IEnumerable<TeacherDto>> GetTeachersAsync();
        Task<TeacherDto> GetTeacherByIdAsync(int id);
        Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto);
        Task<bool> UpdateTeacherAsync(UpdateTeacherDto updateTeacherDto);
        Task<bool> DeleteTeacherAsync(int id);
    }
}
