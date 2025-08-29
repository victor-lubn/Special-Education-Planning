using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.ProjectService
{
    public interface IProjectService
    {
        Task<RepositoryResponse<string>> CreateUpdateProjectForCreatio(CreatioProjectDto creatio);
    }
}
