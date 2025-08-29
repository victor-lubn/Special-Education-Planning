
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Service.Sap;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly IEfUnitOfWork unitOfWork;
        private readonly ILogger<ProjectService> logger;
        private readonly IBuilderRepository builderRepository;
        private readonly IObjectMapper mapper;
        private readonly ISapService sapService;
        private readonly IProjectRepository projectRepo;
        private readonly IAiepRepository AiepRepo;
        private readonly IHouseSpecificationRepository housingSpecsRepo;

        public ProjectService(IEfUnitOfWork unitOfWork,
            ILogger<ProjectService> logger,
            IBuilderRepository builderRepository,
            IObjectMapper mapper,
            ISapService sapService,
            IProjectRepository projectRepo,
            IAiepRepository AiepRepo,
            IHouseSpecificationRepository housingSpecsRepo)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.builderRepository = builderRepository;
            this.mapper = mapper;
            this.sapService = sapService;
            this.projectRepo = projectRepo;
            this.AiepRepo = AiepRepo;
            this.housingSpecsRepo = housingSpecsRepo;
        }

        public async Task<RepositoryResponse<string>> CreateUpdateProjectForCreatio(CreatioProjectDto creatio)
        {
            logger.LogDebug("CreatioService called CreateUpdateProjectForCretio()");

            unitOfWork.BeginTransaction();

            try
            {
                var builderResponse = await GetCreateBuilderAsync(creatio.Builder);
                if (builderResponse.ErrorList.Count > 0)
                {
                    unitOfWork.Rollback();
                    return new RepositoryResponse<string>(builderResponse.ErrorList);
                }
                var builder = builderResponse.Content;

                var Aiep = await AiepRepo.GetAiepByCode(creatio.AiepCode, true);
                if (Aiep.ErrorList.Count > 0)
                {
                    unitOfWork.Rollback();
                    return new RepositoryResponse<string>("Aiep does not exist");
                }
                var AiepModel = Aiep.Content;
                var AiepDB = mapper.Map<AiepModel, Aiep>(AiepModel);

                var projectResponse = await projectRepo.GetProjectByProjectKeyNameAsync(creatio.KeyName);
                if (projectResponse.ErrorList.Count > 0)
                {
                    unitOfWork.Rollback();
                    return new RepositoryResponse<string>(projectResponse.ErrorList);
                }

                if (projectResponse.ErrorList.Count == 0 && projectResponse.Content.IsNotNull())
                {
                    //update project found + associated tables with values from payload

                    var projectDB = projectResponse.Content;

                    var housingSpecsForDB = await UpdateHousingSpecifications(creatio, projectDB.Id);

                    projectDB.CodeProject = creatio.CodeProject;
                    projectDB.KeyName = creatio.KeyName;
                    projectDB.SinglePlanProject = false;
                    projectDB.Aiep = AiepDB;
                    projectDB.Builder = builder;
                    projectDB.HousingSpecifications = housingSpecsForDB;

                    projectRepo.Update(projectDB);  //in SaveChangesOverride() via Update(), a transaction commit exists

                    logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response for existing ProjectModel");

                    return (new RepositoryResponse<string>(projectDB.KeyName, new List<string>()));
                }

                // insert values for new project

                var housingSpecificationForDB = CreateNewHousingSpecifications(creatio.HousingSpecifications);

                var newProject = new Project()
                {
                    CodeProject = creatio.CodeProject,
                    KeyName = creatio.KeyName,
                    SinglePlanProject = false,
                    Aiep = AiepDB,
                    Builder = builder,
                    HousingSpecifications = housingSpecificationForDB,
                    IsArchived = false,
                };
                projectRepo.Update(newProject);     //in SaveChangesOverride() via Update(), a transaction commit exists
                
                logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response for new ProjectModel");

                return new RepositoryResponse<string>(newProject.KeyName, new List<string>());
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }                     
        }



        #region Methods Private

        private List<HousingSpecification> CreateNewHousingSpecifications(ICollection<HousingSpecificationDto> housingSpecsFromAPI)
        {
            var housingSpecificationForDB = new List<HousingSpecification>();

            var housingSpecs = housingSpecsFromAPI.Select(x =>
            {
                return new HousingSpecification()
                {
                    Id = 0,
                    Code = x.Code,
                    Name = x.Name,
                    PlanState = 0,
                    HousingTypes = x.HousingTypes.Select(y => new HousingType()
                    {
                        Id = 0,
                        Code = y.Code,
                        Name = y.Name,
                    }).ToList(),
                };
            }).ToList();
            return housingSpecs;
        }

        private async Task<List<HousingSpecification>> UpdateHousingSpecifications(CreatioProjectDto creatio, int projectId)
        {
            var housingSpecificationForDB = new List<HousingSpecification>();

            var housingSpecsFromAPI = creatio.HousingSpecifications;
            var housingCodes = housingSpecsFromAPI.Select(x => x.Code).ToList();

            var housingSpecsInDB = await housingSpecsRepo.GetHousingSpecificationsByProjectIdAsync(projectId);
            var codesInDb = housingSpecsInDB.Select(x => x.Code).ToList();

            foreach (var spec in housingSpecsInDB)
            {
                HousingSpecificationDto housingSpecDto = housingSpecsFromAPI.FirstOrDefault(x => x.Code == spec.Code);
                if (housingSpecDto.IsNull())
                {
                    continue;
                }
                spec.HousingTypes.Clear();
                
                var newHouseTypes = housingSpecDto.HousingTypes.Select(x => new HousingType()
                {
                    Id = 0,
                    Name = x.Name,
                    Code = x.Code,
                });
                spec.HousingTypes.AddRange(newHouseTypes);
            }

            var housingSpecsNotInDB = housingSpecsFromAPI.Select(x =>
            {
                return new HousingSpecification()
                {
                    Id = 0,
                    Code = x.Code,
                    Name = x.Name,
                    PlanState = 0,
                    HousingTypes = x.HousingTypes.Select(y => new HousingType()
                    {
                        Id = 0,
                        Code = y.Code,
                        Name = y.Name,
                    }).ToList(),
                };
            }).Where(y => !codesInDb.Contains(y.Code)).ToList();

            housingSpecificationForDB.AddRange(housingSpecsInDB);
            housingSpecificationForDB.AddRange(housingSpecsNotInDB);

            return housingSpecificationForDB;
        }

        private async Task<RepositoryResponse<Builder>> GetCreateBuilderAsync(BuilderDto builder)
        {
            Builder builderDB;
            var builderModel = mapper.Map<BuilderDto, BuilderModel>(builder);

            if (string.IsNullOrEmpty(builder.AccountNumber))
            {
                if (string.IsNullOrEmpty(builder.TradingName)
                    || string.IsNullOrEmpty(builder.Address1)
                    || string.IsNullOrEmpty(builder.PostCode))
                {
                    return new RepositoryResponse<Builder>(null, new List<string> { "Builder AccountNumber is required for SAP account or Builder TradingName, Address1 and PostCode are required for non-SAP account" });
                }
                else
                {
                    //create Cash Account Builder
                    var builderEntity = mapper.Map<BuilderModel, Builder>(builderModel);
                    builderDB = await builderRepository.AddBuilder(builderEntity);
                }
            }
            else
            {
                builderDB = await builderRepository.GetExistingDbBuilderAsync(builderModel);

                if (builderDB == null)
                {
                    var sapBuilderResponse = await sapService.GetPossibleSapBuilderByAccountNumber(builder.AccountNumber);
                    var sapContent = sapBuilderResponse.Content;

                    if (sapBuilderResponse.ErrorList.Count == 0 &&
                        sapContent.Builders.IsNotNull() &&
                        sapContent.Type == BuilderMatchType.Exact) //add SapBuilder to Builder table
                    {
                        var sapBuilder = sapContent.Builders.FirstOrDefault();
                        var builderSap = mapper.Map<BuilderModel, Builder>(sapBuilder.Builder);
                        builderDB = await builderRepository.AddBuilder(builderSap);
                    }
                    else
                    {
                        return new RepositoryResponse<Builder>(null, new List<string> { "SAP account doesn't exist" });
                    }
                }
            }

            return new RepositoryResponse<Builder>(builderDB);
        }

        #endregion
    }
}

