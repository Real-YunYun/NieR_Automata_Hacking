using UnityEngine;
using Unity.Collections;
using Entities;

namespace Items.Threads.EventBased {
    public abstract class Proc : EventBased {
        [Header("Proc Parameters")] 
        [Tooltip("This is a normalized (0.0 - 1.0) probable chance of this Thread happening, this will occur when Random.value <= ChanceToProc")]
        [SerializeField, ReadOnly] protected float ChanceToProc = 0.0f;

        /// Method to handle "OnHit" events
        protected abstract void OnHit(Entity HitEntity);

        /// Attempts to 'Proc', when succeeds, calls 'OnProc'
        protected abstract void TryProc();

        /// Handles how this Thread works. (Also should call 'OnThreadStarted' if intended)
        protected abstract void OnProc();

        /// After the "OnProc" method was called how to revert it. (Also should call 'OnThreadEnded' if intended)
        protected virtual void OnReset() { }
    }
}
