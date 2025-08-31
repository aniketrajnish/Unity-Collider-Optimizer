using System.Collections.Generic;
using UnityEngine;
using UnityColliderOptimizer.Utils;

namespace UnityColliderOptimizer.Core.P
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonColliderOptimizer : BaseColliderOptimizer<PolygonCollider2D, PathData>
    {
        [SerializeField] public PolyOptParams Params = new PolyOptParams();
        [HideInInspector][SerializeField] List<Vector2[]> _authoringPathsSer = new();
        readonly List<List<Vector2>> _authoringPaths = new();
        public override void Optimize()
        {
            EnsureRefs(); if (_tgtColl == null) return;
            BuildAuthoringPaths();
            WritePaths(_tgtColl, _authoringPaths);
            var p = Params ?? (Params = new PolyOptParams());
            var simplified = SimplifyPolys(_authoringPaths, p, transform);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Optimize PolygonCollider2D");
#endif
            WritePaths(_tgtColl, simplified);

            var asset = ScriptableObject.CreateInstance<PathData>();
            asset.Paths = simplified.ConvertAll(path => new Path2D { Pts = path?.ToArray() ?? System.Array.Empty<Vector2>() });
            _saved = asset;
        }
        public override void Reset()
        {
            EnsureRefs(); if (_tgtColl == null) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Reset PolygonCollider2D");
#endif
            BuildAuthoringPaths();
            WritePaths(_tgtColl, _authoringPaths);
        }
        public override void LoadSaved(Object __obj)
        {
            EnsureRefs(); if (_tgtColl == null) return;
            var data = __obj as PathData;
            if (data == null || data.Paths == null) return;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Load Saved PolygonCollider2D");
#endif
            var arrPaths = new List<Vector2[]>(data.Paths.Count);
            for (int i = 0; i < data.Paths.Count; i++)
            {
                var p = data.Paths[i];
                var pts = (p != null && p.Pts != null) ? p.Pts : System.Array.Empty<Vector2>();
                arrPaths.Add((Vector2[])pts.Clone());
            }
            WritePaths(_tgtColl, arrPaths);
            _saved = data;
        }
        static List<List<Vector2>> SimplifyPolys(List<List<Vector2>> __src, PolyOptParams __p, Transform __t = null)
        {
            var result = new List<List<Vector2>>(__src.Count);
            for (int i = 0; i < __src.Count; i++)
            {
                var path = __src[i];
                if (path == null || path.Count < 3) { result.Add(path); continue; }

                float tol = __p.Tolerance;
                if (__p.Mode == ToleranceMode.Relative)
                {
                    var bb = Bounds2D(path);
                    var diag = (bb.max - bb.min).magnitude;
                    tol = diag * __p.Tolerance;
                }
                if (__p.PerPathScaleByBounds)
                {
                    var bb = Bounds2D(path);
                    tol *= (bb.max - bb.min).magnitude;
                }

                var simp = DPR.DPRPts(path, tol);
                result.Add(simp);
            }
            return result;
        }
        static (Vector2 min, Vector2 max) Bounds2D(List<Vector2> __pts)
        {
            var min = __pts[0]; var max = __pts[0];
            for (int i = 1; i < __pts.Count; i++)
            {
                var p = __pts[i];
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }
            return (min, max);
        }
        void BuildAuthoringPaths()
        {
            if (TryBuildFromSprite(_tgtColl, _authoringPaths))
                return;
            if (_authoringPathsSer == null || _authoringPathsSer.Count == 0)
            {
                var cur = ReadPaths(_tgtColl);
                _authoringPathsSer = cur.ConvertAll(p => p.ToArray());
            }
            _authoringPaths.Clear();
            for (int i = 0; i < _authoringPathsSer.Count; i++)
            {
                var arr = _authoringPathsSer[i] ?? System.Array.Empty<Vector2>();
                _authoringPaths.Add(new List<Vector2>(arr));
            }
        }
        static bool TryBuildFromSprite(PolygonCollider2D __c, List<List<Vector2>> __outPaths)
        {
            __outPaths.Clear();
            var sr = __c.GetComponent<SpriteRenderer>();
            var sprite = sr ? sr.sprite : null;
            if (!sprite) return false;

            int shapeCount = sprite.GetPhysicsShapeCount();
            if (shapeCount <= 0) return false;

            var shape = new List<Vector2>(64);
            for (int i = 0; i < shapeCount; i++)
            {
                shape.Clear();
                sprite.GetPhysicsShape(i, shape);
                __outPaths.Add(new List<Vector2>(shape));
            }
            return __outPaths.Count > 0;
        }
        static List<List<Vector2>> ReadPaths(PolygonCollider2D __c)
        {
            var list = new List<List<Vector2>>(__c.pathCount);
            for (int i = 0; i < __c.pathCount; i++) list.Add(new List<Vector2>(__c.GetPath(i)));
            return list;
        }
        static void WritePaths(PolygonCollider2D c, List<List<Vector2>> __paths)
        {
            c.pathCount = __paths.Count;
            for (int i = 0; i < __paths.Count; i++) c.SetPath(i, __paths[i].ToArray());
        }
        static void WritePaths(PolygonCollider2D c, List<Vector2[]> __paths)
        {
            c.pathCount = __paths.Count;
            for (int i = 0; i < __paths.Count; i++) c.SetPath(i, __paths[i] ?? System.Array.Empty<Vector2>());
        }
    }
}