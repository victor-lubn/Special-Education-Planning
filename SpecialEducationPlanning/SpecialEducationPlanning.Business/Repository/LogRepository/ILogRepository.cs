using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface ILogRepository : IBaseRepository<Log>
    {


        Task<RepositoryResponse<IEnumerable<LogModel>>> GetAllLog(int take, int skip);
        Task<RepositoryResponse<IPagedQueryResult<LogModel>>> GetLogsFiltered(IPageDescriptor searchModel);
        Task<RepositoryResponse<IEnumerable<LogModel>>> GetLogsFilteredAsync(string level, DateTime? initDate = null, DateTime? endDate = null);
        Task AutomaticRemoveOldItems(DateTime dateTime, double delete);
        Task<RepositoryResponse<Log>> SaveExternalLog(LogModel logModel);

    }
}
