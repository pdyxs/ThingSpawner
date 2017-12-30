using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PDYXS.ThingSpawner
{
    public class VariedSpawnableListDrawer<T,U> : PropertyDrawer
        where T : MonoBehaviour, IInitialisable<U>, ISpawnTrackable
        where U : class
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 
                                   (3 + property.FindPropertyRelative("prefabs").arraySize) + 
                                   EditorDrawer.BOX_PADDING * 2;
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
            var spawnable = (property.GetObject() as VariedSpawnableList<T,U>);

            var prefabs = property.FindPropertyRelative("prefabs");
            var counts = property.FindPropertyRelative("counts");

            var countRect = new Rect(pos.x, pos.y, 30, pos.height);
            pos = new Rect(pos.x + 30, pos.y, pos.width - 50, pos.height);
            var rmRect = new Rect(pos.x + pos.width, pos.y, 20, pos.height);
            for (int i = 0; i != prefabs.arraySize; ++i) {

                countRect = EditorDrawer.PropertyField(countRect, counts.GetArrayElementAtIndex(i),
                                                       GUIContent.none);
                                        
                var prefabProp = prefabs.GetArrayElementAtIndex(i);
                pos = EditorDrawer.PropertyField(pos, prefabProp, GUIContent.none);
                rmRect = EditorDrawer.DefaultButton(rmRect, "-", Color.red, () => {
                    prefabs.DeleteArrayElementAtIndex(i);
                    counts.DeleteArrayElementAtIndex(i);
                    i-=1;
                });
            }

            pos = EditorDrawer.Button(pos, "+", Color.green, () => {
                prefabs.arraySize++;
                counts.arraySize++;
            });

            if (GUI.Button(rect, "Build"))
            {
                var spawned = spawnable.EditorSpawn();
                spawned.gameObject.hideFlags = HideFlags.DontSaveInBuild;
            }
            GUI.enabled = true;

        }

    }
}