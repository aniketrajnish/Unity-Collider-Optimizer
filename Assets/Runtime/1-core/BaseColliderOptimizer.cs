using UnityEngine;
namespace UnityColliderOptimizer.Core
{
    public abstract class BaseColliderOptimizer<TCollider, TSaved> : MonoBehaviour, IColliderOptimizer
     where TCollider : Component
     where TSaved : Object
    {
        [SerializeField] protected TCollider _tgtColl;
        [SerializeField] protected TSaved _base, _saved;
        [SerializeField] protected bool autoCaptureOnEnable = true;
        protected virtual void OnEnable()
        {
            EnsureRefs();
            if (autoCaptureOnEnable && !_base) Capture();
        }
        protected void EnsureRefs() { if (!_tgtColl) _tgtColl = GetComponent<TCollider>(); }
        public abstract void Optimize();
        public abstract void Reset();
        public abstract void Capture();
        public abstract void LoadSaved(Object __obj);
        public Object GetSaved() => _saved;
    }
}
