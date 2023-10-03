using UnityEngine;
using Habrador_Computational_Geometry;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MeshColliderOptimizer))]
public class MeshColliderOptimizerEditor : Editor
{
    private MeshColliderOptimizer mco;
    private void OnEnable()
    {
        mco = (MeshColliderOptimizer)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Optimize Collider", EditorStyles.boldLabel);

        mco.mp.connectingMode = (HalfEdgeData3.ConnectOppositeEdges)EditorGUILayout.EnumPopup("Connecting Mode", mco.mp.connectingMode);
        mco.mp.optimizationFactor = Mathf.Max(0f, EditorGUILayout.FloatField("Optimization Factor", mco.mp.optimizationFactor));
        mco.mp.meshStyle = (MyMesh.MeshStyle)EditorGUILayout.EnumPopup("Mesh Style", mco.mp.meshStyle);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Optimize Mesh Collider"))        
            mco.SimplifyMeshCollider(mco.mp);        

        if (GUILayout.Button("Reset"))        
            mco.Reset();        

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save Optimized Collider"))
        {
            mco.SaveOptimizedCollider();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Load Saved Collider Data", EditorStyles.boldLabel);        

        mco.savedMesh = (Mesh)EditorGUILayout.ObjectField("Saved Mesh", mco.savedMesh, typeof(Mesh), false);

        if (GUILayout.Button("Load Saved Mesh"))
        {
            mco.LoadSavedMesh(mco.savedMesh);
        }
    }   
}
#endif