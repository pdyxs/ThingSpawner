﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PDYXS.ThingSpawner
{
    [System.Serializable]
    public abstract class Spawnable
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
            ret.gameObject.hideFlags = HideFlags.DontSave;
            ret.transform.SetParent(parent);
            ret.transform.localScale = Vector3.one;
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
        
        public T Get => _entity;
        internal T _entity;

        public bool Exists => _entity != null;

        public void Set(T val)
        {
            _entity = val;
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

        protected void CheckPool() 
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif
            if (!hasPool)
            {
                ObjectPool.CreatePool(prefab, 1);
                hasPool = true;
            }
        }

        protected virtual T Spawn()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return EditorSpawn().GetComponent<T>();
            }
#endif
            if (!CheckSpawn(prefab, parent))
            {
                return null;
            }
            CheckPool();
            if (preBuiltObjects.Count > 0)
            {
                var obj = preBuiltObjects[0];
                preBuiltObjects.RemoveAt(0);
                return obj;
            }

            var ret = prefab.Spawn(parent);
            var layout = parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layout != null) {
                layout.StartCoroutine(FixLayout(layout));
            }
            return ret;
        }

        //#overkill
        private IEnumerator FixLayout(HorizontalOrVerticalLayoutGroup layout) {
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
            layout.CalculateLayoutInputVertical();
            layout.CalculateLayoutInputHorizontal();
            layout.SetLayoutVertical();
            layout.SetLayoutHorizontal();
        }
    }

    public class Spawnable<T, U> : Spawnable<T>
        where T : MonoBehaviour, IEntityInitialisable<U>, ISpawnTrackable
        where U : class
    {
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
                _entity.Despawn();
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