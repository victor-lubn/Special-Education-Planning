using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface ITitleRepository : IBaseRepository<Title>
    {
    }
}
