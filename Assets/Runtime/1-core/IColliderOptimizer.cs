using UnityEngine;

namespace UnityColliderOptimizer.Core
{
    public interface IColliderOptimizer
    {
        void Optimize();
        void Reset();
        void LoadSaved(Object __saved);
        Object GetSaved();
    }
}
