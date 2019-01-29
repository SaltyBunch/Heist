using System;
using Hazard;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using ZeroMQ.lib;

namespace Editor
{
    [CustomEditor(typeof(LethalLaser))]
    public class LethalLaserEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var laser = target as LethalLaser;


            var l1Position = laser.Laser1.transform.position;
            var l1Rotation = laser.Laser1.transform.rotation;
            var l2Position = laser.Laser2.transform.position;
            var l2Rotation = laser.Laser2.transform.rotation;


            Handles.color = Color.red;

            Handles.DrawLine(l1Position, l2Position);

            EditorGUI.BeginChangeCheck();
            switch (Tools.current)
            {
                case Tool.View:
                    break;
                case Tool.Move:
                    l1Position = Handles.PositionHandle(l1Position, l1Rotation);
                    l2Position = Handles.PositionHandle(l2Position, l2Rotation);
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
                Undo.RecordObject(laser.Laser1.transform, "Change laser1 transform");
                Undo.RecordObject(laser.Laser2.transform, "Change laser2 transform");
                Undo.RecordObject(laser.transform, "Change parent transform");
                foreach (var lr in laser.LineRenderer)
                {
                    Undo.RecordObject(lr, "Change line transform");
                    lr.SetPositions(new Vector3[]
                    {
                        laser.Laser1.transform.localPosition, laser.Laser2.transform.localPosition
                    });
                }

                Undo.RecordObject(laser.Collider, "Change collider bounds");


                laser.transform.position = (l1Position + l2Position) / 2;

                laser.transform.LookAt(l1Position);
                
                laser.Laser1.transform.position = l1Position;
                laser.Laser2.transform.position = l2Position;
                
                

                laser.Laser1.transform.LookAt(laser.Laser2.transform);
                laser.Laser2.transform.LookAt(laser.Laser1.transform);
                
                
                laser.Collider.size = new Vector3()
                {
                    x = laser.Laser1.transform.localPosition.x * 2+ 0.1f,
                    z = laser.Laser1.transform.localPosition.z * 2 , y =  1.5f};
            }
        }
    }
}