using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Model.FeatureManagementModel
{
    public class LaunchDarklyUpdateRequestModel
    {
        public string environmentKey { get; set; }
        public List<Instruction> instructions { get; set; }
    }

    public class Instruction
    {
        public string kind { get; set; }
    }
}
