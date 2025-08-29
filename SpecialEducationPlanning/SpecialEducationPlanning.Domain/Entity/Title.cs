using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Title : BaseEntity<int>
    {
        public string TitleName { get; set; }
    }
}
