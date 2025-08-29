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
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Specification.LogSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class LogRepository : BaseRepository<Log>, ILogRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly IEntityRepository entityRepository;
        private readonly IObjectMapper mapper;
        private readonly DataContext context;

        private readonly ILogger<LogRepository> logger;

        public LogRepository(ILogger<LogRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository entityRepository) : base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)

        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.entityRepository = entityRepository;
            this.mapper = mapper;
            this.context = (DataContext)dbContextAccessor.GetCurrentContext();

            this.logger = logger;
        }
        public async Task<RepositoryResponse<IEnumerable<LogModel>>> GetAllLog(int take = 1000, int skip = 0)
        {
            logger.LogDebug("LogRepository called GetAllLog");

            var logModels = mapper.Map<Log, LogModel>(await entityRepositoryKey.GetAll<Log>().Skip(skip).Take(take).ToListAsync());

            logger.LogDebug("LogRepository end call GetAllLog -> return Repository response List of Log model");

            return new RepositoryResponse<IEnumerable<LogModel>>(logModels);
        }
        public async Task<RepositoryResponse<IPagedQueryResult<LogModel>>> GetLogsFiltered(IPageDescriptor searchModel)
        {
            logger.LogDebug("LogRepository called GetLogsFiltered");

            var spec = Specification<Log>.True;

            var modelSpec = SpecificationBuilder.Create<LogModel>(searchModel.Filters);

            var query = new LogMaterializedLogModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = entityRepositoryKey.Query(query);

            logger.LogDebug("LogRepository end call GetLogsFiltered -> return Paged query LogModel");

            return new RepositoryResponse<IPagedQueryResult<LogModel>>(result);
        }

        public async Task<RepositoryResponse<IEnumerable<LogModel>>> GetLogsFilteredAsync(string level, DateTime? initDate = null, DateTime? endDate = null)
        {
            logger.LogDebug("LogRepository called GetLogsFilteredAsync");

            var response = new RepositoryResponse<LogModel>();

            var spec = Specification<Log>.True;
            if (initDate.HasValue && endDate.HasValue)
            {
                spec = spec.And(new LogsByDateRangeSpecification(initDate.Value, endDate.Value));
            }
            if (!string.IsNullOrEmpty(level))
            {
                spec = spec.And(new LogsByLevelSpecification(level));
            }

            var logs = await repository.Where(spec).OrderByDescending(x => x.TimeStamp).Skip(0).Take(1000).ToListAsync();

            var logModels = mapper.Map<IEnumerable<Log>, IEnumerable<LogModel>>(logs);

            logger.LogDebug("LogRepository end call GetLogsFilteredAsync -> return Repository response List of LogModel");

            return new RepositoryResponse<IEnumerable<LogModel>>(logModels);
        }

        public async Task AutomaticRemoveOldItems(DateTime dateTime, double delete)
        {
            logger.LogDebug("LogRepository called AutomaticRemoveOldItems");

            UnitOfWork.BeginTransaction();

            context.DeleteOldLogs(dateTime, delete);

            UnitOfWork.Commit();

            logger.LogDebug("LogRepository end call AutomaticRemoveOldItems");
        }

        public async Task<RepositoryResponse<Log>> SaveExternalLog(LogModel logModel)
        {
            logger.LogDebug("LogRepository called SaveExternalLog");

            var log = mapper.Map<LogModel, Log>(logModel);

            UnitOfWork.BeginTransaction();

            log.ExternalSource = true;
            log = repository.Add(log);

            logger.LogDebug("LogRepository SaveExternalLog call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("LogRepository end call SaveExternalLog -> return Repository response Log");

            return new RepositoryResponse<Log>(log);
        }
    }
}