using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IHouseSpecificationRepository
    {
        Task<List<HousingSpecification>> GetHousingSpecificationsByCodeAsync(List<string> housingCode);
        Task<List<HousingSpecification>> GetHousingSpecificationsByProjectIdAsync(int projectId);
        Task<List<HousingSpecification>> GetHouseSpecsByProjectIdAsync(int projectId);
        Task<List<HousingSpecification>> GetHouseSpecsByProjectAndPlanIdAsync(int projectId, int planId);
    }
}
