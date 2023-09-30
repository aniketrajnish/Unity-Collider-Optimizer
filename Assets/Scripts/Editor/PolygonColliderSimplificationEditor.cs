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
        base.OnInspectorGUI();

        float tolerance = pco.optimizationFactor / 50f;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Optimize Polygon Collider"))        
            pco.OptimizePolygonCollider(tolerance);        

        if (GUILayout.Button("Reset"))        
            pco.Reset();        

        GUILayout.EndHorizontal();

        EditorUtility.SetDirty(target);
       
    }    
}
#endif
