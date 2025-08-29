using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model.View;
using SpecialEducationPlanning
.Domain.Entity.View;


namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IActionLogsRepository :  IBaseRepository<ActionLogs>
    {
        Task<RepositoryResponse<IPagedQueryResult<ActionLogsModel>>> GetActionLogsFilteredAsync(IPageDescriptor searchModel);
        Task<RepositoryResponse<StringBuilder>> GetActionLogsCsv(DateTime startDate, DateTime endDate);
        Task AutomaticRemoveOldItems(DateTime dateTime, double delete);
    }
}
