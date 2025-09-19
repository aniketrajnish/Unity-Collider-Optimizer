#if UNITY_EDITOR
using System.Globalization;
using System.IO;
using UnityEngine;

namespace ColliderOptimizer.Utils
{
    static class ObjExporter
    {
        public static string WriteOBJ(Mesh __mesh, string __path)
        {
            using var sw = new StreamWriter(__path);
            var inv = CultureInfo.InvariantCulture;
            var v = __mesh.vertices; var n = __mesh.normals; var uv = __mesh.uv;

            sw.WriteLine("# glb -> obj");
            for (int i = 0; i < v.Length; i++) sw.WriteLine($"v {v[i].x.ToString(inv)} {v[i].y.ToString(inv)} {v[i].z.ToString(inv)}");
            if (n != null && n.Length == v.Length) for (int i = 0; i < n.Length; i++) sw.WriteLine($"vn {n[i].x.ToString(inv)} {n[i].y.ToString(inv)} {n[i].z.ToString(inv)}");
            if (uv != null && uv.Length == v.Length) for (int i = 0; i < uv.Length; i++) sw.WriteLine($"vt {uv[i].x.ToString(inv)} {uv[i].y.ToString(inv)}");

            bool hasUV = uv != null && uv.Length == v.Length;
            bool hasN = n != null && n.Length == v.Length;
            for (int s = 0; s < __mesh.subMeshCount; s++)
            {
                if (__mesh.GetTopology(s) != MeshTopology.Triangles) continue;
                var tris = __mesh.GetTriangles(s);
                for (int i = 0; i < tris.Length; i += 3)
                {
                    int a = tris[i] + 1, b = tris[i + 1] + 1, c = tris[i + 2] + 1;
                    if (hasUV && hasN) sw.WriteLine($"f {a}/{a}/{a} {b}/{b}/{b} {c}/{c}/{c}");
                    else if (hasUV) sw.WriteLine($"f {a}/{a} {b}/{b} {c}/{c}");
                    else if (hasN) sw.WriteLine($"f {a}//{a} {b}//{b} {c}//{c}");
                    else sw.WriteLine($"f {a} {b} {c}");
                }
            }
            return __path;
        }
    }
}
#endif