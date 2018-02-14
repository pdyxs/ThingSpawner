using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDYXS.ThingSpawner
{
    public abstract class EntityControllerBehaviour<T> : 
        EntityControllerBehaviour, 
        IInitialisable<T>, 
        ISpawnTrackable,
        IPrefabSaveable
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

        [SerializeField]
        private PrefabSaver prefabSaver;
    }

    public abstract class EntityControllerBehaviour : MonoBehaviour {}
}