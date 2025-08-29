using Koa.Domain;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Business.Report
{
    public class AiepReportModel : BaseModel<int>
    {
        public string AiepCode { get; set; }
        public ICollection<BuilderReportModel> Builders = new List<BuilderReportModel>();
        public ICollection<EducationerReportModel> Educationers = new List<EducationerReportModel>();
        public ICollection<PlanReportModel> Plans = new List<PlanReportModel>();
    }
}


