#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core
{
    [CreateAssetMenu(menuName = "ColliderOptimizer/Poly Preset")]
    public class PolyOptPreset : ScriptableObject { public PolyOptParams Params = new(); }
}
#endif
