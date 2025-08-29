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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.View;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity.View;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Specification.AreaSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class ActionLogsRepository : BaseRepository<ActionLogs>, IActionLogsRepository
    {
        private readonly IEfUnitOfWork unitOfWork;
        private readonly IObjectMapper mapper;
        private readonly DataContext context;
        private readonly ILogger<ActionLogsRepository> logger;


        public ActionLogsRepository(ILogger<ActionLogsRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.context = (DataContext)dbContextAccessor.GetCurrentContext();
            this.logger = logger;
        }

        public async Task AutomaticRemoveOldItems(DateTime dateTime, double delete)
        {
            logger.LogDebug("ActionLogsRepository called AutomaticRemoveOldItems");

            UnitOfWork.BeginTransaction();

            context.DeleteOldActions(dateTime, delete);

            logger.LogDebug("ActionLogsRepository AutomaticRemoveOldItems Commit");

            UnitOfWork.Commit();

            logger.LogDebug("ActionLogsRepository end call AutomaticRemoveOldItems");
        }

        public async Task<RepositoryResponse<IPagedQueryResult<ActionLogsModel>>> GetActionLogsFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("ActionLogsRepository called GetActionLogsFilteredAsync");

            var spec = Specification<ActionLogs>.True;
            if (searchModel.Filters.Any(f => f.Member.Contains("ActionType")))
            {
                var filter = searchModel.Filters.FirstOrDefault(f => f.Member.Contains("ActionType"));
                var actionsEnum = Enum.GetValues(typeof(Domain.Enum.ActionType))
                               .Cast<Domain.Enum.ActionType>().ToList();

                var matches = actionsEnum.Where(x => x.GetDescription().ToString().ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())).ToList();
                spec = spec.And(new Specification<ActionLogs>(p => matches.Contains(p.ActionType)));
            }

            var modelSpec = this.SpecificationBuilder.Create<ActionLogsModel>(searchModel.Filters);

            var query = new ActionLogsMaterializedActionLogsModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("ActionLogsRepository end call GetActionLogsFilteredAsync -> return Paged query Action logs");

            return new RepositoryResponse<IPagedQueryResult<ActionLogsModel>>(result);
        }

        public async Task<RepositoryResponse<StringBuilder>> GetActionLogsCsv(DateTime startDate, DateTime endDate)
        {
            logger.LogDebug("ActionLogsRepository called GetActionLogsCsv");

            var spec = new ActionLogsByRangeOfDatesSpecification(startDate, endDate);
            var actionLogs = await repository.Where(spec).OrderByDescending(x => x.Date).ToListAsync();

            var sb = new StringBuilder();

            if (actionLogs.Count > 0)
            {
                sb.AppendLine("ActionType, EntityId, EntityName, User, EntityValue, Date");

                foreach (var actionLog in actionLogs)
                {
                    sb.AppendLine(actionLog.ActionType + "," + actionLog.EntityId + ", " + actionLog.EntityName + ", " + actionLog.User
                        + ", " + actionLog.EntityValue + ", " + actionLog.Date);
                }
            }

            logger.LogDebug("ActionLogsRepository end call GetActionLogsCsv -> return String csv with action logs");

            return new RepositoryResponse<StringBuilder>(sb);
        }
    }
}
