using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDYXS.ThingSpawner
{
    public class SpawnableList<T, U> : Spawnable<T>,
        IEnumerable
        where T : MonoBehaviour, IInitialisable<U>
        where U : class
    {
        protected List<T> entities = new List<T>();

        private EventList<U> baseEntities;

        public T this[int i]
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

        public void Initialise(EventList<U> bes)
        {
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

        protected void Clear()
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

        protected void Add(U ce)
        {
            var obj = Spawn();
            obj.Initialise(ce);
            entities.Add(obj);
        }

        protected void Insert(int index, U ce)
        {
            var obj = Spawn();
            obj.Initialise(ce);
            entities.Insert(index, obj);
        }

        protected void Remove(T obj)
        {
            entities.Remove(obj);
            obj.Recycle();
        }

        protected void Remove(U ce)
        {
            var o = entities.Find((obj) => obj.entity == ce);
            if (o != null)
            {
                Remove(o);
            }
        }

        protected void RemoveAt(int index)
        {
            var obj = entities[index];
            entities.RemoveAt(index);
            obj.Recycle();
        }
    }
}
