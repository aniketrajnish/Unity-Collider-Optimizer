#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ColliderOptimizer.Utils
{
    public static class OptEditorHelpers
    {
        public static T LoadAssetViaPanel<T>(string __title, string __ext) where T : Object
        {
            var sysPath = EditorUtility.OpenFilePanel(__title, Application.dataPath, __ext);
            if (string.IsNullOrEmpty(sysPath)) return null;

            var dataPath = Application.dataPath.Replace('\\', '/');
            sysPath = sysPath.Replace('\\', '/');
            if (!sysPath.StartsWith(dataPath))
            {
                EditorUtility.DisplayDialog("invalid selection",
                    "pick an asset under Assets/", "OK");
                return null;
            }
            var rel = "Assets" + sysPath.Substring(dataPath.Length);
            return AssetDatabase.LoadAssetAtPath<T>(rel);
        }
    }
}
#endif
