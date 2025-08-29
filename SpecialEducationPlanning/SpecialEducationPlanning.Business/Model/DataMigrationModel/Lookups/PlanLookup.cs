using System.Collections.Generic;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups
{
    public class PlanLookup : EntityBaseLookup<Plan, int, string>
    {
        /// <summary>
        /// Createas a new instance of <see cref="PlanLookup"/>
        /// </summary>
        /// <param name="entities">Entities to lookup</param>
        public PlanLookup(IEnumerable<Plan> entities) : base(entities)
        {
        }

        /// <summary>
        /// Uses PlanCode to generate the secondary key
        /// </summary>
        /// <param name="entity">Entity to get the secondary key</param>
        /// <returns>Secondary key</returns>
        public override string GetSecondaryKey(Plan entity)
        {
            return entity.PlanCode;
        }
    }
}
