using System.Collections;
using System.Collections.Generic;

namespace PDYXS.ThingSpawner
{
    public interface IInitialisable
    {
    }
    
    public interface IInitialisable<T> : IInitialisable
    {
        void Initialise(T obj);
    }
    
    public interface IEntityInitialisable<T> : IInitialisable<T>
    {
        T entity
        {
            get;
        }

        void Despawn();
    }

    public interface ISpawnTrackable
    {
        bool HasSpawned {
            get;
        }
    }
}