using UnityEngine;
namespace UnityColliderOptimizer.Core
{
    public enum ToleranceMode { World, Relative }
    public enum ConnectOppositeEdges { Precise, Fast, No }
    [System.Serializable]
    public class MeshOptParams
    {
        [Range(0f, 1f)] public float ContractionFactor = .25f;
        public bool RecalcNormals = true;
        public bool BakeScale = true;
        public bool Convex = false;
        public Vector3 LossyScale = Vector3.one;
        public ConnectOppositeEdges Connect = ConnectOppositeEdges.Fast;
    }
    [System.Serializable]
    public class PolyOptParams
    {
        public ToleranceMode Mode = ToleranceMode.World;
        public float Tolerance = .01f;
        public bool PerPathScaleByBounds = false;
    }
}