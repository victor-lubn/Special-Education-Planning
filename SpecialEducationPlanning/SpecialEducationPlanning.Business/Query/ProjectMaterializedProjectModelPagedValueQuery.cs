using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Query
{
    public class ProjectMaterializedProjectModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Project, ProjectModel>
    {
        public ProjectMaterializedProjectModelPagedValueQuery(ISpecification<Project> specification, ISpecification<ProjectModel> builders,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, builders, sortDescriptor, pageSize)
        {
        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<ProjectModel> Materialize(IQueryable<Project> queryable)
        {
            var materialize = queryable.Select(c => new ProjectModel
            {
                Id = c.Id,
                CodeProject = c.CodeProject,
                AiepId = c.AiepId,
                KeyName = c.KeyName,
                SinglePlanProject = c.SinglePlanProject,
                CreatedDate = c.CreatedDate,
                CreationUser = c.CreationUser,
                UpdatedDate = c.UpdatedDate,
                UpdateUser = c.UpdateUser,
                IsArchived = c.IsArchived,
                BuilderId = c.BuilderId,
                Builder = c.BuilderId != null ? new BuilderModel
                {
                    Id = c.Builder.Id,
                    TradingName = c.BuilderId != null ? c.Builder.TradingName : string.Empty
                } : null,
                Aiep = new AiepModel
                {
                    AiepCode = c.Aiep.AiepCode,
                    Name = c.Aiep.Name,
                    Email = c.Aiep.Email,
                    Address1 = c.Aiep.Address1,
                    Postcode = c.Aiep.Postcode,
                    PhoneNumber = c.Aiep.PhoneNumber,
                    FaxNumber = c.Aiep.FaxNumber,
                },
                ProjectTemplates = c.ProjectTemplates.Select(p => new ProjectTemplatesModel
                {
                    Id = p.Id,
                    PlanId = p.PlanId,
                    ProjectId = p.ProjectId
                }).ToList(),
                HousingSpecifications = c.HousingSpecifications.Select(h => new HousingSpecificationModel
                {
                    Id = h.Id,
                    ProjectId = h.ProjectId,
                    Code = h.Code,
                    Name = h.Name,
                    PlanState = h.PlanState,
                    HousingTypes = h.HousingTypes.Select(t => new HousingTypeModel
                    {
                        Id = t.Id,
                        HousingSpecificationId = t.HousingSpecificationId,
                        Code = t.Code,
                        Name = t.Name,
                    }).ToList(),
                    HousingSpecificationTemplates = h.HousingSpecificationTemplates.Select(t => new HousingSpecificationTemplatesModel
                    {
                        Id = t.Id,
                        HousingSpecificationId = t.HousingSpecificationId,
                        PlanId = t.PlanId
                    }).ToList(),
                }).ToList(),
            });
            return materialize;
        }
    }
}

