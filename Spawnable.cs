using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PDYXS.ThingSpawner
{
    [System.Serializable]
    public class Spawnable
    {
        public MonoBehaviour basePrefab;

        public Transform parent;

        public virtual System.Type prefabType
        {
            get
            {
                return typeof(MonoBehaviour);
            }
        }

#if UNITY_EDITOR
        public virtual MonoBehaviour EditorSpawn()
        {
            var ret = UnityEditor.PrefabUtility.InstantiatePrefab(
                basePrefab
            ) as MonoBehaviour;
            ret.transform.SetParent(parent);
            return ret;
        }
#endif
    }

    public abstract class Spawnable<T> : Spawnable
        where T : MonoBehaviour, ISpawnTrackable
    {
        public T prefab {
            get {
                return basePrefab as T;
            }
        }

        public override System.Type prefabType {
            get {
                return typeof(T);
            }
        }

        protected bool hasPool = false;

        protected List<T> preBuiltObjects = new List<T>();

        protected void Initialise() {
            if (parent == null) {
                Debug.LogError("Spawning " + typeof(T).ToString() + " failed: Missing parent");
                return;
            }
            foreach (T ch in parent.GetComponentsInChildren<T>(false))
            {
                if (!ch.HasSpawned)
                {
                    preBuiltObjects.Add(ch);
                }
            }
        }

        protected bool CheckSpawn(T prefab, Transform parent)
        {
            if (prefab == null)
            {
                Debug.LogError("Spawning " + prefabType.ToString() + " failed: Missing prefab");
                return false;
            }
            if (parent == null)
            {
                Debug.LogError("Spawning " + prefabType.ToString() + " failed: Missing parent");
                return false;
            }
            return true;
        }

        protected virtual T Spawn()
        {
            if (!CheckSpawn(prefab, parent))
            {
                return null;
            }
            if (!hasPool)
            {
                ObjectPool.CreatePool(prefab, 1);
                hasPool = true;
            }
            if (preBuiltObjects.Count > 0)
            {
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

        private EventObject<U> eventObject = null;

        public void Initialise(U ce)
        {
            Initialise();
            DoSpawn(ce);
        }

        private void DoSpawn(U ce) {
            if (ce == null)
            {
                Reset();
                return;
            }
            if (_entity == null || !_entity.isActiveAndEnabled)
            {
                _entity = Spawn();
            }
            if (_entity != null)
            {
                _entity.Initialise(ce);
            }
        }

        public void Initialise(EventObject<U> eo) {
            Initialise(eo.val);
            eventObject = eo;
            eo.OnChanged.AddListener(DoSpawn);
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
            if (eventObject != null) {
                eventObject.OnChanged.RemoveListener(DoSpawn);
                eventObject = null;
            }
            Reset();
        }
    }
}