using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDYXS.ThingSpawner
{
    public class VariedSpawnableList<T, U> : SpawnableList<T, U>
        where T : MonoBehaviour, IEntityInitialisable<U>, ISpawnTrackable
        where U : class
    {
        public List<T> prefabs = new List<T>();

        public List<int> counts = new List<int>();

        protected T spawnableFor(int index) {
            var count = 0;
            for (int i = 0; i != prefabs.Count; ++i)
            {
                count += counts[i];
                if (counts[i] == 0 || count > index || i == prefabs.Count - 1)
                {
                    return prefabs[i];
                }
            }
            return null;
        }

        protected virtual T Spawn(int index)
        {
            var spawnable = spawnableFor(index);
            if (spawnable == null) {
                return null;
            }

            if (!CheckSpawn(spawnable, parent))
            {
                return null;
            }
            //if (!hasPool)
            //{
                ObjectPool.CreatePool(spawnable, 1);
                hasPool = true;
            //}
            if (preBuiltObjects.Count > 0)
            {
                var obj = preBuiltObjects[0];
                preBuiltObjects.RemoveAt(0);
                return obj;
            }
            return spawnable.Spawn(parent);
        }

        protected override T Spawn()
        {
            return Spawn(entities.Count);
        }

        protected override void Insert(int index, U ce)
        {
            var obj = Spawn(index);
            obj.Initialise(ce);
            entities.Insert(index, obj);
        }

#if UNITY_EDITOR
        public override MonoBehaviour EditorSpawn()
        {
            var ret = UnityEditor.PrefabUtility.InstantiatePrefab(
                spawnableFor(parent.GetComponentsInChildren<T>().Length)
            ) as MonoBehaviour;
            ret.transform.SetParent(parent);
            ret.transform.localScale = Vector3.one;
            return ret;
        }
#endif

    }
}