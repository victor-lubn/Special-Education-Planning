using Koa.Domain;
using System;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Comment : BaseEntity<int>, IAuditableEntity
    {
        #region Properties 
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        #endregion

        #region Properties IAuditableEntity
        public DateTime CreatedDate { get; set; }

        public string CreationUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdateUser { get; set; }
        #endregion
    }
}
