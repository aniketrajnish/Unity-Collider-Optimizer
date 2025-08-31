#if UNITY_EDITOR
using UnityEngine;

namespace UnityColliderOptimizer.Utils
{
    public static class MeshOptHelpers
    {
        public static Mesh CloneMesh(Mesh __m) { var c = Object.Instantiate(__m); c.name = __m.name + " (Clone)"; return c; }
        public static Mesh BakeScaledCopy(Mesh __m, Vector3 __s)
        {
            var c = CloneMesh(__m);
            var v = c.vertices;
            for (int i = 0; i < v.Length; i++) v[i] = Vector3.Scale(v[i], __s);
            c.vertices = v; c.RecalculateBounds(); return c;
        }
        public static void ResetMeshTo(MeshCollider __mc, Mesh __src, MeshOptParams __p)
        {
            __mc.sharedMesh = null;
#if UNITY_2020_2_OR_NEWER
            __mc.cookingOptions =
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase;
#endif
            __mc.convex = __p != null && __p.Convex;
            __mc.sharedMesh = __src;
        }
        public static void ApplyMesh(MeshCollider __mc, Mesh __m, MeshOptParams __p)
        {
            __mc.sharedMesh = null;
#if UNITY_2020_2_OR_NEWER
            __mc.cookingOptions =
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase;
#endif
            __mc.convex = __p != null && __p.Convex;
            __mc.sharedMesh = __m;
        }
    }
}
#endif
