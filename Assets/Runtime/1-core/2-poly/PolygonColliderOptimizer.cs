using System.Collections.Generic;
using UnityEngine;

namespace UnityColliderOptimizer.Core.P
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonColliderOptimizer : BaseColliderOptimizer<PolygonCollider2D, PathData>
    {
        [SerializeField] public PolyOptParams Params = new PolyOptParams();
        IPolySimpStrat _strat = new PolySimpStratLegacy();
        [HideInInspector][SerializeField] List<Vector2[]> _authoringPathsSer = new();
        readonly List<List<Vector2>> _authoringPaths = new();
        public override void Optimize()
        {
            EnsureRefs(); if (_tgtColl == null) return;
            BuildAuthoringPaths();
            WritePaths(_tgtColl, _authoringPaths);
            var p = Params ?? (Params = new PolyOptParams());
            var simplified = _strat.Simplify(_authoringPaths, p, transform);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_tgtColl, "Optimize PolygonCollider2D");
#endif
            WritePaths(_tgtColl, simplified);

            var asset = ScriptableObject.CreateInstance<PathData>();
            asset.Paths = simplified.ConvertAll(path => new Path2D
            {
                Pts = path != null ? path.ToArray() : System.Array.Empty<Vector2>()
            });
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
