using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model.Project
{
    public class TenderPackUpdatePlanModel
    {
        public int Id { get; set; }
        public string HousingTypeName { get; set; }
        public string HousingSpecificationName { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public string PlanReference { get; set; }
    }
}
