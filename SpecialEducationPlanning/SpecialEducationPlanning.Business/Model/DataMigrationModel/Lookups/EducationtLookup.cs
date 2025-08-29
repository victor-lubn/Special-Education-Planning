using System.Collections.Generic;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups
{
    /// <summary>
    /// A Aiep lookup
    /// </summary>
    public class AiepLookup : EntityBaseLookup<Aiep, int, string>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AiepLookup"/>
        /// </summary>
        /// <param name="entities">Entities to lookup</param>
        public AiepLookup(IEnumerable<Aiep> entities) : base(entities)
        {
        }

        /// <summary>
        /// Return the entity secondary key
        /// </summary>
        /// <param name="entity">Entity to get the key</param>
        /// <returns>Secondary key</returns>
        public override string GetSecondaryKey(Aiep entity)
        {
            return entity.AiepCode;
        }
    }
}

