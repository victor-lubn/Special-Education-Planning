using System;

namespace SpecialEducationPlanning
.Api.Service.FittersPack
{
    [Serializable]
    public class FittersPackContext
    {
        public int VersionId { get; set; }
        public int EducationTool3DCVersionId { get; set; }

        public FitterPackProcessType ProcessType { get; set; }
        public FittersPackContext(int versionId, int EducationTool3DCVersionId, FitterPackProcessType processType)
        {
            VersionId = versionId;
            EducationTool3DCVersionId = EducationTool3DCVersionId;
            ProcessType = processType;
        }
    }
}

