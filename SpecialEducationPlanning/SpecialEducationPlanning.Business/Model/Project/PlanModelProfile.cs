using AutoMapper;
using SpecialEducationPlanning
.Business.Constants;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class PlanModelProfile : BaseProfile<PlanModel, Plan, int>
    {
        protected override IMappingExpression<Plan, PlanModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(v => v.Project, opt => opt.Ignore())
                .ForMember(v => v.MasterVersion, opt => opt.Ignore())
                .ForMember(v => v.Builder, opt => opt.Ignore())
                .ForMember(v => v.Educationer, opt => opt.Ignore())
                .ForMember(v => v.EducationOrigin,
                    opt => opt.MapFrom((src, dest) =>
                        src.EducationToolOriginId == null
                            ? EducationOriginConstants.DefaultEducationOrigin
                            : src.EducationToolOrigin?.Name));
        }

        protected override IMappingExpression<PlanModel, Plan> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(v => v.Project, opt => opt.Ignore());
        }
    }
    public class PlanModel2Profile : BaseProfile<PlanModel2, Plan, int>
    {
        //protected override IMappingExpression<Plan, PlanModel2> MapEntityToModel()
        //{
        //    return base.MapEntityToModel()
        //        .ForMember(v => v.Project, opt => opt.Ignore())
        //        .ForMember(v => v.MasterVersion, opt => opt.Ignore())
        //        .ForMember(v => v.Builder, opt => opt.Ignore())
        //        .ForMember(v => v.Educationer, opt => opt.Ignore());
        //}

        //protected override IMappingExpression<PlanModel2, Plan> MapModelToEntity()
        //{
        //    return base.MapModelToEntity().ForMember(v => v.Project, opt => opt.Ignore());
        //}
    }
}
