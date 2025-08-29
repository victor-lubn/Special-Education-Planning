using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.Query;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Constants;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;



namespace SpecialEducationPlanning
.Business.Query
{
    public class PlanMaterializedPlanModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Plan, PlanModel>
    {
        public PlanMaterializedPlanModelPagedValueQuery(ISpecification<Plan> specification, ISpecification<PlanModel> plans,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, plans, sortDescriptor, pageSize)
        {
        }



        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }



        protected override IQueryable<PlanModel> Materialize(IQueryable<Plan> queryable)
        {
            var materialize = queryable.Select(c => new PlanModel
            {
                Id = c.Id,
                Title = c.Title,
                CreatedDate = c.CreatedDate,
                KeyName = c.KeyName,
                LastOpen = c.LastOpen,
                ProjectId = c.ProjectId,
                EndUserId = c.EndUserId,
                CreationUser = c.CreationUser,
                PlanName = c.PlanName,
                IsStarred = c.IsStarred,
                IsTemplate = c.IsTemplate,
                EndUser = c.EndUserId != null ? new EndUserModel
                {
                    FirstName = c.EndUser.FirstName,
                    Surname = c.EndUser.Surname,
                    FullName = c.EndUser.FullName,
                    Postcode = c.EndUser.Postcode,
                    Address0 = c.EndUser.Address0,
                    Address1 = c.EndUser.Address1,
                    MobileNumber = c.EndUser.MobileNumber
                } : null,
                Survey = c.Survey,
                PlanCode = c.PlanCode,
                PlanState = c.PlanState,
                BuilderId = c.BuilderId,
                EducationerId = c.EducationerId,
                Educationer = c.EducationerId != null ? new UserModel()
                {
                    FirstName = c.Educationer.FirstName,
                    Surname = c.Educationer.Surname,
                    UniqueIdentifier = c.Educationer.UniqueIdentifier
                } : null,
                MasterVersionId = c.MasterVersionId,
                MasterVersion = c.MasterVersionId != null ? new VersionModel()
                {
                    Range = c.MasterVersion.Range,
                    ExternalCode = c.MasterVersion.ExternalCode,
                    KeyName = c.MasterVersion.KeyName,
                    PlanId = c.MasterVersion.PlanId,
                    VersionNumber = c.MasterVersion.VersionNumber,
                    VersionNotes = c.MasterVersion.VersionNotes,
                    CatalogId = c.MasterVersion.CatalogId,
                    Catalog = new CatalogModel()
                    {
                        Name = c.Catalog.Name,
                        Code = c.Catalog.Code,
                        Range = c.Catalog.Range,
                        Enabled = c.Catalog.Enabled
                    },
                    AiepCode = c.MasterVersion.AiepCode,
                    CreatedDate = c.MasterVersion.CreatedDate,
                    CreationUser = c.MasterVersion.CreationUser,
                    UpdatedDate = c.MasterVersion.UpdatedDate,
                    UpdateUser = c.MasterVersion.UpdateUser,
                    RomPath = c.MasterVersion.RomPath,
                    PreviewPath = c.MasterVersion.PreviewPath,
                    QuoteOrderNumber = c.MasterVersion.QuoteOrderNumber,
                    Id = c.MasterVersion.Id
                } : null,
                CatalogId = c.CatalogId,
                BuilderTradingName = c.BuilderTradingName,
                PlanType = c.PlanType,
                UpdatedDate = c.UpdatedDate,
                CadFilePlanId = c.CadFilePlanId,
                Versions = c.Versions.Select(ur => new VersionModel
                {
                    ExternalCode = ur.ExternalCode
                }).ToList(),
                OfflineSyncDate = c.OfflineSyncDate,
                HousingTypeId = c.HousingType != null ? c.HousingType.Id : null,
                HousingTypeModel = c.HousingType != null ? new HousingTypeModel
                {
                    Id = c.HousingType.Id,
                    Code = c.HousingType.Code,
                    Name = c.HousingType.Name,
                    HousingSpecificationId = c.HousingType.HousingSpecificationId,
                    HousingSpecificationModel = new HousingSpecificationModel
                    {
                        Id = c.HousingType.HousingSpecificationId,
                        ProjectId = c.HousingType.HousingSpecification.ProjectId,
                        Code = c.HousingType.HousingSpecification.Code,
                        Name = c.HousingType.HousingSpecification.Name,
                        PlanState = c.HousingType.HousingSpecification.PlanState,
                    },
                } : null,
                HousingSpecificationTemplatesId = c.HousingSpecificationTemplates.Id,
                HousingSpecificationTemplatesModel = c.HousingSpecificationTemplates != null ? new HousingSpecificationTemplatesModel
                {
                    Id = c.HousingSpecificationTemplates.Id,
                    HousingSpecificationId = c.HousingSpecificationTemplates.HousingSpecificationId,
                    HousingSpecificationModel = new HousingSpecificationModel
                    {
                        Id = c.HousingSpecificationTemplates.HousingSpecificationId,
                        ProjectId = c.HousingSpecificationTemplates.HousingSpecification.ProjectId,
                        Code = c.HousingSpecificationTemplates.HousingSpecification.Code,
                        Name = c.HousingSpecificationTemplates.HousingSpecification.Name,
                        PlanState = c.HousingSpecificationTemplates.HousingSpecification.PlanState,
                    },
                    PlanId = c.HousingSpecificationTemplates.PlanId
                } : null,
                ProjectTemplatesId = c.ProjectTemplates.Id,
                ProjectTemplatesModel = c.ProjectTemplates != null ? new ProjectTemplatesModel
                {
                    Id = c.ProjectTemplates.Id,
                    ProjectId = c.ProjectTemplates.ProjectId,
                    PlanId = c.ProjectTemplates.PlanId
                } : null,
                EducationOrigin = c.EducationToolOrigin != null ? c.EducationToolOrigin.Name : EducationOriginConstants.DefaultEducationOrigin
            });
            return materialize;
        }
    }
}

