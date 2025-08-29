using Koa.Persistence.EntityRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class AreaCore
    {
        public static async Task<Area> SetAiepAreas(this IEntityRepository<int> entityRepository, Area area, IEnumerable<int> AiepIds, ILogger logger)
        {
            logger.LogDebug("AreaCore called SetAiepAreas");

            foreach (var AiepId in AiepIds)
            {
                var Aiep = await entityRepository.FindOneAsync<Aiep>(AiepId);

                if (Aiep.IsNull())
                {
                    logger.LogError(ErrorCode.EntityNotFound.GetDescription() + "Error with id: " + AiepId.ToString());

                    logger.LogDebug("AreaCore end call SetAiepAreas -> return Null");

                    return null;
                }

                Aiep.Area = area;
            }

            logger.LogDebug("AreaCore end call SetAiepAreas -> return Area");

            return area;
        }

        public static async Task<Area> CreateOrUpdateArea(this IEntityRepository<int> entityRepository, AreaDtoModel areaDtoModel, IObjectMapper mapper, ILogger logger)
        {
            logger.LogDebug("AreaCore called CreateOrUpdateArea");
            Area area = await entityRepository.FindOneAsync<Area>(areaDtoModel.Id);
            area = mapper.Map(areaDtoModel, area);   
         
        
            foreach (var Aiep in areaDtoModel.AiepIds)
            {
                area.Aieps.Add(await entityRepository.FindOneAsync<Aiep>(Aiep));
            }

            if (areaDtoModel.Id == 0)
            {
                var newArea = entityRepository.Add(area);

                logger.LogDebug("AreaCore end call CreateOrUpdateArea -> return Area");

                return newArea;
            }

            logger.LogDebug("AreaCore end call CreateOrUpdateArea -> return Area");

            return area;
        }
    }



}

