using System.Collections.Generic;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups
{
    /// <summary>
    /// Builder lookup
    /// </summary>
    public class BuilderLookup : EntityBaseLookup<Domain.Entity.Builder, int, string>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BuilderLookup"/>
        /// </summary>
        /// <param name="entities">Entities to lookup</param>
        public BuilderLookup(IEnumerable<Builder> entities) : base(entities)
        {
        }

        /// <summary>
        /// Returns the secondary key for a builder
        /// </summary>
        /// <param name="entity">Builder</param>
        /// <returns>Builder secondary key</returns>
        public override string GetSecondaryKey(Builder entity)
        {
            return string.Join("-", entity.TradingName, entity.Address1, entity.Postcode);
        }
    }
}
