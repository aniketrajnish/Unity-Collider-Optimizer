#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core.M
{
    [System.Serializable]
    public class MeshOptParams
    {
        [Range(0f, 1f)] public float ContractionFactor = .25f;
        public bool RecalcNormals = true;
        public bool Convex = false;
        public bool Aggressive = true;
        public bool Permissive = false;
        public void CopyFrom(MeshOptParams __other)
        {
            ContractionFactor = __other.ContractionFactor;
            RecalcNormals = __other.RecalcNormals;
            Convex = __other.Convex;
            Aggressive = __other.Aggressive;
            Permissive = __other.Permissive;
        }
        public void ResetToDefaults() => CopyFrom(new MeshOptParams());
    }
}
#endif
