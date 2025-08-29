using Koa.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionInfoModel : BaseModel<int>
    {
        /// <summary>
        /// 
        /// </summary>
        [StringLength(DataContext.LongPropertyLength)]
        public string VersionNotes { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string QuoteOrderNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Range { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CatalogCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<RomItemModel> RomItems { get; set; } = new Collection<RomItemModel>();
        /// <summary>
        /// Property filled by the frontend to know if the version is coming from the offline sync or not
        /// </summary>
        public bool IsSync { get; set; } = false;
    }
}
