using UnityEngine;

namespace Editor
{
    public class FieldOfView
    {
        public static Vector3 DrawFieldOfView(Vector3 pos, float fieldOfView, float range, Transform transform)
        {
            var localPos = transform.TransformPoint(pos);
            var angle = (fieldOfView / 180) * Mathf.PI;


            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);


            var forward = transform.forward;

            UnityEditor.Handles.color = Color.red;

            UnityEditor.Handles.DrawLine(localPos, localPos + forward * range * cos + transform.right * range * sin);
            UnityEditor.Handles.DrawLine(localPos,
                localPos + forward * range * cos + transform.right * -1 * range * sin);
            UnityEditor.Handles.DrawLine(localPos, localPos + forward * range * cos + transform.up * range * sin);
            UnityEditor.Handles.DrawLine(localPos, localPos + forward * range * cos + transform.up * -1 * range * sin);

            UnityEditor.Handles.DrawWireDisc(localPos + forward * cos * range, forward,
                sin * range);


            pos = UnityEditor.Handles.PositionHandle(localPos, transform.rotation);

            return pos;
        }
    }
}