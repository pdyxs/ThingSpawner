using System.Collections;
using System.Collections.Generic;

namespace PDYXS.ThingSpawner
{
    public interface IInitialisable<T>
    {
        T entity
        {
            get;
        }

        void Initialise(T obj);
    }
}