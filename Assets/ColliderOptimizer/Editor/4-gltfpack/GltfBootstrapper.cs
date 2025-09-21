#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ColliderOptimizer.Gltfpack
{
    [InitializeOnLoad]
    static class GltfBootstrapper
    {
        const string PrefKey = "K_GLTF";
        static ListRequest _list;
        static AddRequest _add;
        static RemoveRequest _rmv;

        static GltfBootstrapper()
        {
            if (SessionState.GetBool(PrefKey, false)) return;
            EditorApplication.update += StartCheck;
        }
        static void StartCheck()
        {
            EditorApplication.update -= StartCheck;
            _list = Client.List(true);
            EditorApplication.update += OnList;
        }
        static void OnList()
        {
            if (!_list.IsCompleted) return;
            EditorApplication.update -= OnList;

            bool hasGlTFast = false, hasUnityGLTF = false;

            if (_list.Status == StatusCode.Success)
            {
                foreach (var p in _list.Result)
                {
                    if (p.name == "com.unity.cloud.gltfast") hasGlTFast = true;
                    if (p.name == "com.unity.formats.glTF") hasUnityGLTF = true;
                }
            }
            else
                Debug.LogWarning("pkg list failed: " + _list.Error?.message);

            if (!hasGlTFast)
            {
                if (EditorUtility.DisplayDialog(
                    "Install glTFast?",
                    "Need 'com.unity.cloud.gltfast' for GLB import, install it now?",
                    "Install", "Not now"))
                {
                    _add = Client.Add("com.unity.cloud.gltfast");
                    EditorApplication.update += OnAdd;
                    return;
                }
            }
            if (hasGlTFast && hasUnityGLTF)
            {
                if (EditorUtility.DisplayDialog(
                    "Multiple glTF importers detected",
                    "Both 'com.unity.cloud.gltfast' and 'com.unity.formats.glTF' are installed.\n" +
                    "To make glTFast the default importer, it's safest to remove UnityGLTF.\nRemove it now?",
                    "Remove UnityGLTF", "Keep both"))
                {
                    _rmv = Client.Remove("com.unity.formats.glTF");
                    EditorApplication.update += OnRemove;
                    return;
                }
            }

            SessionState.SetBool(PrefKey, true);
        }
        static void OnAdd()
        {
            if (!_add.IsCompleted) return;
            EditorApplication.update -= OnAdd;

            if (_add.Status == StatusCode.Success)
                Debug.Log("installed " + _add.Result.packageId);
            else
                Debug.LogError("failed to install glTFast: " + _add.Error.message);

            SessionState.SetBool(PrefKey, true);
        }
        static void OnRemove()
        {
            if (!_rmv.IsCompleted) return;
            EditorApplication.update -= OnRemove;

            if (_rmv.Status == StatusCode.Success)
                Debug.Log("removed 'com.unity.formats.glTF'.");
            else
                Debug.LogError("failed to remove UnityGLTF: " + _rmv.Error.message);

            SessionState.SetBool(PrefKey, true);
        }
    }
}
#endif