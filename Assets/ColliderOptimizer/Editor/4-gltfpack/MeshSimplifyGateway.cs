#if UNITY_EDITOR
using System;
using System.IO;
using ColliderOptimizer.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ColliderOptimizer.Gltfpack
{
    public static class MeshSimplifyGateway
    {
        public static Mesh SimplifyWithGltfpack(
            Mesh __src, float __keepRatio, bool __recalcNormals,
            string __saveDir = "Assets/ColliderOptimizer/Editor/4-opt-out",
            bool __aggressive = false, bool __permissive = false
        )
        {
            if (!__src) return null;
            __keepRatio = Mathf.Clamp01(__keepRatio);

            string tempRoot = Path.Combine(Path.GetTempPath(), "gltfpack_unity");
            string tempDir = Path.Combine(tempRoot, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            string baseName = SanitizeFileName(string.IsNullOrEmpty(__src.name) ? "Mesh" : __src.name);
            string objPath = Path.Combine(tempDir, baseName + ".obj");
            string glbPath = Path.Combine(tempDir, baseName + "-simp.glb");

            try
            {
                ObjExporter.WriteOBJ(__src, objPath);

                if (!GltfpackRunner.Run(objPath, glbPath, __keepRatio, __aggressive, __permissive))
                    return null;

                Directory.CreateDirectory(__saveDir);
                string projGlb = Path.Combine(__saveDir, baseName + "-simp.glb");
                File.Copy(glbPath, projGlb, true);

                AssetDatabase.ImportAsset(RelativizeToProject(projGlb),
                    ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

                Mesh baked = null;
                var assets = AssetDatabase.LoadAllAssetsAtPath(RelativizeToProject(projGlb));
                foreach (var a in assets) { if (a is Mesh m) { baked = m; break; } }

                if (!baked)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RelativizeToProject(projGlb));
                    if (prefab)
                    {
                        var go = UnityEngine.Object.Instantiate(prefab);
                        try
                        {
                            var mf = go.GetComponentInChildren<MeshFilter>();
                            if (mf && mf.sharedMesh) baked = mf.sharedMesh;
                            if (!baked)
                            {
                                var sk = go.GetComponentInChildren<SkinnedMeshRenderer>();
                                if (sk && sk.sharedMesh) baked = sk.sharedMesh;
                            }
                        }
                        finally
                        {
                            UnityEngine.Object.DestroyImmediate(go);
                        }
                    }
                }

                if (!baked)
                {
                    Debug.LogError("ensure a GLTF/GLB importer is installed (com.unity.cloud.gltfast)");
                    return null;
                }

                string meshPath = AssetDatabase.GenerateUniqueAssetPath(
                    Path.Combine(__saveDir, baseName + "-simp.asset"));
                var meshCopy = UnityEngine.Object.Instantiate(baked);
                meshCopy.name = baseName + "-simp";

                if (__recalcNormals) meshCopy.RecalculateNormals();
                meshCopy.RecalculateBounds();

                AssetDatabase.CreateAsset(meshCopy, RelativizeToProject(meshPath));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return AssetDatabase.LoadAssetAtPath<Mesh>(RelativizeToProject(meshPath));
            }
            finally
            {
                SafeDeleteFile(objPath);
                SafeDeleteFile(glbPath);
                SafeDeleteDirIfEmpty(tempDir);
            }
        }
        static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Replace(' ', '_');
        }
        static string RelativizeToProject(string __path)
        {
            __path = __path.Replace('\\', '/');
            string dataPath = Application.dataPath.Replace('\\', '/');
            if (__path.StartsWith(dataPath))
                return "Assets" + __path.Substring(dataPath.Length);
            return __path;
        }
        static void SafeDeleteFile(string path) { try { if (File.Exists(path)) File.Delete(path); } catch { } }
        static void SafeDeleteDirIfEmpty(string __dir)
        {
            try
            {
                if (!Directory.Exists(__dir)) return;
                if (Directory.GetFiles(__dir).Length == 0 && Directory.GetDirectories(__dir).Length == 0)
                    Directory.Delete(__dir);
            }
            catch { }
        }
    }
}
#endif
