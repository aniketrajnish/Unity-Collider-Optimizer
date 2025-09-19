#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core.P
{
    public enum ToleranceMode { World, Relative }
    [System.Serializable]
    public class PolyOptParams
    {
        public ToleranceMode Mode = ToleranceMode.World;
        public float Tolerance = .01f;
        public bool PerPathScaleByBounds = false;

        public void CopyFrom(PolyOptParams __other)
        {
            Mode = __other.Mode;
            Tolerance = __other.Tolerance;
            PerPathScaleByBounds = __other.PerPathScaleByBounds;
        }
        public void ResetToDefaults() => CopyFrom(new PolyOptParams());
    }
}
#endif
