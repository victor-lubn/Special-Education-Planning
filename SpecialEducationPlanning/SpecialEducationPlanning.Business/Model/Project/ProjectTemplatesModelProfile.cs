using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model.Project
{
    public class ProjectTemplatesModelProfile : BaseProfile<ProjectTemplatesModel, Domain.Entity.ProjectTemplates, int>
    {
        protected override IMappingExpression<Domain.Entity.ProjectTemplates, ProjectTemplatesModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(v => v.Project, opt => opt.Ignore())
                .ForMember(v => v.Plan, opt => opt.Ignore());
        }
    }
}
