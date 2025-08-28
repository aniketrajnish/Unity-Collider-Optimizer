using System.Collections.Generic;
using UnityEngine;
namespace UnityColliderOptimizer.Core.P
{
    [CreateAssetMenu(menuName = "ColliderOptimizer/Optimized Path Data")]
    public class OptimizedPathData : ScriptableObject
    {
        public List<Vector2[]> Paths = new();
    }
}
