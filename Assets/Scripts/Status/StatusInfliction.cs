using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Status {
    
    [System.Serializable]
    public enum EStatusType { EST_Positive, EST_Negative }
    
    // The base class of Status Effects in NA: Hacking
    [System.Serializable]
    public abstract class StatusInfliction {

        [Header("Status Parameters")]
        [SerializeField] protected internal string Name = "";
        [SerializeField] protected internal EStatusType BenefitType;
        
        [Header("Components")] 
        [SerializeField, Tooltip("Which StatusComponent is this Status a part of.")]
        protected internal StatusComponent StatusComp;
        protected internal HitResult HitResult;

        #region Durations and Expirations

        [SerializeField] protected internal float StatusInflictionTime;
        
        [SerializeField, Min(0), Tooltip("How long will this Status last for")]
        protected internal float StatusDuration;

        [SerializeField, Tooltip("Does this Status ever expire")]
        protected internal bool StatusExpires;
        
        [SerializeField] protected internal GameObject StatusGameObjectEffect;
        
        #endregion
        
        #region Stacking Capabilities

        [Header("Stacking Parameters")]
        [SerializeField, Tooltip("If this Status can stack on itself")]
        protected internal bool CanStack;
        
        [SerializeField, Min(0), Tooltip("How many stacks does this Status have currently. (Can be used for \"stacking\" status'")]
        protected internal int Stacks;
        
        [SerializeField, Min(0), Tooltip("What is the max stacks this Status can have.")]
        protected internal int MaxStacks;

        #endregion

        #region Ticking Capabilities
        
        [Header("Ticking Parameters")]
        [SerializeField] protected internal float TickTime;
        
        [SerializeField, Tooltip("Can this Status have a affect as it's applied.")]
        protected internal bool CanTick;

        [SerializeField, Min(0), Tooltip("How long between ticks should this affect take place.")]
        protected internal float TickDuration;

        #endregion

        #region Event Observers

        protected internal virtual void OnStatusInflicted() { }

        protected internal virtual void OnStatusTick() { }
        protected internal virtual void OnStatusRelieved() { }

        #endregion

        public string GetStatusDetails() {
            return Name + " | " + 
                   (BenefitType == 0 ? "Positive" : "Negative") + " | " +
                   "Dur: " + StatusInflictionTime + "s / " + StatusDuration + "s | " +
                   "Tick: " + TickTime + "s / " + TickDuration + " | " +
                   "Stacks: " + Stacks + " / " + MaxStacks;
        }

    }
    
}