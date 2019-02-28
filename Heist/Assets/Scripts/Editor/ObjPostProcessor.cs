using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ObjPostProcessor : AssetPostprocessor
    {
        // Use this for initialization Import Settings global "Scale Factor" = 1:1 and not 0.01
        void OnPreprocessModel()
        {
            var asset = assetImporter as ModelImporter;
            if (assetPath.Contains(".obj"))
            {
                Debug.Log("Changed "  + assetPath + " to file scale 0.0625");
                asset.globalScale = 0.0625f;
            }

        }


        [MenuItem("SaltyBunch/Scale Objects")]
        static void DoSomething()
        {
            foreach (string s in AssetDatabase.GetAllAssetPaths()
                .Where(s => s.EndsWith(".obj", StringComparison.OrdinalIgnoreCase)))
            {
                var model = (ModelImporter)AssetDatabase.LoadAssetAtPath(s, typeof(ModelImporter));
                Debug.Log("Changed "  + model.assetPath + " to file scale 0.0625");
                model.globalScale = 0.0625f;
            }
        }
    }
}