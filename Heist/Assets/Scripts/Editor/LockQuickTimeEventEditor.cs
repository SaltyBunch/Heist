using Game;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LockQuickTimeEvent))]
    public class Lock : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var quicktime = target as LockQuickTimeEvent;

            if (GUILayout.Button("Generate"))
            {
                quicktime.Generate();
            }

            if (GUILayout.Button("Clear"))
            {
                quicktime.Clear();
            }
        }
    }
}