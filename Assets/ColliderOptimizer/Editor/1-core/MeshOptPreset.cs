#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core
{
    [CreateAssetMenu(menuName = "ColliderOptimizer/Mesh Preset")]
    public class MeshOptPreset : ScriptableObject { public MeshOptParams Params = new(); }
}
#endif