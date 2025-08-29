
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.DtoModel.BuilderSapSearch
{
    public class BuilderSapSearch
    {

        public BuilderSearchTypeEnum BuilderSearchType { get; set; }

        public BuilderModel Builder { get; set; }
    }
}
