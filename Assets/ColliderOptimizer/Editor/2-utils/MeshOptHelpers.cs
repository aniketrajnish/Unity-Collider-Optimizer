#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using ColliderOptimizer.Core.M;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace ColliderOptimizer.Utils
{
    public static class MeshOptHelpers
    {
        public static Mesh CloneMesh(Mesh __m) { var c = Object.Instantiate(__m); c.name = __m.name + "-clone"; return c; }
#if UNITY_EDITOR
        static bool IsAsset(Object __o)
        {
            if (!__o) return false;
            return !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(__o));
        }
        public static void SafeDestroyRuntime(Object __o)
        {
            if (!__o) return;
            if (IsAsset(__o)) return;
            Object.DestroyImmediate(__o);
        }
#endif
        public static void FlipWindingAllSubmeshes(Mesh __m)
        {
            if (!__m) return;
            int sub = __m.subMeshCount;
            for (int i = 0; i < sub; i++)
            {
                var topo = __m.GetTopology(i);
                if (topo != MeshTopology.Triangles) continue;
                var idx = __m.GetIndices(i);
                for (int t = 0; t < idx.Length; t += 3) { var tmp = idx[t]; idx[t] = idx[t + 1]; idx[t + 1] = tmp; }
                __m.SetIndices(idx, topo, i, false);
            }
        }
        public static void BakeTRSIntoMesh(Mesh __m, Matrix4x4 __trs)
        {
            if (!__m) return;

            var verts = __m.vertices;
            for (int i = 0; i < verts.Length; i++) verts[i] = __trs.MultiplyPoint3x4(verts[i]);
            __m.vertices = verts;

            var n = __m.normals;
            if (n != null && n.Length == verts.Length)
            {
                var nM = __trs.inverse.transpose;
                for (int i = 0; i < n.Length; i++) n[i] = nM.MultiplyVector(n[i]).normalized;
                __m.normals = n;
            }

            __m.RecalculateBounds();
        }
        public static Mesh CombineMeshesFromPrefabHierarchy(GameObject __root)
        {
            if (!__root) return null;
            var mfs = __root.GetComponentsInChildren<MeshFilter>(true);
            var sks = __root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            var items = new List<(Mesh mesh, Matrix4x4 mtx)>();
            for (int i = 0; i < mfs.Length; i++)
            {
                var mf = mfs[i];
                if (mf && mf.sharedMesh) items.Add((mf.sharedMesh, mf.transform.localToWorldMatrix));
            }
            for (int i = 0; i < sks.Length; i++)
            {
                var sk = sks[i];
                if (sk && sk.sharedMesh) items.Add((sk.sharedMesh, sk.transform.localToWorldMatrix));
            }
            if (items.Count == 0) return null;

            int totalVerts = 0;
            int totalIdx = 0;
            for (int it = 0; it < items.Count; it++)
            {
                var m = items[it].mesh;
                totalVerts += m.vertexCount;
                for (int s = 0; s < m.subMeshCount; s++)
                {
                    if (m.GetTopology(s) == MeshTopology.Triangles)
                        totalIdx += m.GetIndices(s).Length;
                }
            }
            if (totalVerts == 0 || totalIdx == 0) return null;

            var verts = new Vector3[totalVerts];
            bool keepNormals = true;
            for (int it = 0; it < items.Count; it++)
            {
                var m = items[it].mesh;
                if (m.normals == null || m.normals.Length != m.vertexCount) { keepNormals = false; break; }
            }
            Vector3[] norms = keepNormals ? new Vector3[totalVerts] : null;
            var indices = new int[totalIdx];

            int vOfs = 0, iOfs = 0;
            for (int it = 0; it < items.Count; it++)
            {
                var (m, trs) = items[it];
                var v = m.vertices;
                for (int i = 0; i < v.Length; i++)
                    verts[vOfs + i] = trs.MultiplyPoint3x4(v[i]);

                if (norms != null)
                {
                    var n = m.normals;
                    var nM = trs.inverse.transpose;
                    for (int i = 0; i < n.Length; i++)
                        norms[vOfs + i] = nM.MultiplyVector(n[i]).normalized;
                }

                bool flip = trs.determinant < 0f;
                for (int s = 0; s < m.subMeshCount; s++)
                {
                    if (m.GetTopology(s) != MeshTopology.Triangles) continue;
                    var idx = m.GetIndices(s);
                    if (flip)
                    {
                        for (int t = 0; t < idx.Length; t += 3)
                        {
                            int tmp = idx[t];
                            idx[t] = idx[t + 1];
                            idx[t + 1] = tmp;
                        }
                    }
                    for (int k = 0; k < idx.Length; k++)
                        indices[iOfs + k] = idx[k] + vOfs;
                    iOfs += idx.Length;
                }
                vOfs += v.Length;
            }

            var combined = new Mesh { name = "combined-gltfpack" };
            combined.indexFormat = (totalVerts > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;
            combined.vertices = verts;
            if (norms != null) combined.normals = norms;
            combined.SetIndices(indices, MeshTopology.Triangles, 0, false);
            combined.RecalculateBounds();
            return combined;
        }
        public static Mesh CombineMeshesFromAssets(Object[] __assets)
        {
            var meshes = new List<Mesh>();
            for (int i = 0; i < __assets.Length; i++) if (__assets[i] is Mesh m) meshes.Add(m);
            if (meshes.Count == 0) return null;

            int totalVerts = 0, totalIdx = 0;
            for (int i = 0; i < meshes.Count; i++)
            {
                var m = meshes[i];
                totalVerts += m.vertexCount;
                for (int s = 0; s < m.subMeshCount; s++)
                    if (m.GetTopology(s) == MeshTopology.Triangles)
                        totalIdx += m.GetIndices(s).Length;
            }
            if (totalVerts == 0 || totalIdx == 0) return null;

            var verts = new Vector3[totalVerts];
            bool keepNormals = true;
            for (int i = 0; i < meshes.Count; i++)
            {
                var m = meshes[i];
                if (m.normals == null || m.normals.Length != m.vertexCount) { keepNormals = false; break; }
            }
            Vector3[] norms = keepNormals ? new Vector3[totalVerts] : null;
            var indices = new int[totalIdx];

            int vOfs = 0, iOfs = 0;
            for (int i = 0; i < meshes.Count; i++)
            {
                var m = meshes[i];
                var v = m.vertices;
                for (int j = 0; j < v.Length; j++) verts[vOfs + j] = v[j];
                if (norms != null)
                {
                    var n = m.normals;
                    for (int j = 0; j < n.Length; j++) norms[vOfs + j] = n[j];
                }
                for (int s = 0; s < m.subMeshCount; s++)
                {
                    if (m.GetTopology(s) != MeshTopology.Triangles) continue;
                    var idx = m.GetIndices(s);
                    for (int k = 0; k < idx.Length; k++) indices[iOfs + k] = idx[k] + vOfs;
                    iOfs += idx.Length;
                }
                vOfs += v.Length;
            }

            var combined = new Mesh { name = "combined-assets" };
            combined.indexFormat = (totalVerts > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;
            combined.vertices = verts;
            if (norms != null) combined.normals = norms;
            combined.SetIndices(indices, MeshTopology.Triangles, 0, false);
            combined.RecalculateBounds();
            return combined;
        }
        public static void ResetMesh(MeshCollider __mc, Mesh __src, MeshOptParams __p)
        {
#if UNITY_EDITOR
            var prev = __mc.sharedMesh;
#endif
            __mc.sharedMesh = null;
#if UNITY_2020_2_OR_NEWER
            __mc.cookingOptions =
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase;
#endif
            __mc.convex = __p != null && __p.Convex;
            __mc.sharedMesh = __src;
#if UNITY_EDITOR
            SafeDestroyRuntime(prev);
#endif
        }
        public static void ApplyMesh(MeshCollider __mc, Mesh __m, MeshOptParams __p)
        {
#if UNITY_EDITOR
            var prev = __mc.sharedMesh;
#endif
            __mc.sharedMesh = null;
#if UNITY_2020_2_OR_NEWER
            __mc.cookingOptions =
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase;
#endif
            __mc.convex = __p != null && __p.Convex;
            __mc.sharedMesh = __m;
#if UNITY_EDITOR
            SafeDestroyRuntime(prev);
#endif
        }
    }
}
