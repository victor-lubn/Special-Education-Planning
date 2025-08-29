using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class EndUserAiep : BaseEntity<int>
    {

        public int EndUserId { get; set; }
        public EndUser EndUser { get; set; }
        public int AiepId { get; set; }
        public Aiep Aiep { get; set; }
    }
}

