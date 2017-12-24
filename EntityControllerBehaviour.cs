using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDYXS.ThingSpawner
{
    public abstract class EntityControllerBehaviour<T> : MonoBehaviour, IInitialisable<T>, ISpawnTrackable
        where T : class
    {

        public T entity
        {
            get; private set;
        }

        public bool HasSpawned
        {
            get
            {
                return hasSpawned;
            }
        }
        private bool hasSpawned = false;

        public virtual void Initialise(T obj)
        {
            entity = obj;
            hasSpawned = true;
        }
    }
}