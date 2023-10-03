using UnityEngine;
using Habrador_Computational_Geometry;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Diagnostics;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MeshColliderOptimizer : MonoBehaviour
{
    private Mesh initMesh;
    private MeshCollider mCollider;
    public MeshProperties mp;
    [HideInInspector] public Mesh savedMesh;
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
        int edgesToContract = (int)(mp.optimizationFactor * 25f);

        if (mCollider == null)
            mCollider = GetComponent<MeshCollider>();

        if (edgesToContract <= 0)
        {
            mCollider.sharedMesh = initMesh;
            return;
        }      

        Mesh meshToSimplify = initMesh;
        MyMesh myMeshToSimplify = new MyMesh(meshToSimplify);       

        Normalizer3 normalizer = new Normalizer3(myMeshToSimplify.vertices);
        myMeshToSimplify.vertices = normalizer.Normalize(myMeshToSimplify.vertices);       

        HalfEdgeData3 myMeshToSimplify_HalfEdge = new HalfEdgeData3(myMeshToSimplify, mp.connectingMode);        
        HalfEdgeData3 mySimplifiedMesh_HalfEdge = MeshSimplification_QEM.Simplify(myMeshToSimplify_HalfEdge, maxEdgesToContract: edgesToContract, maxError: Mathf.Infinity, normalizeTriangles: true);
        
        MyMesh mySimplifiedMesh = mySimplifiedMesh_HalfEdge.ConvertToMyMesh("Simplified Mesh", mp.meshStyle);
        mySimplifiedMesh.vertices = normalizer.UnNormalize(mySimplifiedMesh.vertices);
        mySimplifiedMesh.vertices = mySimplifiedMesh.vertices.Select(x => x.ToVector3().ToMyVector3()).ToList();

        Mesh unitySimplifiedMesh = mySimplifiedMesh.ConvertToUnityMesh(generateNormals: true, meshName: "Simplified Collider");

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

#if UNITY_EDITOR
    public void SaveOptimizedCollider()
    {
        if (mCollider.sharedMesh != null)
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Optimized Collider", "OptimizedCollider", "asset", "Enter the filename for Optimized Collider:");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(mCollider.sharedMesh, path);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Need an Optimized Collider bauss :3");
        }
    }

    public void LoadSavedMesh(Mesh mesh)
    {
        if (mesh == null || mCollider == null)
        {
            UnityEngine.Debug.LogWarning("Add a path first bauss :3");
            return;
        }

        mCollider.sharedMesh = mesh;
    }
#endif
}
