using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Specification.VersionSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public static class VersionRepositoryExtensions
    {
        public static async Task<IEnumerable<Domain.Entity.Version>> GetVersionsByPlanIdAsync(this IEntityRepository<int> entityRepository, int planId)
        {
            var verSpec = new VersionsByPlanIdSpecification(planId);
            return await entityRepository.Where<Domain.Entity.Version>(verSpec).OrderByDescending(v => v.UpdatedDate).ToListAsync();
        }
    }
}
