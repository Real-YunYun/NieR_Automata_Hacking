using System;
using Entities;
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
        [Header("Thread Parameters")]
        protected EThreadBehaviour Type = EThreadBehaviour.EventBased;
        
        protected new virtual void Awake() {
            Name = "None";
            Description = "";
            Sprite = "Player/UI Images/None";
            Duration = 0f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Type = EThreadBehaviour.EventBased;
            this.enabled = true;
        }

        #region Event Based Parameters
        
        public delegate void OnThreadStartedDelegate();
        public delegate void OnThreadEndedDelegate();
        
        public event OnThreadStartedDelegate OnThreadStarted;
        public event OnThreadEndedDelegate OnThreadEnded;

        protected void Execute_OnThreadStarted() { if (OnThreadStarted != null) OnThreadStarted(); }
        protected void Execute_OnThreadEnded() { if (OnThreadEnded != null) OnThreadEnded(); }

        #endregion
    }
}
