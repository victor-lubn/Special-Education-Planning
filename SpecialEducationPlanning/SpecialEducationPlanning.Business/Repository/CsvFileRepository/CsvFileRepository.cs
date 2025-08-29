using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.UserSpecifications;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class CsvFileRepository : ICsvFileRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly IEfUnitOfWork unitOfWork;
        private readonly IObjectMapper mapper;
        private readonly ILogger<CsvFileRepository> logger;

        public CsvFileRepository(IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork,
            IObjectMapper mapper, ILogger<CsvFileRepository> logger)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        #region Public Methods
        /// <summary>
        /// Method to insert users into DB
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<int>> InsertUsers(IEnumerable<UserCsvModel> records)
        {
            logger.LogDebug("CsvFileRepository called InsertUsers");

            var repositoryResponse = new RepositoryResponse<int>();
            int userCount = 0;
            unitOfWork.BeginTransaction();
            List<string> usersAdded = new List<string>();
            foreach (UserCsvModel user in records)
            {
                if (await CheckUserIdentifierExists(user.userprincipalName, usersAdded))
                {
                    logger.LogError("User already exists");
                    repositoryResponse.AddError(ErrorCode.EntityAlreadyExist, user.userprincipalName);
                }
                else
                {
                    if (user.departmentNumber.IsNull())
                    {
                        logger.LogError("Aiep is null");
                        repositoryResponse.AddError(ErrorCode.UndefinedAiep, "Aiep is null");
                    }
                    var Aiep = await entityRepositoryKey.GetAiepByAiepCodeIgnoreFilter(user.departmentNumber, logger);
                    UserModel newUser = FillUser(user, Aiep);
                    Role role = await SetRole(user);
                    User userEntity = await entityRepositoryKey.CreateUpdateUserAsync(newUser, role, mapper, logger);
                    entityRepositoryKey.Add(userEntity);
                    usersAdded.Add(newUser.UniqueIdentifier);
                    repositoryResponse.Content = ++userCount;
                }
            }
            if (repositoryResponse.ErrorList.Count == 0)
                unitOfWork.Commit();
            else
                unitOfWork.Rollback();

            logger.LogDebug("CsvFileRepository end call InsertUsers -> return Repository response Int");

            return repositoryResponse;
        }

        /// <summary>
        /// Method to insert Aieps into DB
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<RepositoryResponse<int>> InsertAieps(IEnumerable<AiepCsvModel> records)
        {
            logger.LogDebug("CsvFileRepository called InsertAieps");

            var repositoryResponse = new RepositoryResponse<int>();
            int AiepCount = 0;
            List<string> AiepsAdded = new List<string>();
            unitOfWork.BeginTransaction();
            foreach (var Aiep in records)
            {
                if (await CheckAiepCodeExists(Aiep.AiepCode, AiepsAdded))
                {
                    logger.LogError("Aiep already exists");
                    repositoryResponse.AddError(ErrorCode.EntityAlreadyExist, Aiep.AiepCode);
                }
                else
                {
                    int AreaId = await CheckAiepsArea(Aiep.Area);
                    if (AreaId == -1)
                    {
                        logger.LogError("Area not found");
                        repositoryResponse.AddError(ErrorCode.EntityNotFound, "Area not found");
                    }
                    var AiepEntity = await FillAiep(Aiep, AreaId);
                    if (AiepEntity.DownloadLimit < 0 || AiepEntity.DownloadSpeed < 0)
                    {
                        logger.LogError("DownloadLimit or DownloadSpeed Error");
                        repositoryResponse.AddError(ErrorCode.ArgumentErrorBusiness, "DownloadLimit or DownloadSpeed Error");
                    }
                    else
                    {
                        var newAiep = await entityRepositoryKey.CreateOrUpdateAiep(AiepEntity, mapper, logger);
                        entityRepositoryKey.Add(newAiep);
                        AiepsAdded.Add(newAiep.AiepCode);
                        repositoryResponse.Content = ++AiepCount;
                    }
                }
            }
            if (repositoryResponse.ErrorList.Count == 0)
                unitOfWork.Commit();
            else
                unitOfWork.Rollback();

            logger.LogDebug("CsvFileRepository end call InsertAieps -> return Repository response Int");

            return repositoryResponse;
        }

        #endregion

        #region Private Methods

        private async Task<bool> CheckUserIdentifierExists(string uniqueIdentifier, List<string> usersAdded)
        {
            logger.LogDebug("CsvFileRepository called CheckUserIdentifierExists");

            var user = await entityRepositoryKey.Where(new UserByUniqueIdentifierSpecification(uniqueIdentifier)).FirstOrDefaultAsync();
            if (user.IsNull())
                if (!usersAdded.Contains(uniqueIdentifier))
                {
                    logger.LogDebug("CsvFileRepository end call CheckUserIdentifierExists -> return False");

                    return false;
                }

            logger.LogDebug("CsvFileRepository end call CheckUserIdentifierExists -> return True");

            return true;
        }

        private UserModel FillUser(UserCsvModel user, Aiep Aiep)
        {
            logger.LogDebug("CsvFileRepository called FillUser");

            UserModel newUser = new UserModel();
            newUser.CreatedDate = DateTime.UtcNow;
            newUser.CreationUser = "manualUserCreate";
            newUser.UpdatedDate = DateTime.UtcNow;
            newUser.UpdateUser = "manualUserUpdate";
            newUser.AiepId = Aiep?.Id;          
            newUser.FirstName = user.GivenName;
            newUser.Surname = user.sn;
            newUser.UniqueIdentifier = user.userprincipalName;           

            logger.LogDebug("CsvFileRepository end call FillUser");

            return newUser;
        }

        private async Task<AiepModel> FillAiep(AiepCsvModel Aiep, int AreaId)
        {
            logger.LogDebug("CsvFileRepository called FillAiep");

            AiepModel newAiep = new AiepModel();
            newAiep.AiepCode = Aiep.AiepCode;
            newAiep.AreaId = AreaId;
            newAiep.Name = Aiep.Name;
            newAiep.Email = Aiep.Email;
            newAiep.Address1 = Aiep.Address1;
            newAiep.Address2 = Aiep.Address2;
            newAiep.Address3 = Aiep.Address3;
            newAiep.Postcode = Aiep.Postcode;
            newAiep.PhoneNumber = Aiep.PhoneNumber;
            newAiep.FaxNumber = Aiep.FaxNumber;
            newAiep.IpAddress = Aiep.IpAddress;
            newAiep.MediaServer = Aiep.MediaServer;
            newAiep.HtmlEmail = Aiep.HtmlEmail;

            if (int.TryParse(Aiep.DownloadSpeed, out int DSpeed))
            {
                newAiep.DownloadSpeed = DSpeed;
            }
            else
            {
                newAiep.DownloadSpeed = -1;
            }
            if (int.TryParse(Aiep.DownloadLimit, out int DLimit))
            {
                newAiep.DownloadLimit = DLimit;
            }
            else
            {
                newAiep.DownloadLimit = -1;
            }

            newAiep.ManagerId = await SetAiepManager(Aiep.Manager);
            if (newAiep.ManagerId < 0)
            {
                newAiep.ManagerId = null;
            }

            logger.LogDebug("CsvFileRepository end call FillAiep");

            return newAiep;
        }

        private async Task<int> SetAiepManager(string manager)
        {
            logger.LogDebug("CsvFileRepository called SetAiepManager");

            var managerEntity = await entityRepositoryKey.Where(new UserByUniqueIdentifierSpecification(manager)).FirstOrDefaultAsync();
            if (managerEntity.IsNotNull())
            {
                logger.LogDebug("CsvFileRepository end call SetAiepManager -> return Id");

                return managerEntity.Id;
            }
            logger.LogDebug("CsvFileRepository end call SetAiepManager -> return -1");

            return -1;
        }

        private async Task<Role> SetRole(UserCsvModel user)
        {
            logger.LogDebug("CsvFileRepository called SetRole");

            var roles = await entityRepositoryKey.GetAll<Role>().ToListAsync();
            bool found = false;
            Role newRole = new Role();
            foreach (Role role in roles)
            {
                if (user.role.Equals(role.Name))
                {
                    found = true;
                    newRole = role;
                    break;
                }
            }
            if (!found)
            {
                user.role = "Educationer";
                newRole = entityRepositoryKey.GetRoleByName(user.role, logger).Result;
            }

            logger.LogDebug("CsvFileRepository end call SetRole -> return Role");

            return newRole;
        }

        private async Task<bool> CheckAiepCodeExists(string AiepCode, List<string> AiepsAdded)
        {
            logger.LogDebug("CsvFileRepository called CheckAiepCodeExists");

            var Aiep = await entityRepositoryKey.GetAiepByAiepCodeIgnoreFilter(AiepCode, logger);
            if (Aiep.IsNull())
                if (!AiepsAdded.Contains(AiepCode))
                {
                    logger.LogDebug("CsvFileRepository end call CheckAiepCodeExists -> return False");

                    return false;
                }

            logger.LogDebug("CsvFileRepository end call CheckAiepCodeExists -> return True");

            return true;
        }

        private async Task<int> CheckAiepsArea(string AreaName)
        {
            logger.LogDebug("CsvFileRepository called CheckAiepCodeExists");

            string AreaSerch = AreaName.Trim().ToUpper().Replace(" ", String.Empty);

            var Area = await entityRepositoryKey.Where<Area>(new Specification<Area>(x => x.KeyName.Trim().ToUpper().Replace(" ", String.Empty) == AreaSerch)).FirstOrDefaultAsync();
            if (Area.IsNotNull())
            {
                logger.LogDebug("CsvFileRepository end call CheckAiepCodeExists -> return Id");

                return Area.Id;
            }

            logger.LogDebug("CsvFileRepository end call CheckAiepCodeExists -> return -1");

            return -1;
        }

        #endregion
    }
}


