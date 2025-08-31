using System.Collections.Generic;
using UnityEngine;
namespace UnityColliderOptimizer.Core.P
{
    [System.Serializable]
    public class Path2D { public Vector2[] Pts; }

    [CreateAssetMenu(menuName = "ColliderOptimizer/Path Data")]
    public class PathData : ScriptableObject
    {
        [SerializeField] public List<Path2D> Paths = new();
    }
}
