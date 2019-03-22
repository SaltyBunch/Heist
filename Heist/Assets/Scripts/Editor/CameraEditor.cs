using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(Hazard.Camera))]
    public class CameraEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var camera = target as Hazard.Camera;

            EditorGUI.BeginChangeCheck();
            var temp = camera.transform.InverseTransformPoint(FieldOfView.DrawFieldOfView(camera.localLensePos, camera.fieldOfView, camera.range, camera.transform));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(camera, "Change lense position");
                camera.localLensePos = temp;
            }
        }
    }
}