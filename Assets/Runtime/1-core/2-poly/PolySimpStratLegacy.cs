using System.Collections.Generic;
using UnityColliderOptimizer.Utils;
using UnityEngine;
namespace UnityColliderOptimizer.Core.P
{
    public class PolySimpStratLegacy : IPolySimpStrat
    {
        public List<List<Vector2>> Simplify(List<List<Vector2>> __src, PolyOptParams __p, Transform __t = null)
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
    }
}
