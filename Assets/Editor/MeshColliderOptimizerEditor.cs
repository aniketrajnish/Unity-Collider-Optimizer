#if UNITY_EDITOR
using UnityEditor;
using UnityColliderOptimizer.Core.M;
namespace UnityColliderOptimizer.E
{
    [CustomEditor(typeof(MeshColliderOptimizer))]
    public class MeshColliderOptimizerEditor : BaseColliderOptimizerEditor<MeshColliderOptimizer> { }
}
#endif