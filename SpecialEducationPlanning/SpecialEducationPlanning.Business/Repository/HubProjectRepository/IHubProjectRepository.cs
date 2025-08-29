using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IHubProjectRepository : IBaseRepository<Project>
    {
        Task<RepositoryResponse<ProjectModel>> CreateProjectForPlan(PlanModel value, int AiepId);
    }
}

