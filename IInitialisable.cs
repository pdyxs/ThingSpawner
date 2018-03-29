using System.Collections;
using System.Collections.Generic;

namespace PDYXS.ThingSpawner
{
    public interface IInitialisable
    {
    }
    
    public interface IInitialisable<T> : IInitialisable
    {
        T entity
        {
            get;
        }

        void Initialise(T obj);
    }

    public interface ISpawnTrackable
    {
        bool HasSpawned {
            get;
        }
    }
}