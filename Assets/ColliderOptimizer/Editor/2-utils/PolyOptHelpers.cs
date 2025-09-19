#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using ColliderOptimizer.Core;

namespace ColliderOptimizer.Utils
{
    public static class PolyOptHelpers
    {
        public static List<List<Vector2>> BuildAuthoringPaths(PolygonCollider2D __pc)
        {
            var outPaths = new List<List<Vector2>>();
            var sr = __pc.GetComponent<SpriteRenderer>();
            var sprite = sr ? sr.sprite : null;
            if (sprite && sprite.GetPhysicsShapeCount() > 0)
            {
                var shape = new List<Vector2>(64);
                for (int i = 0; i < sprite.GetPhysicsShapeCount(); i++)
                {
                    shape.Clear(); sprite.GetPhysicsShape(i, shape);
                    outPaths.Add(new List<Vector2>(shape));
                }
                return outPaths;
            }
            for (int i = 0; i < __pc.pathCount; i++)
                outPaths.Add(new List<Vector2>(__pc.GetPath(i)));
            return outPaths;
        }
        public static List<List<Vector2>> ReadPaths(PolygonCollider2D __pc)
        {
            var list = new List<List<Vector2>>(__pc.pathCount);
            for (int i = 0; i < __pc.pathCount; i++) list.Add(new List<Vector2>(__pc.GetPath(i)));
            return list;
        }
        public static void WritePaths(PolygonCollider2D pc, List<List<Vector2>> paths)
        {
            pc.pathCount = paths.Count;
            for (int i = 0; i < paths.Count; i++) pc.SetPath(i, paths[i].ToArray());
        }
        public static (Vector2 min, Vector2 max) Bounds2D(List<Vector2> __pts)
        {
            var min = __pts[0]; var max = __pts[0];
            for (int i = 1; i < __pts.Count; i++) { var p = __pts[i]; min = Vector2.Min(min, p); max = Vector2.Max(max, p); }
            return (min, max);
        }
        public static List<List<Vector2>> SimplifyPolys(List<List<Vector2>> __src, PolyOptParams __p)
        {
            var result = new List<List<Vector2>>(__src.Count);

            for (int i = 0; i < __src.Count; i++)
            {
                var path = __src[i];
                if (path == null || path.Count < 3) { result.Add(path); continue; }

                float tol = Mathf.Max(0f, __p.Tolerance);
                var bb = Bounds2D(path);
                float diag = (bb.max - bb.min).magnitude;

                if (__p.Mode == ToleranceMode.Relative)
                    tol = diag * tol;

                if (__p.PerPathScaleByBounds && __p.Mode == ToleranceMode.World)
                    tol *= diag;

                if (tol <= Mathf.Epsilon) { result.Add(path); continue; }

                result.Add(RDP.RDPPts(path, tol));
            }

            return result;
        }
    }
}
#endif
