using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model
{

    public class BuilderEducationerAiepModel : BaseModel<int>
    {

        public BuilderEducationerAiepModel(BuilderModel builder, AiepModel Aiep)
        {
            BuilderId = builder.Id;
            AiepId = Aiep.Id;
            Builder = builder;
            Aiep = Aiep;
        }

        public BuilderEducationerAiepModel(int builderId, int AiepId)
        {
            BuilderId = builderId;
            AiepId = AiepId;
        }

        public BuilderEducationerAiepModel()
        {
        }

        #region Properties 

        public BuilderModel Builder { get; set; }

        public int BuilderId { get; set; }

        public AiepModel Aiep { get; set; }

        public int AiepId { get; set; }

        #endregion

    }

}

