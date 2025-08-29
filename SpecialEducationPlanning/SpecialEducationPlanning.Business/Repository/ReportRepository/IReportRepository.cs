using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Report;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IReportRepository : IBaseRepository<Aiep>
    {
        Task<RepositoryResponse<ICollection<AiepReportModel>>> GetReport(DateTime fromDate, DateTime toDate);
    }
}

