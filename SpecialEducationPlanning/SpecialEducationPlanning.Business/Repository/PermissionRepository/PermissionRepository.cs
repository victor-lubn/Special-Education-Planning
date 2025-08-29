using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        private readonly IEntityRepository<int> repositoryKey;
        public PermissionRepository(ILogger<PermissionRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) : base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.repositoryKey = repositoryKey;
        }
    }
}