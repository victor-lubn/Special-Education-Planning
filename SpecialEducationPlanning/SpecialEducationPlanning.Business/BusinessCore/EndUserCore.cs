using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.EndUserSpecifications;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class EndUserCore
    {
        public static async Task<EndUser> GetEndUserByMandatoryFieldsAsync(this IEntityRepository<int> entityRepository,
            EndUser endUser, ILogger logger)
        {
            logger.LogDebug("EndUserCore called GetEndUserByMandatoryFieldsAsync");

            var endUserEntity = await entityRepository.Where(new FindExistingEndUserByMandatoryFieldsSpecification(endUser.Surname, endUser.Postcode, endUser.Address1))
                .IgnoreQueryFilters().FirstOrDefaultAsync();

            if (endUserEntity == null)
            {
                logger.LogDebug("EndUserCore end call GetEndUserByMandatoryFieldsAsync -> return Null");

                return null;
            }

            logger.LogDebug("EndUserCore end call GetEndUserByMandatoryFieldsAsync -> return EndUser");

            return endUserEntity;
        }

        public static async Task<EndUser> GetOrCreateEndUserByMandatoryFieldsAsync(this IEntityRepository<int> entityRepository,
            EndUser endUser, ILogger logger)
        {
            logger.LogDebug("EndUserCore called GetOrCreateEndUserByMandatoryFieldsAsync");

            var endUserEntity = await entityRepository.Where(new FindExistingEndUserByMandatoryFieldsSpecification(endUser.Surname, endUser.Postcode, endUser.Address1))
                .IgnoreQueryFilters().FirstOrDefaultAsync();

            if (endUserEntity == null)
            {
                entityRepository.Add(endUser);
            }

            logger.LogDebug("EndUserCore end call GetOrCreateEndUserByMandatoryFieldsAsync -> return EndUser");

            return endUserEntity;
        }

        public static async Task<Aiep> GetAiepFromEndUserAsync(this IEntityRepository<int> entityRepository,
          int endUserId, int AiepId, ILogger logger)
        {
            logger.LogDebug("EndUserCore called GetAiepFromEndUserAsync");

            var endUserEntity = await entityRepository.Where(new EntityByIdSpecification<EndUser>(endUserId))
                .Include(eu => eu.EndUserAieps).IgnoreQueryFilters().FirstOrDefaultAsync();

            if (endUserEntity == null)
            {
                logger.LogDebug("EndUserCore end call GetAiepFromEndUserAsync -> return Null EndUser");

                return null;
            }

            var AiepEntity = await entityRepository.Where(new EntityByIdSpecification<Aiep>(AiepId))
                .FirstOrDefaultAsync();
            if (AiepEntity == null)
            {
                logger.LogDebug("EndUserCore end call GetAiepFromEndUserAsync -> return Null Aiep");

                return null;
            }

            var ownAiep = endUserEntity.EndUserAieps.Select(endUserAiep => endUserAiep.AiepId == AiepId).FirstOrDefault();

            if (endUserEntity.EndUserAieps.Select(endUserAiep => endUserAiep.AiepId == AiepId).Any())
            {
                logger.LogDebug("EndUserCore end call GetAiepFromEndUserAsync -> return Aiep");

                return AiepEntity;
            }

            logger.LogDebug("EndUserCore end call GetAiepFromEndUserAsync -> return new Aiep");

            return new Aiep() { Id = int.MinValue, AiepCode = "SEARCH" };
        }

        public static async Task<ICollection<EndUserAiep>> GetAllAiepsForEndUser(this IEntityRepository<int> entityRepository,
          int endUserId, ILogger logger)
        {
            logger.LogDebug("EndUserCore called GetAllAiepsForEndUser");

            var endUserEntity = await entityRepository.Where(new EntityByIdSpecification<EndUser>(endUserId))
               .Include(eu => eu.EndUserAieps).IgnoreQueryFilters().FirstOrDefaultAsync();

            if (endUserEntity == null)
            {
                logger.LogDebug("EndUserCore end call GetAllAiepsForEndUser -> return Null");

                return null;
            }

            logger.LogDebug("EndUserCore end call GetAllAiepsForEndUser -> return Collection of EndUserAiep");

            return endUserEntity.EndUserAieps;
        }


        public static async Task<bool> AssignEndUserToAiepAsync(this IEntityRepository<int> entityRepository,
           int endUserId, int AiepId, ILogger logger)
        {
            logger.LogDebug("EndUserCore called AssignEndUserToAiepAsync");

            var endUserEntity = await entityRepository.Where(new EntityByIdSpecification<EndUser>(endUserId))
                .Include(eu => eu.EndUserAieps).IgnoreQueryFilters().FirstOrDefaultAsync();
            if (endUserEntity == null)
            {
                logger.LogDebug("EndUserCore end call AssignEndUserToAiepAsync -> return False");

                return false;
            }

            var AiepEntity = await entityRepository.Where(new EntityByIdSpecification<Aiep>(AiepId))
                .FirstOrDefaultAsync();
            if (AiepEntity == null)
            {
                logger.LogDebug("EndUserCore end call AssignEndUserToAiepAsync -> return False");

                return false;
            }

            if (endUserEntity.EndUserAieps.Any(endUserAiep => endUserAiep.AiepId == AiepId))
            {
                logger.LogDebug("EndUserCore end call AssignEndUserToAiepAsync -> return False");

                return false;
            }

            entityRepository.Add(new EndUserAiep() { EndUserId = endUserId, AiepId = AiepId });

            logger.LogDebug("EndUserCore end call AssignEndUserToAiepAsync -> return True");

            return true;
        }


        public static async Task<Aiep> GetEndUserLatestUserAiepAsync(this IEntityRepository<int> entityRepository,
          int endUserId, ILogger logger)
        {
            logger.LogDebug("EndUserCore called GetEndUserLatestUserAiepAsync");

            var endUserEntity = await entityRepository.Where(new EntityByIdSpecification<EndUser>(endUserId))
                .IgnoreQueryFilters().FirstOrDefaultAsync();
            if (endUserEntity == null)
            {
                logger.LogDebug("EndUserCore end call GetEndUserLatestUserAiepAsync -> return Null");

                return null;
            }

            var planEntity = await entityRepository.GetAll<Plan>().Where(p => p.EndUserId == endUserId)
                .OrderByDescending(p => p.CreatedDate).IgnoreQueryFilters().Include(p => p.Project).ThenInclude(z => z.Aiep).FirstOrDefaultAsync();

            logger.LogDebug("EndUserCore end call GetEndUserLatestUserAiepAsync -> return Aiep");

            return planEntity?.Project?.Aiep;
        }


        public static async Task<Aiep> GetEndUserExistsInAiepAsync(this IEntityRepository<int> entityRepository,
          int endUserId, int AiepId, ILogger logger)
        {
            logger.LogDebug("EndUserCore called GetEndUserExistsInAiepAsync");

            var endUserEntity = await entityRepository.Where(new EntityByIdSpecification<EndUser>(endUserId))
                .IgnoreQueryFilters().Include(eu => eu.EndUserAieps).FirstOrDefaultAsync();
            if (endUserEntity == null)
            {
                logger.LogDebug("EndUserCore end call GetEndUserExistsInAiepAsync -> return Null");

                return null;
            }

            //check if AiepId already exist in list of Aieps for this endUser
            if (!endUserEntity.EndUserAieps.Any(d => d.AiepId == AiepId))
            {
                logger.LogDebug("EndUserCore end call GetEndUserExistsInAiepAsync -> return Null");

                return null;
            }

            var plans = await entityRepository.GetAll<Plan>().Where(p => p.EndUserId == endUserId)
                .Include(p => p.Project).ThenInclude(z => z.Aiep).ToListAsync<Plan>();

            Aiep AiepFound = plans.Where(p => p.Project.Aiep.Id == AiepId).Select(p => p.Project.Aiep).FirstOrDefault<Aiep>();

            logger.LogDebug("EndUserCore end call GetEndUserExistsInAiepAsync -> return Aiep");

            return AiepFound; 
        }
    }
}

