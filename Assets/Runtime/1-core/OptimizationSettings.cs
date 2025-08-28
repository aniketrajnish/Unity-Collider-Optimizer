using UnityEngine;
namespace UnityColliderOptimizer.Core
{
    [CreateAssetMenu(menuName = "ColliderOptimizer/Global Settings", fileName = "ColliderOptimizerSettings")]
    public class OptimizationSettings : ScriptableObject
    {
        public MeshOptParams DefaultMOP = new MeshOptParams();
        public PolyOptParams DefaultPOP = new PolyOptParams();
        const string K_DEFAULT_PATH = "ColliderOptimizerSettings";
        public static OptimizationSettings Instance
        {
            get
            {
                var loaded = Resources.Load<OptimizationSettings>(K_DEFAULT_PATH);
                if (loaded) return loaded;
                var all = Resources.LoadAll<OptimizationSettings>("");
                return all != null && all.Length > 0 ? all[0] : ScriptableObject.CreateInstance<OptimizationSettings>();
            }
        }
    }
}
