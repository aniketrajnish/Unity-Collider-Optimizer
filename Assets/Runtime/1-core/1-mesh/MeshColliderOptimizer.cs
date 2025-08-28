using UnityEngine;
namespace UnityColliderOptimizer.Core.M
{
    public class MeshColliderOptimizer : BaseColliderOptimizer<MeshCollider, Mesh>
    {
        public MeshOptParams OverrideParams = null;
        IMeshSimpStrat _strat = new MeshSimpStratLegacy();
        Mesh _srcMesh => GetComponent<MeshFilter>()?.sharedMesh;
        OptimizationSettings _settings => OptimizationSettings.Instance;
        public override void Optimize()
        {
            EnsureRefs();
            var src = _srcMesh;
            if (!src) return;

            var pGlobal = OverrideParams ?? _settings.DefaultMOP;
            var p = new MeshOptParams
            {
                ContractionFactor = pGlobal.ContractionFactor,
                RecalcNormals = pGlobal.RecalcNormals,
                BakeScale = pGlobal.BakeScale,
                Convex = pGlobal.Convex,
                Connect = pGlobal.Connect,
                LossyScale = transform ? transform.lossyScale : Vector3.one
            };

            var simplified = _strat.Simplify(src, p);
            ApplyToCollider(simplified, p);
            _saved = simplified;
        }
        public override void Reset()
        {
            EnsureRefs();
            if (!_base) { _tgtColl.sharedMesh = _srcMesh; return; }
            _tgtColl.sharedMesh = null;
            _tgtColl.sharedMesh = _base;
        }
        public override void Capture()
        {
            EnsureRefs();
            var src = _srcMesh;
            if (!src) return;
            _base = Instantiate(src);
            _base.name = src.name + " (Baseline)";
        }
        public override void LoadSaved(UnityEngine.Object __obj)
        {
            EnsureRefs();
            var m = __obj as Mesh;
            if (!m) return;
            _tgtColl.sharedMesh = null;
            _tgtColl.sharedMesh = m;
            _saved = m;
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
