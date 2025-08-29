using Koa.Domain;
using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Domain.Model.AzureSearchModel
{

    public class SearchBaseIndexModel : BaseModel<string>
    {
        #region Properties 

        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable] public virtual new string Id { get; set; }

        [IsFilterable] [IsSortable] public DateTime? UpdatedDate { get; set; }
        [IsFilterable] public IEnumerable<int> Acls { get; set; } = new List<int>();
        [IsFilterable] public IEnumerable<int> AiepIds { get; set; } = new List<int>();

        #endregion

    }

}
