using UnityEngine;
using UnityColliderOptimizer.Gltfpack;

namespace UnityColliderOptimizer.Core.M
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshCollider))]
    public class MeshColliderOptimizer : BaseColliderOptimizer<MeshCollider, Mesh>
    {
        [SerializeField] public MeshOptParams Params = new MeshOptParams();

        Mesh _srcMesh
        {
            get
            {
                var mf = GetComponent<MeshFilter>();
                if (mf && mf.sharedMesh) return mf.sharedMesh;
                return _tgtColl ? _tgtColl.sharedMesh : null;
            }
        }
        public override void Optimize()
        {
            EnsureRefs(); var src = _srcMesh; if (!src) return;

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Reset MeshCollider to Authoring");
#endif
            ResetColliderTo(src);

            var pSrc = Params ?? (Params = new MeshOptParams());
            var p = new MeshOptParams
            {
                ContractionFactor = pSrc.ContractionFactor,
                RecalcNormals = pSrc.RecalcNormals,
                BakeScale = pSrc.BakeScale,
                Convex = pSrc.Convex,
                LossyScale = transform ? transform.lossyScale : Vector3.one,
                Aggressive = pSrc.Aggressive,
                Permissive = pSrc.Permissive,
            };

            var simplified = SimplifyMesh(src, p);

            if (p.Convex && simplified && simplified.triangles != null)
            {
                int triCount = simplified.triangles.Length / 3;
                if (triCount > 255)
                    Debug.LogWarning($"convex MeshCollider may be auto-reduced by Unity, tris={triCount} (>255)", this);
            }

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Optimize MeshCollider");
#endif
            ApplyToCollider(simplified, p);
            _saved = simplified;
        }
        public override void Reset()
        {
            EnsureRefs(); var src = _srcMesh; if (!src) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Reset MeshCollider");
#endif
            ResetColliderTo(src);
        }
        public override void LoadSaved(Object __obj)
        {
            EnsureRefs(); var m = __obj as Mesh; if (!m) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Load Saved MeshCollider");
#endif
            ApplyToCollider(m, Params ?? new MeshOptParams());
            _saved = m;
        }
        static Mesh SimplifyMesh(Mesh __src, MeshOptParams __p)
        {
            float keep = 1f - Mathf.Clamp01(__p.ContractionFactor);

#if UNITY_EDITOR
            var bakedOrSrc = (__p.BakeScale && __p.LossyScale != Vector3.one)
                ? BakeScaledCopy(__src, __p.LossyScale)
                : __src;

            Mesh outMesh = null;
            try
            {
                outMesh = MeshSimplifyGateway
                            .SimplifyWithGltfpack(
                                bakedOrSrc,
                                keep,
                                __p.RecalcNormals,
                                "Assets/ColliderOptimizer/Simplified"
                            );
            }
            finally
            {
                if (!ReferenceEquals(bakedOrSrc, __src))
                {
#if UNITY_EDITOR
                    DestroyImmediate(bakedOrSrc);
#else
                    Destroy(bakedOrSrc);
#endif
                }
            }
            return outMesh ? outMesh : CloneMesh(__src);
#else
            return CloneMesh(__src);
#endif
        }
        static Mesh CloneMesh(Mesh __m)
        {
            var c = Instantiate(__m);
            c.name = __m.name + " (Clone)";
            return c;
        }
        static Mesh BakeScaledCopy(Mesh __m, Vector3 __scale)
        {
            var c = CloneMesh(__m);
            var v = c.vertices;
            for (int i = 0; i < v.Length; i++) v[i] = Vector3.Scale(v[i], __scale);
            c.vertices = v;
            c.RecalculateBounds();
            return c;
        }

        void ResetColliderTo(Mesh __src)
        {
            _tgtColl.sharedMesh = null;
#if UNITY_2020_2_OR_NEWER
            _tgtColl.cookingOptions =
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase;
#endif
            _tgtColl.convex = Params != null && Params.Convex;
            _tgtColl.sharedMesh = __src;
        }

        void ApplyToCollider(Mesh __m, MeshOptParams __p)
        {
            _tgtColl.sharedMesh = null;
#if UNITY_2020_2_OR_NEWER
            _tgtColl.cookingOptions =
                MeshColliderCookingOptions.EnableMeshCleaning |
                MeshColliderCookingOptions.WeldColocatedVertices |
                MeshColliderCookingOptions.UseFastMidphase;
#endif
            _tgtColl.convex = __p != null && __p.Convex;
            _tgtColl.sharedMesh = __m;
        }
    }
}