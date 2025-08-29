using System.Collections.Generic;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups
{
    /// <summary>
    /// 
    /// </summary>
    public class RomItemLookup : EntityBaseLookup<RomItem, int, int>
    {
        /// <summary>
        /// Createas a new instance of <see cref="PlanItemMigrationService"/>
        /// </summary>
        /// <param name="entities">Entitites to migrate</param>
        public RomItemLookup(IEnumerable<RomItem> entities) : base(entities)
        {
        }

        /// <summary>
        /// There is no secondary key for ROM items... using primary key
        /// </summary>
        /// <param name="entity">Rom item</param>
        /// <returns>Id</returns>
        public override int GetSecondaryKey(RomItem entity)
        {
            return entity.GetHashCode();
        }
    }
}
