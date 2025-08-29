using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class HousingSpecificationModelProfile : BaseProfile<HousingSpecificationModel, HousingSpecification, int>
    {
        protected override IMappingExpression<HousingSpecification, HousingSpecificationModel> MapEntityToModel()
        {
            return base.MapEntityToModel();
        }

        protected override IMappingExpression<HousingSpecificationModel, HousingSpecification> MapModelToEntity()
        {
            return base.MapModelToEntity();
        }
    }
}
