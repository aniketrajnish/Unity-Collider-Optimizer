#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityColliderOptimizer.Utils
{
    public static class ColliderOptEditorUtils
    {
        public static T LoadAssetViaPanel<T>(string __title, string __ext) where T : Object
        {
            var sysPath = EditorUtility.OpenFilePanel(__title, Application.dataPath, __ext);
            if (string.IsNullOrEmpty(sysPath)) return null;

            var dataPath = Application.dataPath.Replace('\\', '/');
            sysPath = sysPath.Replace('\\', '/');
            if (!sysPath.StartsWith(dataPath))
            {
                EditorUtility.DisplayDialog("Invalid selection",
                    "Pick an asset inside this project (under Assets/).", "OK");
                return null;
            }
            var rel = "Assets" + sysPath.Substring(dataPath.Length);
            return AssetDatabase.LoadAssetAtPath<T>(rel);
        }
    }
}
#endif
