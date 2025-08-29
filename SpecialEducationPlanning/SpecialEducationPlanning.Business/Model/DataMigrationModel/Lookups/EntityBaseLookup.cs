using Koa.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups
{
    public abstract class EntityBaseLookup<E, PK, SK> where E : class, IEntity<PK>
    {

        protected ILookup<PK, E> primarykeyLookup;

        protected ILookup<SK, E> secondaryKeyLookup;

        public EntityBaseLookup(IEnumerable<E> entities)
        {
            primarykeyLookup = BuildPrimaryKeyLookup(entities);
            secondaryKeyLookup = BuildSecondaryKeyLookup(entities);
        }

        protected virtual ILookup<PK, E> BuildPrimaryKeyLookup(IEnumerable<E> entities)
        {
            return entities.ToLookup(x => x.Id, x => x);
        }

        protected virtual ILookup<SK, E> BuildSecondaryKeyLookup(IEnumerable<E> entities)
        {
            return entities.ToLookup(x => GetSecondaryKey(x));
        }

        public abstract SK GetSecondaryKey(E entity);

        public E GetByPrimaryKey(PK id)
        {
            if (primarykeyLookup.Contains(id))
            {
                return primarykeyLookup[id].First();
            }
            else
            {
                return null;
            }
        }

        public E GetBySecondaryKey(SK id)
        {
            if (secondaryKeyLookup.Contains(id))
            {
                return secondaryKeyLookup[id].First();
            }
            else
            {
                return null;
            }
        }
    }
}
