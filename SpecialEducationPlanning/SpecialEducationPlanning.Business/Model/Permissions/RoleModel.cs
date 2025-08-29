using Koa.Domain;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class RoleModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Name { get; set; }
    }
}
