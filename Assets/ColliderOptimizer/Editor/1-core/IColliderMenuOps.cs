#if UNITY_EDITOR
using UnityEngine;

namespace ColliderOptimizer.Core
{
    interface IColliderMenuOps<TCollider> where TCollider : Component
    {
        void Optimize(TCollider __c);
        void Reset(TCollider __c);
        void Save(TCollider __c);
        void Load(TCollider __c);
    }
}
#endif