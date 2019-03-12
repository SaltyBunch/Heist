using Game;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BarQuickTimeEvent))]
    public class BarQuickTimeEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var bar = target as BarQuickTimeEvent;

            if (GUILayout.Button("Generate"))
            {
                bar.Generate();
            }
        }
    }
}