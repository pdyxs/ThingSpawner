using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif
using HutongGames.PlayMaker;

namespace PDYXS.ThingSpawner
{
    public abstract class Actor<TBaseObject, TEventEnum, TStateEnum> :
        FSMWrapper<TEventEnum, TStateEnum>,
        ISpawnTrackable,
        IPrefabSaveable,
        IActor<TBaseObject>
        where TBaseObject : class
        where TEventEnum : System.IConvertible
        where TStateEnum : System.IConvertible
    {
        public TBaseObject entity
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

        public virtual void Initialise(TBaseObject obj)
        {
            entity = obj;
            hasSpawned = true;
        }

        [SerializeField]
        private PrefabSaver prefabSaver;
    }

    public interface IActor<TBaseObject> : IInitialisable<TBaseObject> {
    }
}