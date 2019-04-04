using Game;
using UI;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlayerUIManager))]
    public class PlayerUIManagerEditor : UnityEditor.Editor
    {
        private int num = 0;

        public override void OnInspectorGUI()
        {
            var uiManager = target as PlayerUIManager;

            base.OnInspectorGUI();


            num = EditorGUILayout.Popup("Player Number", num, new string[] {"1", "2", "3", "4"});

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Top Left"))
            {
                uiManager.SetPosition(new Rect()
                {
                    x = 0, y = 1
                }, num);
            }

            if (GUILayout.Button("Top Right"))
            {
                uiManager.SetPosition(new Rect()
                {
                    x = 1, y = 1
                }, num);
            }

            if (GUILayout.Button("Bottom Left"))
            {
                uiManager.SetPosition(new Rect()
                {
                    x = 0, y = 0
                }, num);
            }

            if (GUILayout.Button("Bottom Right"))
            {
                uiManager.SetPosition(new Rect()
                {
                    x = 1, y = 0
                }, num);
            }

            GUILayout.EndHorizontal();
        }
    }
}