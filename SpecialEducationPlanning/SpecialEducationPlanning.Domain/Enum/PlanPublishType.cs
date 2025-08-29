using System;
using System.Collections.Generic;
using System.Text;

namespace SpecialEducationPlanning
.Domain.Enum
{
    public enum PlanPublishType
    {
        ImageStandardQuality = 1, //Still Images
        ImageHighQuality = 2, // High Quality Images
        ImageAndVideoStandardQuality = 3, // Images and Video
        ImageAndVideoHighQuality = 4, // Images and Video
        CyclesImage = 5 // Cycles Images
    }
}