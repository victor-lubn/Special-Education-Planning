using Koa.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Model
{
    public class ActionModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        public ActionType ActionType { get; set; }
        [CodeInjectionReject]
        public string AdditionalInfo { get; set; }

        public DateTime Date { get; set; }

        public int EntityId { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [CodeInjectionReject]
        public string EntityName { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [CodeInjectionReject]
        public string User { get; set; }



    }
}
