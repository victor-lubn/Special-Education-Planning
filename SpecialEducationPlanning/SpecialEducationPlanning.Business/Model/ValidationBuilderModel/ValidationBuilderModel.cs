using System.Collections.Generic;
using System.Collections.ObjectModel;

using SpecialEducationPlanning
.Business.DtoModel.BuilderSapSearch;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model
{
    public class ValidationBuilderModel
    {
        public BuilderMatchType Type { get; set; } = BuilderMatchType.None;
        public ICollection<BuilderSapSearch> Builders { get; set; } = new Collection<BuilderSapSearch>();
    }
}