using Koa.Domain.Search.Page;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.CountrySpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class CountryRepository : BaseRepository<Country>, ICountryRepository
    {
        private readonly IObjectMapper mapper;
        private readonly ILogger<CountryRepository> logger;

        public CountryRepository(ILogger<CountryRepository> logger, IEntityRepository<int> repositoryKey, IEntityRepository repository, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<RepositoryResponse<ICollection<RegionModel>>> GetCountryRegions(int countryId)
        {
            logger.LogDebug("CountryRepository called GetCountryRegions");

            var spec = new EntityByIdSpecification<Country>(countryId);
            var country = await repository.Where(spec).Include(x => x.Regions).FirstOrDefaultAsync();

            if (country.IsNull())
            {
                var errors = new Collection<string>();
                errors.Add(ErrorCode.EntityNotFound.GetDescription());

                logger.LogDebug("CountryRepository end call GetCountryRegions -> return Repository response Errors Entity not founds");

                return new RepositoryResponse<ICollection<RegionModel>>(errors);
            }
            var result = mapper.Map<Country, CountryModel>(country).Regions;

            logger.LogDebug("CountryRepository end call GetCountryRegions -> return Repository response Colection of Region model");

            return new RepositoryResponse<ICollection<RegionModel>>(result);
        }

        public async Task<RepositoryResponse<IPagedQueryResult<CountryModel>>> GetCountriesFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("CountryRepository called GetCountriesFilteredAsync");

            var spec = Specification<Country>.True;
            if (searchModel.Filters.Any(f => f.Member.Contains("RegionsCount")))
            {
                var filter = searchModel.Filters.Where(f => f.Member.Contains("RegionsCount")).FirstOrDefault();
                spec = spec.And(new Specification<Country>(p => p.Regions.Count().ToString() == filter.Value));
            }

            var modelSpec = SpecificationBuilder.Create<CountryModel>(searchModel.Filters);

            var query = new CountryMaterializedCountryModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("CountryRepository end call GetCountriesFilteredAsync -> return Repository response Paged Country model");

            return new RepositoryResponse<IPagedQueryResult<CountryModel>>(result);
        }

        public async Task<RepositoryResponse<CountryModel>> GetDuplicatedCountry(CountryModel countryModel)
        {
            logger.LogDebug("CountryRepository called GetDuplicatedCountry");

            var country = await repository.Where(new CountryByNameSpecification(countryModel.KeyName)).FirstOrDefaultAsync();
            if (country != null)
            {
                logger.LogError("{type}#{Country} already exists", typeof(CountryModel), country.KeyName);

                logger.LogDebug("CountryRepository end call GetDuplicatedCountry -> return Repository response Errors Entity already exists");

                return new RepositoryResponse<CountryModel>(null, ErrorCode.EntityAlreadyExist, "Country already exists");
            }
            logger.LogDebug("CountryRepository end call GetDuplicatedCountry -> return Repository response Country model");

            return new RepositoryResponse<CountryModel>(countryModel);
        }
    }
}