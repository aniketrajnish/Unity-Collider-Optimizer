#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityColliderOptimizer.Services;
using UnityColliderOptimizer.Core.P;
namespace UnityColliderOptimizer.E
{
    public class AssetPersistenceEditor : IAssetPersistence
    {
        public bool SaveMeshAsset(Mesh mesh, string __name)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Mesh", __name, "asset", "Pick a location");
            if (string.IsNullOrEmpty(path)) return false;
            var clone = Object.Instantiate(mesh);
            AssetDatabase.CreateAsset(clone, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(clone);
            return true;
        }
        public bool SavePathDataAsset(OptimizedPathData data, string __name)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Path Data", __name, "asset", "Pick a location");
            if (string.IsNullOrEmpty(path)) return false;
            var clone = Object.Instantiate(data);
            AssetDatabase.CreateAsset(clone, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(clone);
            return true;
        }
    }
}
#endif