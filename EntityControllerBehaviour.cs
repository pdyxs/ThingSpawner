using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDYXS.ThingSpawner
{
    public abstract class EntityControllerBehaviour<T> : MonoBehaviour, IInitialisable<T>
        where T : class
    {

        public T entity
        {
            get; private set;
        }

        public virtual void Initialise(T obj)
        {
            entity = obj;
        }
    }
}