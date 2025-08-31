#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityColliderOptimizer.Utils;

namespace UnityColliderOptimizer.Gltfpack
{
    public static class MeshSimplifyGateway
    {
        public static Mesh SimplifyWithGltfpack(Mesh __src, float __keepRatio, bool __recalcNormals,
                                                string __saveDir = "Assets/ColliderOptimizer/Simplified")
        {
            if (!__src) return null;
            __keepRatio = Mathf.Clamp01(__keepRatio);

            string tempDir = Path.Combine(Path.GetTempPath(), "gltfpack_unity");
            Directory.CreateDirectory(tempDir);
            string baseName = (string.IsNullOrEmpty(__src.name) ? "Mesh" : __src.name).Replace(' ', '_');
            string objPath = Path.Combine(tempDir, baseName + ".obj");
            string glbPath = Path.Combine(tempDir, baseName + "_simp.glb");
            ObjExporter.WriteOBJ(__src, objPath);

            if (!GltfpackRunner.Run(objPath, glbPath, __keepRatio)) return null;

            Directory.CreateDirectory(__saveDir);
            string projGlb = Path.Combine(__saveDir, baseName + "_simp.glb");
            File.Copy(glbPath, projGlb, true);
            AssetDatabase.ImportAsset(projGlb, ImportAssetOptions.ForceSynchronousImport);

            Mesh baked = null;
            var assets = AssetDatabase.LoadAllAssetsAtPath(projGlb);
            foreach (var a in assets) { if (a is Mesh m) { baked = m; break; } }
            if (!baked)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(projGlb);
                if (prefab)
                {
                    var go = Object.Instantiate(prefab);
                    var mf = go.GetComponentInChildren<MeshFilter>();
                    if (mf && mf.sharedMesh) baked = mf.sharedMesh;
                    else
                    {
                        var sk = go.GetComponentInChildren<SkinnedMeshRenderer>();
                        if (sk && sk.sharedMesh) baked = sk.sharedMesh;
                    }
                    Object.DestroyImmediate(go);
                }
            }
            if (!baked) { Debug.LogError("no mesh found in GLB import: " + projGlb); return null; }

            if (__recalcNormals) baked.RecalculateNormals();
            baked.RecalculateBounds();
            baked.indexFormat = (baked.vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;

            string meshPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(__saveDir, baseName + "_Simplified.asset"));
            var meshCopy = Object.Instantiate(baked);
            meshCopy.name = baseName + "_Simplified";
            AssetDatabase.CreateAsset(meshCopy, meshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        }
    }
}
#endif
