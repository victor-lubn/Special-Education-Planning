using Koa.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class ReleaseInfoModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        //[CodeInjectionReject]
        public string Title { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        //[CodeInjectionReject]
        public string Subtitle { get; set; }
        public string Version { get; set; }
        public string FusionVersion { get; set; }
        public DateTime DateTime { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        //[CodeInjectionReject]
        public string Document { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        //[CodeInjectionReject]
        public string DocumentPath { get; set; }
    }

}
