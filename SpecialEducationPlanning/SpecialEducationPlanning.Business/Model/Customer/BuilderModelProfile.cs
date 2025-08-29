using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{

    public class BuilderModelProfile : BaseProfile<BuilderModel, Builder, int>
    {

        #region Methods Protected

        protected override IMappingExpression<BuilderModel, Builder> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(b => b.BuilderEducationerAieps, opt => opt.Ignore())
                .ForMember(b => b.Plans, opt => opt.Ignore())
                .ForMember(b => b.Id, opt => opt.Ignore())
             ;
        }

        protected override IMappingExpression<Builder, BuilderModel> MapEntityToModel()
        {

            return base.MapEntityToModel().ForMember(b => b.BuilderEducationerAieps, opt => opt.Ignore());

        }

        #endregion

    }

}

