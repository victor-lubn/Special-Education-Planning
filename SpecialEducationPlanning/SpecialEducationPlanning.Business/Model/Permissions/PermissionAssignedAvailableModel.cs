using System.Collections.Generic;

namespace SpecialEducationPlanning
.Business.Model
{
    public class PermissionAssignedAvailableModel
    {
        public IEnumerable<PermissionModel> PermissionAssigned { get; set; }
        public IEnumerable<PermissionModel> PermissionsAvailable { get; set; }
    }
}
