using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class SoundtrackRepository : BaseRepository<Soundtrack>, ISoundtrackRepository
    {
        private readonly IObjectMapper mapper;

        public SoundtrackRepository(ILogger<SoundtrackRepository> logger,
            IEntityRepository<int> entityRepositoryKey,
            IEfUnitOfWork unitOfWork,
            IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor,
            ISpecificationBuilder specificationBuilder,
            IEntityRepository entityRepository)
            : base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.mapper = mapper;
        }


    }
}