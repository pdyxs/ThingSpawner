using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDYXS.ThingSpawner
{
    public class SpawnableList<TComponent, TModelClass> : Spawnable<TComponent>,
        IEnumerable
        where TComponent : MonoBehaviour, IInitialisable<TModelClass>, ISpawnTrackable
        where TModelClass : class
    {
        protected List<TComponent> entities = new List<TComponent>();

        private EventList<TModelClass> baseEntities;

        public TComponent this[int i]
        {
            get
            {
                return entities[i];
            }
        }

        public int Count
        {
            get
            {
                return entities.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        public void Initialise(EventList<TModelClass> bes)
        {
            Initialise();
            Clear();
            if (bes == null)
            {
                return;
            }
            foreach (var ce in bes)
            {
                Add(ce);
            }
            baseEntities = bes;
            bes.OnAdded.AddListener(Add);
            bes.OnRemoved.AddListener(Remove);
            bes.OnCleared.AddListener(Clear);
            bes.OnInserted.AddListener(Insert);
            bes.OnRemovedAt.AddListener(RemoveAt);

            CheckPool();
            foreach (var obj in preBuiltObjects) {
                obj.Recycle();
            }
            preBuiltObjects.Clear();
        }

        public void Recycle()
        {
            baseEntities.OnAdded.RemoveListener(Add);
            baseEntities.OnRemoved.RemoveListener(Remove);
            baseEntities.OnCleared.RemoveListener(Clear);
            baseEntities.OnInserted.RemoveListener(Insert);
            baseEntities.OnRemovedAt.RemoveListener(RemoveAt);
            Clear();
        }

        protected virtual void Clear()
        {
            foreach (var e in entities)
            {
                if (e.isActiveAndEnabled)
                {
                    e.Recycle();
                }
            }
            entities.Clear();
        }

        protected virtual void Add(TModelClass ce)
        {
            var obj = Spawn();
            if (obj != null) {
                obj.Initialise(ce);
                entities.Add(obj);
            }
        }

        protected virtual void Insert(int index, TModelClass ce)
        {
            var obj = Spawn();
            obj.Initialise(ce);
            entities.Insert(index, obj);
        }

        protected virtual void Remove(TComponent obj)
        {
            entities.Remove(obj);
            obj.Recycle();
        }

        protected virtual void Remove(TModelClass ce)
        {
            var o = entities.Find((obj) => obj.entity == ce);
            if (o != null)
            {
                Remove(o);
            }
        }

        protected virtual void RemoveAt(int index)
        {
            var obj = entities[index];
            entities.RemoveAt(index);
            obj.Recycle();
        }
    }
}
