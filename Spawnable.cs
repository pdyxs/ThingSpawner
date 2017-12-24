using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PDYXS.ThingSpawner
{
    public abstract class Spawnable<T> where T : MonoBehaviour
    {
        public T prefab;
        public Transform parent;

        private bool hasPool = false;

        protected virtual T Spawn()
        {
            if (!hasPool)
            {
                ObjectPool.CreatePool(prefab, 1);
                hasPool = true;
            }
            return prefab.Spawn(parent);
        }
    }

    public class Spawnable<T, U> : Spawnable<T>
        where T : MonoBehaviour, IInitialisable<U>
        where U : class
    {
        public T Get
        {
            get
            {
                return _entity;
            }
        }
        private T _entity;

        public void Initialise(U ce)
        {
            Reset();
            if (ce == null)
            {
                return;
            }
            if (_entity == null)
            {
                _entity = Spawn();
            }
            _entity.Initialise(ce);
        }

        protected void Reset()
        {
            if (_entity != null && _entity.isActiveAndEnabled)
            {
                _entity.Recycle();
            }
            _entity = null;
        }

        public void Recycle()
        {
            Reset();
        }
    }
}