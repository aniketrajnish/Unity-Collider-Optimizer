#if UNITY_EDITOR
using UnityEditor;
using UnityColliderOptimizer.Core.P;
namespace UnityColliderOptimizer.E
{
    [CustomEditor(typeof(PolygonColliderOptimizer))]
    public class PolygonColliderOptimizerEditor : BaseColliderOptimizerEditor<PolygonColliderOptimizer> { }
}
#endif
