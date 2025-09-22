using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Projectiles;
using UnityEngine;
using Entities.Status;

namespace Status {

    [System.Serializable]
    public class DataLeak : StatusInfliction {
        
        private List<GameObject> DataLeakEffect = new List<GameObject>();
        private HealthComponent HealthComp;
        
        public DataLeak() {
            Name = "Data Leak";
            BenefitType = EStatusType.EST_Negative;

            StatusExpires = true;
            StatusInflictionTime = StatusDuration = 10f;

            CanTick = true;
            TickTime = TickDuration = 0.5f;

            CanStack = true;
            StatusGameObjectEffect = Resources.Load<GameObject>("Effects/Data Leak");
        }

        protected internal override void OnStatusInflicted() {
            StatusInflictionTime = StatusDuration;
            Stacks++;
            StatusComp.Owner.TryGetComponent(out HealthComp);
            if (DataLeakEffect.Count > 4) return;
            DataLeakEffect.Add(Object.Instantiate(StatusGameObjectEffect, StatusComp.Owner.transform));
        }

        protected internal override void OnStatusTick() {
            if (HealthComp == null) return;
            HealthComp.TakeDamage(Stacks * 0.75f, HitResult);
        }

        protected internal override void OnStatusRelieved() {
            foreach (GameObject D in DataLeakEffect) {
                Object.Destroy(D);
            }
            
            DataLeakEffect.Clear();
        }
    }
    
}