using AutoMapper;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

namespace SpecialEducationPlanning
.Business.Model.AzureSearch
{

    public class OmniSearchPlanIndexModelProfile : SearchIndexBaseProfiler<OmniSearchPlanIndexModel, Plan>
    {

        #region Methods Protected

        protected override IMappingExpression<Plan, OmniSearchPlanIndexModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(pi => pi.EndUserAddress, opt => opt.MapFrom(p => p.EndUser.Address0))
                .ForMember(pi => pi.EndUserPostcode, opt => opt.MapFrom(p => p.EndUser.Postcode))
                .ForMember(pi => pi.EndUserSurname, opt => opt.MapFrom(p => p.EndUser.Surname))
                .ForMember(pi => pi.EducationerSurname, opt => opt.MapFrom(p => p.Educationer.Surname));
        }

        #endregion

    }

}
