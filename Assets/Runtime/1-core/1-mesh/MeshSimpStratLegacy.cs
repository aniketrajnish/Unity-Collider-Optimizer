using Habrador_Computational_Geometry;
using UnityEngine;
using HConnect = Habrador_Computational_Geometry.HalfEdgeData3.ConnectOppositeEdges;

namespace UnityColliderOptimizer.Core.M
{
    public class MeshSimpStratLegacy : IMeshSimpStrat
    {
        public Mesh Simplify(Mesh __src, MeshOptParams __p)
        {
            var src = (__p.BakeScale && __p.LossyScale != Vector3.one)
                ? BakeScaledCopy(__src, __p.LossyScale)
                : __src;

            var my = new MyMesh(src);
            var norm = new Normalizer3(my.vertices);
            my.vertices = norm.Normalize(my.vertices);

            var he = new HalfEdgeData3(my, Map(__p.Connect));

            int edgesApprox = Mathf.Max(1, he.faces.Count * 3 / 2);
            int e2c = Mathf.Clamp(
                Mathf.RoundToInt(Mathf.Clamp01(__p.ContractionFactor) * edgesApprox),
                0, edgesApprox
            );
            if (e2c == 0) return CloneMesh(src);

            var heOut = MeshSimplification_QEM.Simplify(
                he,
                maxEdgesToContract: e2c,
                maxError: Mathf.Infinity,
                normalizeTriangles: true
            );

            var outMy = heOut.ConvertToMyMesh("Simplified Mesh", MyMesh.MeshStyle.HardEdges);
            outMy.vertices = norm.UnNormalize(outMy.vertices);

            var outUnity = outMy.ConvertToUnityMesh(__p.RecalcNormals, "Simplified Collider");
            outUnity.RecalculateBounds();
            return outUnity;
        }
        static HConnect Map(ConnectOppositeEdges m) => m switch
        {
            ConnectOppositeEdges.Precise => HConnect.Precise,
            ConnectOppositeEdges.Fast => HConnect.Fast,
            _ => HConnect.No
        };
        static Mesh CloneMesh(Mesh __m)
        {
            var c = Object.Instantiate(__m);
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
    }
}
