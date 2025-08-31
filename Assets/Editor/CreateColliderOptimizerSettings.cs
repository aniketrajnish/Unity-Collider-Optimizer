#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityColliderOptimizer.Core;
namespace UnityColliderOptimizer.E
{
    static class CreateColliderOptimizerSettings
    {
        [MenuItem("Tools/Collider Optimizer/Create Settings Asset")]
        public static void Create()
        {
            const string dir = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets", "Resources");

            var path = $"{dir}/ColliderOptimizerSettings.asset";
            var existing = AssetDatabase.LoadAssetAtPath<OptimizationSettings>(path);
            if (existing)
            {
                EditorGUIUtility.PingObject(existing);
                Selection.activeObject = existing;
                return;
            }

            var inst = ScriptableObject.CreateInstance<OptimizationSettings>();
            AssetDatabase.CreateAsset(inst, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(inst);
            Selection.activeObject = inst;
        }
    }
}
#endif