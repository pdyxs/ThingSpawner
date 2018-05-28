using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Component = UnityEngine.Component;
using EditorCoroutines;

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
            var target = property.serializedObject.targetObject;
            var go = (target as Component).gameObject;
            if (PrefabUtility.FindPrefabRoot(go) != go)
            {
                return;
            }
            
            EditorDrawer.Button(position, "Save Prefab", new Color(0.5f, 0.5f, 1),
            () =>
            {
                var mono = property.serializedObject.targetObject as MonoBehaviour;

                var toSave = mono.GetComponentsInChildren<IPrefabSaveable>(true).ToList().ConvertAll(
                        (a) => (a as Component).gameObject
                    );
                toSave.Add(mono.gameObject);

                toSave.RemoveAll((g) => PrefabUtility.FindPrefabRoot(g) != g);

                var oldHideFlags = new Dictionary<GameObject, HideFlags>();
                foreach (var go1 in toSave)
                {
                    oldHideFlags[go1] = go1.hideFlags;
                    go1.hideFlags = HideFlags.DontSave;
                }

                foreach (var go2 in toSave)
                {
                    go2.hideFlags = HideFlags.None;
                    if (PrefabUtility.GetPrefabType(go2) == PrefabType.DisconnectedPrefabInstance)
                    {
                        PrefabUtility.ReconnectToLastPrefab(go2);
                    }
                    PrefabUtility.ReplacePrefab(go2, PrefabUtility.GetPrefabParent(go2), ReplacePrefabOptions.ConnectToPrefab);
                    go2.hideFlags = HideFlags.DontSave;
                }

                foreach (var go3 in toSave)
                {
                    go3.hideFlags = oldHideFlags[go3];
                }
            });
        }
    }
}