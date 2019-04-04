using System;
using Character;
using Game;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var inv = target as Inventory;
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            foreach (var key in (KeyType[]) Enum.GetValues(typeof(KeyType)))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Enum.GetName(typeof(KeyType), key), GUILayout.Width(128));
                GUILayout.Label(inv.keys[key].ToString());
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<")) inv.SelectedIndex--;
            if (GUILayout.Button(">")) inv.SelectedIndex++;
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            if (inv.SelectedItem != null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.IntField(inv.SelectedIndex, GUILayout.ExpandWidth(false));
                EditorGUILayout.ObjectField(inv.SelectedItem, typeof(Item), true);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}