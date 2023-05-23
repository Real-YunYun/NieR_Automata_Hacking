using System;
using UnityEngine;

namespace Items.Threads {
    public enum EThreadBehaviour {
        EventBased,     // OnHit Events, OnFire Events, etc
        Conditional,    // Standing Still for X seconds, then it activates
        Periodic,       // After a (conditional) period of time, they activate
        Instantaneous,  // Movement speed increase, Health increase, etc
        Constant,       // Constantly Active
    }

    [Serializable, DefaultExecutionOrder(2)]
    public abstract class Thread : Item {
        protected Entity Owner => transform.GetComponent<Entity>();
        
        [Header("Thread Parameters")]
        protected EThreadBehaviour Type = EThreadBehaviour.EventBased;

        #region Event Based Parameters
        
        public delegate void OnThreadStartedDelegate();
        public delegate void OnThreadEndedDelegate();
        
        public event OnThreadStartedDelegate OnThreadStarted;
        public event OnThreadEndedDelegate OnThreadEnded;

        protected void Execute_OnThreadStarted() { if (OnThreadStarted != null) OnThreadStarted(); }
        protected void Execute_OnThreadEnded() { if (OnThreadEnded != null) OnThreadEnded(); }

        #endregion
        
        
        protected new virtual void Awake() {
            Stats.Name = "None";
            Stats.Description = "";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;
        }
    }
}
