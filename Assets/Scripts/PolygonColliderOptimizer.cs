using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
[ExecuteInEditMode]
public class PolygonColliderOptimizer : MonoBehaviour
{
    [Min(0f)] public float optimizationFactor = 0;
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
}

