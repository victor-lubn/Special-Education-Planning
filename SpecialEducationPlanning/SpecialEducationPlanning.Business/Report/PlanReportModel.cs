using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpecialEducationPlanning
.Business.Report
{
    public class PlanReportModel
    {
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Survey { get; set; }
        public EndUserReportModel EndUser { get; set; }
        public CatalogReportModel Catalog { get; set; } // MasterVersion.Range
        public IEnumerable<RomItemReportModel> RomItems { get; set; } = new Collection<RomItemReportModel>();
        public string Id { get; set; }
        public int? BuilderId { get; set; }
        public int? EducationerId { get; set; }
    }
}

