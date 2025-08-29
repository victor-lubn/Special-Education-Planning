using Koa.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class ProjectModelContractHub : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public string CodeProject { get; set; }
        public int AiepId { get; set; }
        public AiepModelContractHub Aiep { get; set; }
        public int? BuilderId { get; set; }
        public BuilderModel Builder { get; set; }
        public bool? IsArchived { get; set; }
        public virtual ICollection<PlanModel> Plans { get; set; }
        public virtual ICollection<HousingSpecificationModel> HousingSpecifications { get; set; }
        public virtual ICollection<ProjectTemplatesModel> ProjectTemplates { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        public string KeyName { get; set; }
        public bool SinglePlanProject { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
