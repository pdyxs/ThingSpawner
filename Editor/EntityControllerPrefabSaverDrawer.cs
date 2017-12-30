using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PDYXS.Skins;

namespace PDYXS.ThingSpawner
{
    [CustomPropertyDrawer(typeof(EntityControllerPrefabSaver))]
    public class EntityControllerPrefabSaverDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorDrawer.Button(position, "Save Prefab", new Color(0.5f, 0.5f, 1),
            () =>
            {
                var mono = property.serializedObject.targetObject as MonoBehaviour;
                Dictionary<GameObject, Transform> parentStore = new Dictionary<GameObject, Transform>();
                foreach (var c in mono.GetComponentsInChildren<EntityControllerBehaviour>()) {
                    parentStore[c.gameObject] = c.transform.parent;
                }
                foreach (var c in mono.GetComponentsInChildren<SkinnedObject>()) {
                    parentStore[c.gameObject] = c.transform.parent;
                }

                foreach (var stuff in parentStore) {
                    stuff.Key.transform.SetParent(mono.transform.parent);
                    stuff.Key.transform.localScale = Vector3.one;
                }

                foreach (var stuff in parentStore) {
                    if (PrefabUtility.GetPrefabType(stuff.Key) == PrefabType.DisconnectedPrefabInstance)
                    {
                        PrefabUtility.ReconnectToLastPrefab(stuff.Key);
                    }
                    PrefabUtility.ReplacePrefab(stuff.Key, PrefabUtility.GetPrefabParent(stuff.Key));
                    PrefabUtility.RevertPrefabInstance(stuff.Key);
                }

                foreach (var stuff in parentStore) {
                    stuff.Key.transform.SetParent(stuff.Value);
                    stuff.Key.transform.localScale = Vector3.one;
                    EditorUtility.SetDirty(stuff.Key);
                }
            });
        }
    }
}