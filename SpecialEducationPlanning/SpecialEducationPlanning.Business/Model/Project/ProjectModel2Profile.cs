using AutoMapper;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model
{
    public class ProjectModelContractHubProfile : BaseProfile<ProjectModelContractHub, Domain.Entity.Project, int>
    {
        protected override IMappingExpression<Domain.Entity.Project, ProjectModelContractHub> MapEntityToModel()
        {
            return base.MapEntityToModel();
        }
    }
}