using Koa.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{

    public class UserModel : BaseModel<int>, IAuditableEntity
    {

        #region Properties 

        public AiepModel CurrentAiep { get; set; }

        public int? CurrentAiepId { get; set; }

        public AiepModel Aiep { get; set; }

        public int? AiepId { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        // [CodeInjectionReject]
        public string FirstName { get; set; }

        public bool FullAclAccess { get; set; }

        public virtual ReleaseInfoModel ReleaseInfo { get; set; }

        public int? ReleaseInfoId { get; set; }

        [StringLength(DataContext.DefaultPropertyLength)]
        // [CodeInjectionReject]
        public string Surname { get; set; }

        [Required]
        public string UniqueIdentifier { get; set; }

        public int? DelegateToUserId { get; set; }

        public bool Leaver { get; set; }

        
        #endregion

        #region Properties IAuditableEntity

        public DateTime CreatedDate { get; set; }

        [StringLength(DataContext.ShortPropertyLength)]
        // [CodeInjectionReject]
        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        [StringLength(DataContext.ShortPropertyLength)]
        // [CodeInjectionReject]
        public string UpdateUser { get; set; }
       
        #endregion

    }

}
