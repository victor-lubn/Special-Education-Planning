using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Api.Model.OmniSearchModel;
using SpecialEducationPlanning
.Business.Model;

namespace SpecialEducationPlanning
.Api.Service.OmniSearch
{
    public class OmniSearchService : IOmniSearchService
    {
        /// <summary>
        /// </summary>
        /// <param name="buildModels"></param>
        /// <param name="planModels"></param>
        /// <param name="plansCount"></param>
        /// <param name="buildersCount"></param>
        /// <returns></returns>
        public IEnumerable<OmniSearchModel> ListResults(IEnumerable<BuilderModel> buildModels, IEnumerable<PlanModel> planModels, IEnumerable<ProjectModelContractHub> projectModels)
        {

            var results = new List<OmniSearchModel>();
            foreach (var item in buildModels)
            {
                results.Add(new OmniSearchModel
                {
                    Type = typeof(BuilderModel).Name,
                    Object = item,
                    UpdatedDate = item.UpdatedDate
                });
            }
            foreach (var item in planModels)
            {
                results.Add(new OmniSearchModel
                {
                    Type = typeof(PlanModel).Name,
                    Object = item,
                    UpdatedDate = item.UpdatedDate
                });
            }
            foreach (var item in projectModels)
            {
                results.Add(new OmniSearchModel
                {
                    Type = typeof(ProjectModel).Name,
                    Object = item,
                    UpdatedDate = item.UpdatedDate
                });
            }

            return results.OrderByDescending(item => item.UpdatedDate);
        }
    }
}