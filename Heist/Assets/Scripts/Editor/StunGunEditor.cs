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
    }
}