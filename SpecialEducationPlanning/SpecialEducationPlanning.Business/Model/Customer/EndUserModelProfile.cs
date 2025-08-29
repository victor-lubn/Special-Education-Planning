using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class EndUserModelProfile : BaseProfile<EndUserModel, EndUser, int>
    {
        protected override IMappingExpression<EndUser, EndUserModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(v => v.Plans, opt => opt.Ignore());
        }

        protected override IMappingExpression<EndUserModel, EndUser> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(v => v.Plans, opt => opt.Ignore());
        }
    }
}