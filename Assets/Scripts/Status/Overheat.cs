using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Status;
using UnityEngine;

namespace Status {

    public class Overheat : StatusInfliction {
        
        private GameObject BurningEffect;
        private HealthComponent HealthComp;
        
        public Overheat() {
            Name = "Overheat";
            BenefitType = EStatusType.EST_Negative;

            StatusExpires = true;
            StatusInflictionTime = StatusDuration = 5f;

            CanTick = true;
            TickTime = TickDuration = 0.5f;

            CanStack = true;
            StatusGameObjectEffect = Resources.Load<GameObject>("Effects/Marked");
        }

        protected internal override void OnStatusInflicted() {
            StatusInflictionTime = StatusDuration; // Reset Timer
            Stacks++;
            
            // Overheat effect
            HealthComp = StatusComp.Owner.HealthComponent;
        }

        protected internal override void OnStatusTick() {
            if (HealthComp == null) return;
            HealthComp.TakeDamage(Stacks * 0.5f, HitResult);
        }

        protected internal override void OnStatusRelieved() {
            
        }
        
    }
    
}