using System.Collections.Generic;
using SpecialEducationPlanning
.Api.Model.OmniSearchModel;
using SpecialEducationPlanning
.Business.Model;

namespace SpecialEducationPlanning
.Api.Service.OmniSearch
{
    public interface IOmniSearchService
    {
        IEnumerable<OmniSearchModel> ListResults(IEnumerable<BuilderModel> buildModels, IEnumerable<PlanModel> planModels, IEnumerable<ProjectModelContractHub> projectModels);
    }
}