using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using SpecialEducationPlanning
.Domain.Entity;
using Koa.Domain.Specification;
using Microsoft.EntityFrameworkCore;

namespace SpecialEducationPlanning
.Business.Repository.HouseTypeRepository
{
    public class HouseTypeRepository : BaseRepository<HousingType>, IHouseTypeRepository
    {
        private readonly IEntityRepository entityRepositoryKey;
        private readonly ILogger<HouseTypeRepository> logger;

        public HouseTypeRepository(ILogger<HouseTypeRepository> logger,
            IEntityRepository<int> entityRepositoryKey,
            IEfUnitOfWork unitOfWork,
            IDbContextAccessor dbContextAccessor,
            ISpecificationBuilder specificationBuilder,
            IEntityRepository entityRepository) : base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.logger = logger;
        }

        public async Task<HousingType> GetHousingTypeAsync(int housingTypeId, ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called GetHousingSpecificationAsync");

            var spec = new Specification<HousingType>(p => p.Id == housingTypeId);
            var housingType = await repository.Where(spec).AsNoTracking().SingleAsync();

            logger.LogDebug("HousingSpecificationCore end call GetHousingSpecificationAsync -> return HousingSpecification");

            return housingType;
        }

        public async Task<List<HousingType>> GetAllHousingTypeAndPlansAsync(ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called GetHousingSpecificationAsync");

            var spec = new Specification<HousingType>(p => true);
            var housingType = await repository.Where(spec).Include(x => x.Plans).AsNoTracking().ToListAsync();

            logger.LogDebug("HousingSpecificationCore end call GetHousingSpecificationAsync -> return HousingSpecification");

            return housingType;
        }

        public async Task<bool> UpdateHousingTypeAsync(
            int housingTypeId,
            int planId,
            DbContext context,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called UpdateHousingTypeAsync");

            await context.Set<HousingType>()
                .Where(x => x.Id == housingTypeId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(p => p.HousingSpecificationId, planId));

            logger.LogDebug("HousingSpecificationCore end call UpdateHousingTypeAsync -> return boolean");

            return true;
        }

    }
}
