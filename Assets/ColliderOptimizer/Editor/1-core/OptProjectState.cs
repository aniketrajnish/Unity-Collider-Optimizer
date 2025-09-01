#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ColliderOptimizer.Core
{
    [FilePath("ProjectSettings/ColliderOptimizerSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class OptProjectState : ScriptableSingleton<OptProjectState>
    {
        public MeshOptParams MeshParams = new();
        public PolyOptParams PolyParams = new();
        public MeshOptPreset ActiveMeshPreset;
        public PolyOptPreset ActivePolyPreset;
        public void SaveProjectSettings(bool __asTxt = true) => Save(__asTxt);
        void OnDisable() => Save(true);
        public MeshOptParams CurrMesh => ActiveMeshPreset ? ActiveMeshPreset.Params : MeshParams;
        public PolyOptParams CurrPoly => ActivePolyPreset ? ActivePolyPreset.Params : PolyParams;
    }
}
#endif
