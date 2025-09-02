#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core
{
    public enum ToleranceMode { World, Relative }
    [System.Serializable]
    public class MeshOptParams
    {
        [Range(0f, 1f)] public float ContractionFactor = .25f;
        public bool RecalcNormals = true;
        public bool BakeScale = true;
        public bool Convex = false;
        public Vector3 LossyScale = Vector3.one;
        public bool Aggressive = false;
        public bool Permissive = false;

        public void CopyFrom(MeshOptParams __other)
        {
            ContractionFactor = __other.ContractionFactor;
            RecalcNormals = __other.RecalcNormals;
            BakeScale = __other.BakeScale;
            Convex = __other.Convex;
            LossyScale = __other.LossyScale;
            Aggressive = __other.Aggressive;
            Permissive = __other.Permissive;
        }
        public void ResetToDefaults() => CopyFrom(new MeshOptParams());
    }
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
