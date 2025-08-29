using Koa.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class CommentModel : BaseModel<int>
    {
        [StringLength(DataContext.DefaultPropertyLength)]
        //[CodeInjectionReject]
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        //[CodeInjectionReject]
        public string User { get; set; }
        //[CodeInjectionReject]
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        //[CodeInjectionReject]
        public string CreationUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        //[CodeInjectionReject]
        public string UpdateUser { get; set; }
    }
}