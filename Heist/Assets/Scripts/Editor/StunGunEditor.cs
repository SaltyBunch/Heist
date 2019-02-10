using System;
using UnityEditor;
using UnityEngine;
using Weapon;

namespace Editor
{
    [CustomEditor(typeof(StunGun))]
    public class StunGunEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            var stunGun = target as StunGun;

            if (GUILayout.Button("Fire"))
            {
                if (stunGun != null) stunGun.Attack();
            }
        }

        private void OnSceneGUI()
        {
            var stunGun = target as StunGun;

            var barrel = stunGun.transform.TransformPoint(stunGun.Barrel);

            EditorGUI.BeginChangeCheck();
            switch (Tools.current)
            {
                case Tool.View:
                    break;
                case Tool.Move:
                    barrel = Handles.PositionHandle(barrel, Quaternion.identity);
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
                Undo.RecordObject(stunGun, "Change barrel transform");
                stunGun.Barrel = stunGun.transform.InverseTransformPoint(barrel);
            }
        }
    }
}