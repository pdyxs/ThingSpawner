using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PDYXS.ThingSpawner
{
    public abstract class EntityControllerBehaviour<T> : 
        EntityControllerBehaviour, 
        IInitialisable<T>, 
        IPrefabSaveable
        where T : class
    {
        public T entity
        {
            get; private set;
        }

        public void Initialise(T obj)
        {
            entity = obj;
            doInitialise();
            HasSpawned = true;
            OnInitialised.Invoke();
        }

        protected virtual void doInitialise() {}

        [SerializeField]
        private PrefabSaver prefabSaver;
    }

    public abstract class EntityControllerBehaviour : 
        MonoBehaviour,
        ISpawnTrackable
    {
        public UnityEvent OnInitialised = new UnityEvent();
        
        public bool HasSpawned { get; internal set; } = false;
    }
    
    
}