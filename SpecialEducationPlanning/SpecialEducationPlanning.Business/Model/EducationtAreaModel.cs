using Koa.Domain;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Business.Model
{
    public class AiepAreaModel : BaseModel<int>
    {

        public IEnumerable<AiepModel> AiepsAssigned { get; set; }

        public IEnumerable<AiepModel> AiepsAvailables { get; set; }

    }
}

