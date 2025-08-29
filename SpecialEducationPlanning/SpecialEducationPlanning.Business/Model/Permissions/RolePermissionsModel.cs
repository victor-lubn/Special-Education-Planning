using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class RolePermissionModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Name { get; set; }
        public IEnumerable<int> Permissions { get; set; } = new Collection<int>();
    }
}
