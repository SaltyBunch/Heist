using Game;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(FloorManager))]
    public class FloorManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {

            var floor = target as FloorManager;

            base.OnInspectorGUI();

            if (GUILayout.Button("ResetChildren"))
            {
                floor.SetLayersRecursive(floor.transform, floor.Floor);
            }
        }


    }
}