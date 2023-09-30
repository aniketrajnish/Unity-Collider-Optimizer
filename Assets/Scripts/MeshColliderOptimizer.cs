using UnityEngine;
using Habrador_Computational_Geometry;
using System.Linq;
using System.Collections;
using UnityEditor;
using System.Diagnostics;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MeshColliderOptimizer : MonoBehaviour
{
    private Mesh initMesh;
    private MeshCollider mCollider;
    public MeshProperties mp;
    public struct MeshProperties
    {
        public HalfEdgeData3.ConnectOppositeEdges connectingMode;
        public float optimizationFactor;
        public MyMesh.MeshStyle meshStyle;        
    }
    private void OnEnable()
    {
        GetInit();
    }
    private void GetInit()
    {
        initMesh = GetComponent<MeshFilter>().sharedMesh;
        mCollider = GetComponent<MeshCollider>();
    }
   
    public void SimplifyMeshCollider(MeshProperties mp)
    {
        //Stopwatch stopwatch = new Stopwatch();

        //stopwatch.Start();

        int edgesToContract = (int)(mp.optimizationFactor * 25f);

        if (mCollider == null)
            mCollider = GetComponent<MeshCollider>();

        if (edgesToContract <= 0)
        {
            mCollider.sharedMesh = initMesh;
            return;
        }
        
        //stopwatch.Stop();
        //print("Time taken for init: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();


        Mesh meshToSimplify = initMesh;
        MyMesh myMeshToSimplify = new MyMesh(meshToSimplify);

        //stopwatch.Stop();
        //print("Time taken for MyMesh: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();

        Normalizer3 normalizer = new Normalizer3(myMeshToSimplify.vertices);
        myMeshToSimplify.vertices = normalizer.Normalize(myMeshToSimplify.vertices);

        //stopwatch.Stop();
        //print("Time taken for Normalizer3: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();

        HalfEdgeData3 myMeshToSimplify_HalfEdge = new HalfEdgeData3(myMeshToSimplify, mp.connectingMode);
        //stopwatch.Stop();
        //print("Time taken for halfedge3: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();
        HalfEdgeData3 mySimplifiedMesh_HalfEdge = MeshSimplification_QEM.Simplify(myMeshToSimplify_HalfEdge, maxEdgesToContract: edgesToContract, maxError: Mathf.Infinity, normalizeTriangles: true);
        //stopwatch.Stop();
        //print("Time taken for QEM: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();
        MyMesh mySimplifiedMesh = mySimplifiedMesh_HalfEdge.ConvertToMyMesh("Simplified Mesh", mp.meshStyle);
        //stopwatch.Stop();
        //print("Time taken for Converstion p1: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();
        mySimplifiedMesh.vertices = normalizer.UnNormalize(mySimplifiedMesh.vertices);
        //stopwatch.Stop();
        //print("Time taken for Converstion p2: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();
        mySimplifiedMesh.vertices = mySimplifiedMesh.vertices.Select(x => x.ToVector3().ToMyVector3()).ToList();
        //stopwatch.Stop();
        //print("Time taken for Converstion p3: " + stopwatch.ElapsedMilliseconds + "ms");
        //stopwatch.Start();
        Mesh unitySimplifiedMesh = mySimplifiedMesh.ConvertToUnityMesh(generateNormals: true, meshName: "Simplified Collider");
        //stopwatch.Stop();
        //print("Time taken for Converstion p4: " + stopwatch.ElapsedMilliseconds + "ms");
        mCollider.sharedMesh = unitySimplifiedMesh;
    }
    public void Reset()
    {
        if (initMesh == null)
            initMesh = GetComponent<MeshCollider>().sharedMesh;

        if (mCollider == null)
            mCollider = GetComponent<MeshCollider>();

        mCollider.sharedMesh = initMesh;
    }
}
