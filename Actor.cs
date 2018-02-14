using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif
using HutongGames.PlayMaker;

namespace PDYXS.ThingSpawner
{
    [RequireComponent(typeof(PlayMakerFSM))]
    public abstract class Actor<TTrackedObject, TEventEnum>
        : Actor<TTrackedObject>
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
        where TTrackedObject : class
        where TEventEnum : System.IConvertible
    {
        private PlayMakerFSM m_fsm;
        public PlayMakerFSM fsm
        {
            get
            {
                if (m_fsm == null)
                {
                    m_fsm = GetComponent<PlayMakerFSM>();
                }
                return m_fsm;
            }
        }

        public void SendEvent(TEventEnum eventType)
        {
            if (!typeof(TEventEnum).IsEnum)
            {
                throw new System.ArgumentException("Actor T must be an enumerated type");
            }
            fsm.SendEvent(System.Enum.GetName(typeof(TEventEnum), eventType));
        }

#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            UnityEditor.EditorApplication.delayCall += ApplyEvents;
        }

        void ApplyEvents()
        {
            if (Application.isPlaying || this == null || gameObject == null)
            {
                return;
            }

            if (!typeof(TEventEnum).IsEnum)
            {
                throw new System.ArgumentException("Actor T must be an enumerated type");
            }

            if (fsm == null)
            {
                gameObject.AddComponent<PlayMakerFSM>();
            }

            var targetFsm = fsm.UsesTemplate ? fsm.FsmTemplate.fsm : fsm.Fsm;
            if (targetFsm == null)
            {
                return;
            }

            var eventNames = System.Enum.GetNames(typeof(TEventEnum));
            var missingEventNames = eventNames.Where(x => targetFsm.Events.All(y => y.Name != x));
            targetFsm.Events = targetFsm.Events.Concat(missingEventNames.Select(x => new FsmEvent(x)))
                .ToArray();
        }
#endif
    }

    public abstract class Actor<T> : 
        Actor, 
        IInitialisable<T>, 
        ISpawnTrackable,
        IPrefabSaveable
        where T : class
    {
        public T entity
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

        public virtual void Initialise(T obj)
        {
            entity = obj;
            hasSpawned = true;
        }

        [SerializeField]
        private PrefabSaver prefabSaver;
    }

    public abstract class Actor : MonoBehaviour {}
}