#if UNITY_EDITOR
using ColliderOptimizer.Core.P;
using UnityEditor;
using UnityEngine;

namespace ColliderOptimizer.UI
{
    static class PolygonColliderOptContextMenu
    {
        static readonly PolygonColliderMenuOps PCMO = new();

        [MenuItem("CONTEXT/PolygonCollider2D/Optimize Collider")]
        static void Optimize(MenuCommand __cmd) => PCMO.Optimize((PolygonCollider2D)__cmd.context);

        [MenuItem("CONTEXT/PolygonCollider2D/Reset Collider")]
        static void Reset(MenuCommand __cmd) => PCMO.Reset((PolygonCollider2D)__cmd.context);

        [MenuItem("CONTEXT/PolygonCollider2D/Save Collider")]
        static void Save(MenuCommand __cmd) => PCMO.Save((PolygonCollider2D)__cmd.context);

        [MenuItem("CONTEXT/PolygonCollider2D/Load Collider")]
        static void Load(MenuCommand __cmd) => PCMO.Load((PolygonCollider2D)__cmd.context);
    }
}
#endif
