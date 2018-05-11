using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEntityProvider<out T>
{
    T entity { get; }
}

namespace PDYXS.ThingSpawner
{
    public abstract class EntityControllerBehaviour<T> : 
        EntityControllerBehaviour, 
        IEntityInitialisable<T>, 
        IEntityProvider<T>,
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
            foreach (var c in GetComponents<IInitialisable<T>>())
            {
                if (!(ReferenceEquals(c, this) || c is EntityControllerBehaviour))
                {
                    c.Initialise(entity);
                }
            }
            foreach (var c in GetComponents<ILifecycle>())
            {
                c.Initialise();
            }
            HasSpawned = true;
            OnInitialised.Invoke();
        }

        protected virtual void doInitialise() {}

        public void Despawn()
        {
            var components = GetComponents<ILifecycle>();
            foreach (var c in components)
            {
                c.Teardown();
            }

            if (components.Length == 0)
            {
                this.Recycle();
            }
        }

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