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
    }
}