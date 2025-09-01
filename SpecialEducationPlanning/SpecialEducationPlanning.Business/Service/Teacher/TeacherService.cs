
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpecialEducationPlanning.Api.DtoModel.Teacher;
using SpecialEducationPlanning.Business.IService;
using SpecialEducationPlanning.Data;
using SpecialEducationPlanning.Domain.Teacher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpecialEducationPlanning.Business.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TeacherService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeacherDto>> GetTeachersAsync()
        {
            var teachers = await _context.Teachers.Include(t => t.Address).ToListAsync();
            return _mapper.Map<IEnumerable<TeacherDto>>(teachers);
        }

        public async Task<TeacherDto> GetTeacherByIdAsync(int id)
        {
            var teacher = await _context.Teachers.Include(t => t.Address).FirstOrDefaultAsync(t => t.Id == id);
            return _mapper.Map<TeacherDto>(teacher);
        }

        public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto)
        {
            var teacher = _mapper.Map<Teacher>(createTeacherDto);
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return _mapper.Map<TeacherDto>(teacher);
        }

        public async Task<bool> UpdateTeacherAsync(UpdateTeacherDto updateTeacherDto)
        {
            var teacher = await _context.Teachers.FindAsync(updateTeacherDto.Id);
            if (teacher == null)
            {
                return false;
            }

            _mapper.Map(updateTeacherDto, teacher);
            _context.Entry(teacher).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return false;
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
