using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository.HouseTypeRepository
{
    public interface IHouseTypeRepository
    {
        Task<List<HousingType>> GetAllHousingTypeAndPlansAsync(ILogger logger);
        Task<HousingType> GetHousingTypeAsync(int housingTypeId, ILogger logger);
        Task<bool> UpdateHousingTypeAsync(int housingTypeId, int planId, DbContext context, ILogger logger);
    }
}
