using System;
using Game;
using Hazard;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ElectricField))]
    public class ElectricFieldEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var electric = target as ElectricField;

            base.OnInspectorGUI();
            if (GUILayout.Button("Place")) electric.Place(electric.transform.position, null);
        }

        private void OnSceneGUI()
        {
            var electric = target as ElectricField;


            var e1Position = electric.Electric1.transform.position;
            var e1Rotation = electric.Electric1.transform.rotation;
            var e2Position = electric.Electric2.transform.position;
            var e2Rotation = electric.Electric2.transform.rotation;


            Handles.color = Color.red;

            Handles.DrawLine(e1Position, e2Position);

            EditorGUI.BeginChangeCheck();
            switch (Tools.current)
            {
                case Tool.View:
                    break;
                case Tool.Move:
                    e1Position = Handles.PositionHandle(e1Position, e1Rotation);
                    e2Position = Handles.PositionHandle(e2Position, e2Rotation);
                    break;
                case Tool.Rotate:
                    break;
                case Tool.Scale:
                    break;
                case Tool.Rect:
                    break;
                case Tool.Transform:
                    break;
                case Tool.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(electric.Electric1.transform, "Change laser1 transform");
                Undo.RecordObject(electric.Electric2.transform, "Change laser2 transform");
                Undo.RecordObject(electric.transform, "Change parent transform");

                Undo.RecordObject(electric.Collider, "Change collider bounds");


                electric.transform.position = (e1Position + e2Position) / 2;

                electric.transform.LookAt(e1Position);

                electric.Electric1.transform.position = e1Position;
                electric.Electric2.transform.position = e2Position;


                electric.Electric1.transform.LookAt(electric.Electric2.transform);
                electric.Electric2.transform.LookAt(electric.Electric1.transform);


                electric.Collider.center = new Vector3
                {
                    x = 0, y = 1.25f, z = 0
                };

                electric.Collider.size = new Vector3
                {
                    x = electric.Electric1.transform.localPosition.x * 2 + 2,
                    z = electric.Electric1.transform.localPosition.z * 2 + 2, y = 2.5f
                };
            }
        }
    }
}