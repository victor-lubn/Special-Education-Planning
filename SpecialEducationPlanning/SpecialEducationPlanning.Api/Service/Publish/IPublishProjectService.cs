using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.Publish
{
    public interface IPublishProjectService
    {
        Task<RepositoryResponse<string>> SendRomItemsToCreatioAsync(int projectId);
        Task<RepositoryResponse<string>> SendRomItemsToCreatioAsync(int projectId, int planId);
    }
}
