using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class HousingTypeModelProfile : BaseProfile<HousingTypeModel, HousingType, int>
    {
        protected override IMappingExpression<HousingType, HousingTypeModel> MapEntityToModel()
        {
            return base.MapEntityToModel();
        }

        protected override IMappingExpression<HousingTypeModel, HousingType> MapModelToEntity()
        {
            return base.MapModelToEntity();
        }
    }
}
