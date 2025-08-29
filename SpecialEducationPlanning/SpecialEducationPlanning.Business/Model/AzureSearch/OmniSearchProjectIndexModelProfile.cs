using AutoMapper;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

namespace SpecialEducationPlanning
.Business.Model.AzureSearch
{
    public class OmniSearchProjectIndexModelProfile : SearchIndexBaseProfiler<OmniSearchProjectIndexModel, SpecialEducationPlanning
.Domain.Entity.Project>
    {

        #region Methods Protected

        protected override IMappingExpression<SpecialEducationPlanning
.Domain.Entity.Project, OmniSearchProjectIndexModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(p => p.BuilderName, opt => opt.MapFrom(p => p.Builder.TradingName));
        }

        #endregion

    }
}
