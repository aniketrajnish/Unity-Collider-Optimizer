using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PolygonColliderOptimizer))]
public class PolygonColliderOptimizerEditor : Editor
{
    private PolygonColliderOptimizer pco;
    private void OnEnable()
    {
        pco = (PolygonColliderOptimizer)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Optimize Collider", EditorStyles.boldLabel);

        base.OnInspectorGUI();

        float tolerance = pco.optimizationFactor / 50f;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Optimize Polygon Collider"))        
            pco.OptimizePolygonCollider(tolerance);        

        if (GUILayout.Button("Reset"))        
            pco.Reset();        

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Save Optimized Path"))
        {
            pco.SaveOptimizedPath();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Load Saved Collider Data", EditorStyles.boldLabel);        

        pco.savedPathData = (OptimizedPathData)EditorGUILayout.ObjectField("Saved Path", pco.savedPathData, typeof(OptimizedPathData), false);

        if (GUILayout.Button("Load Saved Path"))
        {
            pco.LoadSavedPath(pco.savedPathData);
        }
    }    
}
#endif
