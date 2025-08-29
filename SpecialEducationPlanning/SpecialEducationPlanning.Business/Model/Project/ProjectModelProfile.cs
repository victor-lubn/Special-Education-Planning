using AutoMapper;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model
{
    public class ProjectModelProfile : BaseProfile<ProjectModel, Domain.Entity.Project, int>
    {
        protected override IMappingExpression<Domain.Entity.Project, ProjectModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(p => p.Aiep, opt => opt.Ignore());
        }
    }
}
