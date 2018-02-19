using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PDYXS.ThingSpawner
{
    [CustomPropertyDrawer(typeof(Spawnable), true)]
    public class SpawnableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + EditorDrawer.BOX_PADDING * 2;
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            pos = EditorDrawer.Box(pos);

            EditorGUI.LabelField(EditorDrawer.WithHeight(pos, EditorGUIUtility.singleLineHeight), 
                                 property.displayName,
                                 EditorStyles.boldLabel);
            pos = EditorDrawer.FromHeight(pos, EditorGUIUtility.singleLineHeight);

            pos = EditorDrawer.FromWidth(pos, 10);
            var rect = new Rect(pos.x + pos.width - 50, pos.y, 50, pos.height);
            pos = new Rect(pos.x, pos.y, pos.width - 50, pos.height);

            pos = EditorDrawer.PropertyField(pos, property, "parent");

            var spawnable = (property.GetObject() as Spawnable);
            spawnable.basePrefab =
                EditorGUI.ObjectField(
                    EditorDrawer.WithHeight(pos, EditorGUIUtility.singleLineHeight),
                    "Prefab",
                    spawnable.basePrefab,
                    spawnable.prefabType,
                    false
                ) as MonoBehaviour;
            EditorUtility.SetDirty(property.GetParent() as Object);
            pos = EditorDrawer.FromHeight(pos, EditorGUIUtility.singleLineHeight);

            if (spawnable.basePrefab == null || spawnable.parent == null || Application.isPlaying) {
                GUI.enabled = false;
            }
            if (GUI.Button(rect, "Build")) {
                var spawned = spawnable.EditorSpawn();
                spawned.gameObject.hideFlags = HideFlags.DontSaveInBuild;
            }
            GUI.enabled = true;
        }
    }
}