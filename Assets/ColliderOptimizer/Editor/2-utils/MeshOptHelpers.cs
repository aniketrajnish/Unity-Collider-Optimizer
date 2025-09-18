// MeshOptHelpers.cs
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using ColliderOptimizer.Core;

namespace ColliderOptimizer.Utils
{
    public static class MeshOptHelpers
    {
        public static Mesh CloneMesh(Mesh __m) { var c = Object.Instantiate(__m); c.name = __m.name + "-clone"; return c; }
        public static Vector3 SafeInverse(Vector3 __s)
        {
            float ix = Mathf.Approximately(__s.x, 0f) ? 0f : 1f / __s.x;
            float iy = Mathf.Approximately(__s.y, 0f) ? 0f : 1f / __s.y;
            float iz = Mathf.Approximately(__s.z, 0f) ? 0f : 1f / __s.z;
            return new Vector3(ix, iy, iz);
        }
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
        public static Mesh ScaleCopy(Mesh __m, Vector3 __s)
        {
            if (!__m) return null;
            var c = Object.Instantiate(__m);
            var v = c.vertices;
            for (int i = 0; i < v.Length; i++) v[i] = Vector3.Scale(v[i], __s);
            c.vertices = v;
            c.RecalculateBounds();
            return c;
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