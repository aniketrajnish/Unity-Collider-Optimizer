using UnityEngine;

namespace UnityColliderOptimizer.Core.M
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshCollider))]
    public class MeshColliderOptimizer : BaseColliderOptimizer<MeshCollider, Mesh>
    {
        [SerializeField] public MeshOptParams Params = new MeshOptParams();
        IMeshSimpStrat _strat = new MeshSimpStratLegacy();
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
                Connect = pSrc.Connect,
                LossyScale = transform ? transform.lossyScale : Vector3.one
            };

            var simplified = _strat.Simplify(src, p);
            if (p.Convex && simplified && simplified.triangles != null)
            {
                int triCount = simplified.triangles.Length / 3;
                if (triCount > 255)
                    Debug.LogWarning($"convex MeshCollider will be auto-reduced by Unity, tris={triCount} (>255)", this);
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
            _tgtColl.sharedMesh = null;
            _tgtColl.sharedMesh = m;
            _saved = m;
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
            _tgtColl.convex = __p.Convex;
            _tgtColl.sharedMesh = __m;
        }
    }
}