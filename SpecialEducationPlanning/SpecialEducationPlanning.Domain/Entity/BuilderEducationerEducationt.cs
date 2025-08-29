using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class BuilderEducationerAiep : BaseEntity<int>, ISearchable<int>
    {
        public int BuilderId { get; set; }
        public virtual Builder Builder { get; set; }
        public int AiepId { get; set; }
        public virtual Aiep Aiep { get; set; }
    }
}


