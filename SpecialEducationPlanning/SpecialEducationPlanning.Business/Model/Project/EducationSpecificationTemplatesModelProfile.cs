using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model.Project
{
    public class HousingSpecificationTemplatesModelProfile : BaseProfile<HousingSpecificationTemplatesModel, HousingSpecificationTemplates, int>
    {
        protected override IMappingExpression<HousingSpecificationTemplates, HousingSpecificationTemplatesModel> MapEntityToModel()
        {
            return base.MapEntityToModel();
        }

        protected override IMappingExpression<HousingSpecificationTemplatesModel, HousingSpecificationTemplates> MapModelToEntity()
        {
            return base.MapModelToEntity();
        }
    }
}
