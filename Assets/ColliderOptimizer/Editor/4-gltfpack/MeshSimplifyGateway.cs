#if UNITY_EDITOR
using System;
using System.IO;
using ColliderOptimizer.Utils;
using UnityEditor;
using UnityEngine;

namespace ColliderOptimizer.Gltfpack
{
    public static class MeshSimplifyGateway
    {
        public static Mesh SimplifyWithGltfpack(
           Mesh __src, float __keepRatio, bool __recalcNormals,
           string __saveDir = "Assets/ColliderOptimizer/Editor/5-opt-out",
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

                var relGlb = RelativizeToProject(projGlb);

                AssetDatabase.ImportAsset(relGlb,
                    ImportAssetOptions.ForceSynchronousImport |
                    ImportAssetOptions.ImportRecursive |
                    ImportAssetOptions.ForceUpdate);

                GameObject prefab = null;
                UnityEngine.Object[] allAssets = null;
                for (int tries = 0; tries < 69; tries++)
                {
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(relGlb);
                    allAssets = AssetDatabase.LoadAllAssetsAtPath(relGlb);

                    if (prefab != null || (allAssets != null && allAssets.Length > 0))
                        break;

                    System.Threading.Thread.Sleep(15);

                    AssetDatabase.ImportAsset(relGlb, ImportAssetOptions.ForceUpdate);
                }

                if (prefab == null && (allAssets == null || allAssets.Length == 0))
                {
                    Debug.LogError(
                        "glb import produced no loadable assets: " + relGlb +
                        "\nmake sure a glb importer (e.g: com.unity.formats.glTF) is installed");
                    return null;
                }

                Mesh baked = null;
                bool usedCombinedBuilder = false;
                Matrix4x4 childRelMtx = Matrix4x4.identity;

                if (prefab)
                {
                    var go = UnityEngine.Object.Instantiate(prefab);
                    try
                    {
                        baked = MeshOptHelpers.CombineMeshesFromPrefabHierarchy(go);
                        usedCombinedBuilder = baked != null;

                        if (!baked)
                        {
                            var mf = go.GetComponentInChildren<MeshFilter>(true);
                            if (mf && mf.sharedMesh)
                            {
                                baked = mf.sharedMesh;
                                childRelMtx = mf.transform.localToWorldMatrix;
                            }
                            else
                            {
                                var sk = go.GetComponentInChildren<SkinnedMeshRenderer>(true);
                                if (sk && sk.sharedMesh)
                                {
                                    baked = sk.sharedMesh;
                                    childRelMtx = sk.transform.localToWorldMatrix;
                                }
                            }
                        }
                    }
                    finally
                    {
                        UnityEngine.Object.DestroyImmediate(go);
                    }
                }
                else
                {
                    var assets = allAssets ?? AssetDatabase.LoadAllAssetsAtPath(relGlb);
                    baked = MeshOptHelpers.CombineMeshesFromAssets(assets);
                    usedCombinedBuilder = baked != null;

                    if (!baked)
                    {
                        foreach (var a in assets) { if (a is Mesh m) { baked = m; break; } }
                        childRelMtx = Matrix4x4.identity;
                    }
                }

                if (!baked) return null;

                string meshPath = AssetDatabase.GenerateUniqueAssetPath(
                    Path.Combine(__saveDir, baseName + "-simp.asset"));

                Mesh meshCopy;
                if (usedCombinedBuilder)
                {
                    meshCopy = baked;
                    meshCopy.name = baseName + "-simp";
                }
                else
                {
                    meshCopy = UnityEngine.Object.Instantiate(baked);
                    meshCopy.name = baseName + "-simp";
                    MeshOptHelpers.BakeTRSIntoMesh(meshCopy, childRelMtx);
                    if (childRelMtx.determinant < 0f) MeshOptHelpers.FlipWindingAllSubmeshes(meshCopy);
                }

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
        static string SanitizeFileName(string __name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                __name = __name.Replace(c, '_');
            return __name.Replace(' ', '_');
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