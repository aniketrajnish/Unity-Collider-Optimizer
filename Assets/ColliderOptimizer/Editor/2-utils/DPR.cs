#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace ColliderOptimizer.Utils
{
    public static class DPR
    {
        public static List<Vector2> DPRPts(List<Vector2> __pts, float __tol)
        {
            if (__pts == null || __pts.Count < 3) return __pts;
            int first = 0, last = __pts.Count - 1;
            while (last > first && __pts[first] == __pts[last]) last--;
            var keep = new List<int> { first, last };
            Recurse(__pts, first, last, __tol, keep);
            keep.Sort();
            var res = new List<Vector2>(keep.Count);
            foreach (var i in keep) res.Add(__pts[i]);
            return res;
        }
        static void Recurse(List<Vector2> __pts, int __a, int __b, float __tol, List<int> __keep)
        {
            float maxD = 0f; int iFar = -1;
            for (int i = __a + 1; i < __b; i++)
            {
                float d = PerpDist(__pts[__a], __pts[__b], __pts[i]);
                if (d > maxD) { maxD = d; iFar = i; }
            }
            if (iFar >= 0 && maxD > __tol)
            {
                __keep.Add(iFar);
                Recurse(__pts, __a, iFar, __tol, __keep);
                Recurse(__pts, iFar, __b, __tol, __keep);
            }
        }
        static float PerpDist(Vector2 __a, Vector2 __b, Vector2 __p)
        {
            float denom = Vector2.Distance(__a, __b);
            if (denom <= Mathf.Epsilon) return Vector2.Distance(__a, __p);
            float area2 = Mathf.Abs(Vector3.Cross((Vector3)(__a - __p), (Vector3)(__b - __p)).z);
            return area2 / denom;
        }
    }
}
#endif