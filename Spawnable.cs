using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PDYXS.ThingSpawner
{
    public abstract class Spawnable<T> where T : MonoBehaviour, ISpawnTrackable
    {
        public T prefab;
        public Transform parent;

        private bool hasPool = false;

        protected bool CheckSpawn(T prefab, Transform parent) {
            if (prefab == null)
            {
                Debug.LogError("Spawning " + typeof(T).ToString() + " failed: Missing prefab");
                return false;
            }
            if (parent == null)
            {
                Debug.LogError("Spawning " + typeof(T).ToString() + " failed: Missing parent");
                return false;
            }
            return true;
        }

        private List<T> preBuiltObjects = new List<T>();

        protected void Initialise() {
            foreach (T ch in parent.GetComponentsInChildren<T>(false))
            {
                if (!ch.HasSpawned)
                {
                    preBuiltObjects.Add(ch);
                }
            }
        }

        protected virtual T Spawn()
        {
            if (!CheckSpawn(prefab, parent)) {
                return null;
            }
            if (!hasPool)
            {
                ObjectPool.CreatePool(prefab, 1);
                hasPool = true;
            }
            if (preBuiltObjects.Count > 0) {
                var obj = preBuiltObjects[0];
                preBuiltObjects.RemoveAt(0);
                return obj;
            }
            return prefab.Spawn(parent);
        }
    }

    public class Spawnable<T, U> : Spawnable<T>
        where T : MonoBehaviour, IInitialisable<U>, ISpawnTrackable
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
            Initialise();
            Reset();
            if (ce == null)
            {
                return;
            }
            _entity = Spawn();
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