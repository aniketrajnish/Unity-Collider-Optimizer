#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core
{
    public static class OptSettings
    {
        public static MeshOptParams MeshParams => OptProjectState.instance.CurrMesh;
        public static PolyOptParams PolyParams => OptProjectState.instance.CurrPoly;
        public static MeshOptPreset GetActiveMeshPreset() => OptProjectState.instance.ActiveMeshPreset;
        public static PolyOptPreset GetActivePolyPreset() => OptProjectState.instance.ActivePolyPreset;
        public static void SetActiveMeshPreset(MeshOptPreset __p)
        {
            var s = OptProjectState.instance;
            s.ActiveMeshPreset = __p;
            s.SaveProjectSettings(true);
        }
        public static void SetActivePolyPreset(PolyOptPreset __p)
        {
            var s = OptProjectState.instance;
            s.ActivePolyPreset = __p;
            s.SaveProjectSettings(true);
        }
    }
}
#endif