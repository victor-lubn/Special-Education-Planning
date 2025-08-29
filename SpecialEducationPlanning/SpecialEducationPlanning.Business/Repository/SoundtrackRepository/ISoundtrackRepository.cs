using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface ISoundtrackRepository : IBaseRepository<Soundtrack>
    {
    }
}