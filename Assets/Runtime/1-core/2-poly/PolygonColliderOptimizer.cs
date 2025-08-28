using System.Collections.Generic;
using UnityEngine;

namespace UnityColliderOptimizer.Core.P
{
    public class PolygonColliderOptimizer : BaseColliderOptimizer<PolygonCollider2D, OptimizedPathData>
    {
        public PolyOptParams OverrideParams = null;
        IPolySimpStrat _strat = new PolySimpStratLegacy();

        readonly List<List<Vector2>> _baselinePaths = new();
        OptimizationSettings _settings => OptimizationSettings.Instance;
        public override void Optimize()
        {
            EnsureRefs();
            if (_tgtColl == null) return;

            var current = ReadPaths(_tgtColl);
            var p = OverrideParams ?? _settings.DefaultPOP;
            var simplified = _strat.Simplify(current, p, transform);

            WritePaths(_tgtColl, simplified);

            var asset = ScriptableObject.CreateInstance<OptimizedPathData>();
            asset.Paths = simplified.ConvertAll(path => path.ToArray());
            _saved = asset;
        }
        public override void Reset()
        {
            EnsureRefs();
            if (_tgtColl == null || _baselinePaths.Count == 0) return;
            WritePaths(_tgtColl, _baselinePaths);
        }
        public override void Capture()
        {
            EnsureRefs();
            if (_tgtColl == null) return;

            _baselinePaths.Clear();
            var current = ReadPaths(_tgtColl);
            _baselinePaths.AddRange(Clone(current));
        }
        public override void LoadSaved(Object __obj)
        {
            EnsureRefs();
            if (_tgtColl == null) return;

            var data = __obj as OptimizedPathData;
            if (data == null || data.Paths == null) return;

            WritePaths(_tgtColl, data.Paths);
            _saved = data;
        }
        static List<List<Vector2>> ReadPaths(PolygonCollider2D __coll)
        {
            var list = new List<List<Vector2>>(__coll.pathCount);
            for (int i = 0; i < __coll.pathCount; i++)
                list.Add(new List<Vector2>(__coll.GetPath(i)));
            return list;
        }
        static void WritePaths(PolygonCollider2D __coll, List<List<Vector2>> __paths)
        {
            __coll.pathCount = __paths.Count;
            for (int i = 0; i < __paths.Count; i++)
                __coll.SetPath(i, __paths[i].ToArray());
        }
        static void WritePaths(PolygonCollider2D __coll, List<Vector2[]> __paths)
        {
            __coll.pathCount = __paths.Count;
            for (int i = 0; i < __paths.Count; i++)
            {
                var arr = __paths[i] ?? System.Array.Empty<Vector2>();
                __coll.SetPath(i, arr);
            }
        }
        static List<List<Vector2>> Clone(List<List<Vector2>> __src)
        {
            var res = new List<List<Vector2>>(__src.Count);
            foreach (var p in __src) res.Add(new List<Vector2>(p));
            return res;
        }
    }
}
