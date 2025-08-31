#if UNITY_EDITOR
using UnityEditor;
using UnityColliderOptimizer.Core.P;

namespace UnityColliderOptimizer.UI
{
    [CustomEditor(typeof(PolygonColliderOptimizer))]
    public class PolygonColliderOptimizerEditor : BaseColliderOptimizerEditor<PolygonColliderOptimizer> { }
}
#endif
