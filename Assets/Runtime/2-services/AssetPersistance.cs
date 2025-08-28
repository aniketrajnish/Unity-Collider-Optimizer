using UnityEngine;
using UnityColliderOptimizer.Core.P;
namespace UnityColliderOptimizer.Services
{
    public interface IAssetPersistence
    {
        public bool SaveMeshAsset(Mesh __m, string __name);
        public bool SavePathDataAsset(OptimizedPathData __p, string __name);
    }
}
