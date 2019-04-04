using Level;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GoldPile))]
    public class GoldPileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var goldPile = target as GoldPile;
            base.OnInspectorGUI();
            if (GUILayout.Button("Update")) goldPile.PercentageRemaining = goldPile.Percentage;
        }
    }
}