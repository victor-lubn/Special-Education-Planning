using Koa.Domain;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class PermissionModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Name { get; set; }

        public string DescriptionCode { get; set; }

    }
}
