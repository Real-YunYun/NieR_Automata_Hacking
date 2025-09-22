using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using Entities.Status;
using UnityEngine;

namespace Status {

    [System.Serializable]
    public class Marked : StatusInfliction {

        [SerializeField] private int ActivationStackCount = 4;
        private GameObject MarkedEffect;
        
        public Marked() {
            Name = "Marked";
            BenefitType = EStatusType.EST_Negative;

            StatusExpires = true;
            StatusInflictionTime = StatusDuration = 2.5f;

            CanStack = true;
            StatusGameObjectEffect = Resources.Load<GameObject>("Effects/Marked");
        }

        protected internal override void OnStatusInflicted() {
            if (StatusComp.GetInfliction("Status.Weakness") != null) {
                StatusComp.RelieveStatus(this);
                return;
            }
            
            // Reset Status Timer
            StatusInflictionTime = StatusDuration;
            if (MarkedEffect == null) {
                Transform OwnerTransform = StatusComp.Owner.transform;
                MarkedEffect = Object.Instantiate(StatusGameObjectEffect, OwnerTransform);
                MarkedEffect.transform.position = OwnerTransform.position - new Vector3(0, 0.9f, 0);
            }

            // Status is brand new
            if (Stacks == 0) { Stacks++; return; }

            // This was "Re-Inflicted"
            Stacks++;
            if (Stacks >= ActivationStackCount) {
                StatusComp.ApplyStatus("Status.Weakness", HitResult);
                StatusComp.RelieveStatus(this);
            }
        }

        protected internal override void OnStatusRelieved() {
            Object.Destroy(MarkedEffect);
        }
        
    }
    
}