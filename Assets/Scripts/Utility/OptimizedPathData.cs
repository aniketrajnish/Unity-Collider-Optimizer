using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Optimized Path Data")]
public class OptimizedPathData : ScriptableObject
{
    public List<List<Vector2>> paths;
}