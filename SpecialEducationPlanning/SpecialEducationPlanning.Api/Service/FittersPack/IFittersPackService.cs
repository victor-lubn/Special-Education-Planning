using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Service.FittersPack
{
    public interface IFittersPackService
    {
        Task GenerateFitterPackAsync(int versionId, int EducationTool3DCVersionId, FitterPackProcessType processType = FitterPackProcessType.Generate);
    }
}

