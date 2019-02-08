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

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<"))
            {
                inv.SelectedIndex--;
            }

            if (GUILayout.Button(">"))
            {
                inv.SelectedIndex++;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            if (inv.SelectedItem != null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.IntField(inv.SelectedIndex, new GUILayoutOption[] {GUILayout.ExpandWidth(false)});
                EditorGUILayout.ObjectField(inv.SelectedItem, typeof(Item), true);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}