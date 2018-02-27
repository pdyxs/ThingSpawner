using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PDYXS.ThingSpawner
{
    [CustomPropertyDrawer(typeof(PrefabSaver))]
    public class PrefabSaverDrawer : PropertyDrawer
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
                foreach (var c in mono.GetComponentsInChildren<MonoBehaviour>()) {
                    if (c is IPrefabSaveable)
                    {
                        parentStore[c.gameObject] = c.transform.parent;
                    }
                }

                foreach (var stuff in parentStore) {
                    stuff.Key.transform.SetParent(mono.transform.parent);
                    stuff.Key.transform.localScale = Vector3.one;
                }

                foreach (var stuff in parentStore) {
                    var hf = stuff.Key.gameObject.hideFlags;
                    stuff.Key.gameObject.hideFlags = HideFlags.None;
                    if (PrefabUtility.GetPrefabType(stuff.Key) == PrefabType.DisconnectedPrefabInstance)
                    {
                        PrefabUtility.ReconnectToLastPrefab(stuff.Key);
                    }
                    PrefabUtility.ReplacePrefab(stuff.Key, PrefabUtility.GetPrefabParent(stuff.Key));
                    PrefabUtility.RevertPrefabInstance(stuff.Key);
                    stuff.Key.gameObject.hideFlags = hf;
                }

                foreach (var stuff in parentStore) {
                    stuff.Key.transform.SetParent(stuff.Value, false);
                    stuff.Key.transform.localScale = Vector3.one;
                    EditorUtility.SetDirty(stuff.Key);
                }
            });
        }
    }
}