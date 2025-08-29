using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;
using Koa.Domain.Specification;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications;
using SpecialEducationPlanning
.Domain.Specification.HousingSpecifications;
using Microsoft.EntityFrameworkCore;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class HouseSpecificationRepository : BaseRepository<HousingSpecification>, IHouseSpecificationRepository
    {
        private readonly IObjectMapper mapper;
        private readonly IEntityRepository entityRepositoryKey;
        private readonly ILogger<HouseSpecificationRepository> logger;

        public HouseSpecificationRepository(ILogger<HouseSpecificationRepository> logger, 
            IEntityRepository<int> entityRepositoryKey, 
            IEfUnitOfWork unitOfWork, 
            IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, 
            ISpecificationBuilder specificationBuilder, 
            IEntityRepository entityRepository) : base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<List<HousingSpecification>> GetHousingSpecificationsByCodeAsync(List<string> housingCodes)
        {
            logger.LogDebug("HouseSpecificationRepository called GetHousingSpecificationsByCodeAsync");

            var spec = new Specification<HousingSpecification>(x => true) as ISpecification<HousingSpecification>;
            var spec1 = new HousingSpecificationByCodeSpecification(housingCodes);
            spec = spec.And(spec1);

            var housingSpecs = await repository.Where(spec)
                .Include(x => x.HousingTypes)
                .ToListAsync();

            logger.LogDebug("HouseSpecificationRepository end call GetHousingSpecificationsByCodeAsync -> return List of HousingSpecification");

            return housingSpecs;
        }

        public async Task<List<HousingSpecification>> GetHousingSpecificationsByProjectIdAsync(int projectId)
        {
            logger.LogDebug("HouseSpecificationRepository called GetHousingSpecificationsByProjectIdAsync");

            var spec = new Specification<HousingSpecification>(x => true) as ISpecification<HousingSpecification>;
            var spec1 = new Specification<HousingSpecification>(x => x.ProjectId == projectId) as ISpecification<HousingSpecification>;
            spec = spec.And(spec1);

            var housingSpecs = await repository.Where(spec)
                .Include(x => x.HousingTypes)
                .ToListAsync();

            logger.LogDebug("HouseSpecificationRepository end call GetHousingSpecificationsByProjectIdAsync -> return List of HousingSpecification");

            return housingSpecs;
        }

        public async Task<List<HousingSpecification>> GetHouseSpecsByProjectIdAsync(int projectId)
        {
            logger.LogDebug("HouseSpecificationRepository called GetHouseSpecsByProjectIdAsync");

            var spec = new Specification<HousingSpecification>(x => true) as ISpecification<HousingSpecification>;
            var spec1 = new Specification<HousingSpecification>(x => x.ProjectId == projectId) as ISpecification<HousingSpecification>;
            spec = spec.And(spec1);

            var housingSpecs = await repository.Where(spec)
                .Include(x => x.HousingTypes)
                .ThenInclude(y => y.Plans)
                .ToListAsync();

            logger.LogDebug("HouseSpecificationRepository end call GetHouseSpecsByProjectIdAsync -> return List of HousingSpecification");

            return housingSpecs;
        }
        public async Task<List<HousingSpecification>> GetHouseSpecsByProjectAndPlanIdAsync(int projectId, int planId)
        {
            logger.LogDebug("HouseSpecificationRepository called GetHouseSpecsByProjectIdAsync");

            var spec = new Specification<HousingSpecification>(x => true) as ISpecification<HousingSpecification>;
            var spec1 = new Specification<HousingSpecification>(x => x.ProjectId == projectId) as ISpecification<HousingSpecification>;
            spec = spec.And(spec1);

            var housingSpecs = await repository.Where(spec)
                .Include(x => x.HousingTypes.Where(house => house.Plans.Select(pl => pl.Id).Contains(planId)))
                .ThenInclude(y => y.Plans)
                .AsNoTracking()
                .ToListAsync();

            var housingSpecsFiltered = housingSpecs.Where(x => x.HousingTypes.Count() > 0).ToList();

            logger.LogDebug("HouseSpecificationRepository end call GetHouseSpecsByProjectIdAsync -> return List of HousingSpecification");

            return housingSpecsFiltered;
        }

    }
}
