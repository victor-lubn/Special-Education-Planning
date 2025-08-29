using System;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Extensions
{
    public static class EducationOriginTypeExtensions
    {
        public static bool Is3Dc(this string EducationOriginType)
        {
            return string.Equals(EducationOriginType, EducationOriginType.ThreeDc.GetDescription(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

