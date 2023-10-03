using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PolygonCollider2D))]
[ExecuteInEditMode]
public class PolygonColliderOptimizer : MonoBehaviour
{
    [Min(0f)] public float optimizationFactor = 0;
    [HideInInspector] public OptimizedPathData savedPathData;

    private PolygonCollider2D pCollider;
    private List<List<Vector2>> initPaths = new List<List<Vector2>>();    
    private void OnEnable()
    {
        GetInitPaths();
    }
    private void GetInitPaths()
    {
        pCollider = GetComponent<PolygonCollider2D>();
        for (int i = 0; i < pCollider.pathCount; i++)
        {
            List<Vector2> path = new List<Vector2>(pCollider.GetPath(i));
            initPaths.Add(path);
        }
    }
    public void OptimizePolygonCollider(float _tolerance)
    {
        float _toleranceNormalized = _tolerance/initPaths.Count;

        if (pCollider == null)
            GetInitPaths();
        
        if (_toleranceNormalized == 0)
        {
             for (int i = 0; i < initPaths.Count; i++)
             {
                 List<Vector2> path = initPaths[i];
                 pCollider.SetPath(i, path.ToArray());
             }
            return;
        }

        for (int i = 0; i < initPaths.Count; i++)
        {
            List<Vector2> path = initPaths[i];
            path = DouglasPeuckerReduction.DouglasPeuckerReductionPoints(path, _toleranceNormalized);
            pCollider.SetPath(i, path.ToArray());

        }
    }
    public void Reset()
    {
        OptimizePolygonCollider(0);
    }
#if UNITY_EDITOR
    public void SaveOptimizedPath()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Optimized Path", "OptimizedPath", "asset", "Enter the filename for Optimized path:");
        if (!string.IsNullOrEmpty(path))
        {
            OptimizedPathData pathData = ScriptableObject.CreateInstance<OptimizedPathData>();
            pathData.paths = new List<List<Vector2>>();

            List<List<Vector2>> currentPaths = new List<List<Vector2>>();
            for (int i = 0; i < pCollider.pathCount; i++)
            {
                List<Vector2> p = new List<Vector2>(pCollider.GetPath(i));
                currentPaths.Add(p);
            }
            pathData.paths = currentPaths;

            AssetDatabase.CreateAsset(pathData, path);
            AssetDatabase.SaveAssets();
        }
    }
    public void LoadSavedPath(OptimizedPathData pathData)
    {
        if (pathData == null || pCollider == null)
        {
            Debug.LogWarning("Add a path first bauss :3");
            return;
        }

        pCollider.pathCount = pathData.paths.Count;

        for (int i = 0; i < pathData.paths.Count; i++)        
            pCollider.SetPath(i, pathData.paths[i].ToArray());
        
    }
#endif
}

