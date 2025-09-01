#if UNITY_EDITOR
using ColliderOptimizer.Core.M;
using UnityEditor;
using UnityEngine;

namespace ColliderOptimizer.UI
{
    static class MeshColliderOptContextMenu
    {
        static readonly MeshColliderMenuOps MCMO = new();

        [MenuItem("CONTEXT/MeshCollider/Optimize Collider")]
        static void Optimize(MenuCommand __cmd) => MCMO.Optimize((MeshCollider)__cmd.context);

        [MenuItem("CONTEXT/MeshCollider/Reset Collider")]
        static void Reset(MenuCommand __cmd) => MCMO.Reset((MeshCollider)__cmd.context);

        [MenuItem("CONTEXT/MeshCollider/Save Collider")]
        static void Save(MenuCommand __cmd) => MCMO.Save((MeshCollider)__cmd.context);

        [MenuItem("CONTEXT/MeshCollider/Load Collider")]
        static void Load(MenuCommand __cmd) => MCMO.Load((MeshCollider)__cmd.context);
    }
}
#endif