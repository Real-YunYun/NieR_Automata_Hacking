using System.Collections;
using System.Collections.Generic;
using Entities.Status;
using UnityEngine;

namespace Status {

    [System.Serializable]
    public class Weakness : StatusInfliction {

        [SerializeField] private GameObject SpawnedEffect;

        public Weakness() {
            Name = "Weakness";
            BenefitType = EStatusType.EST_Negative;

            StatusExpires = true;
            StatusInflictionTime = StatusDuration = 10.0f;

            CanStack = false;

            StatusGameObjectEffect = Resources.Load<GameObject>("Effects/Weakness");
        }

        protected internal override void OnStatusInflicted() {
            StatusInflictionTime = StatusDuration;
            if (SpawnedEffect == null) SpawnedEffect = Object.Instantiate(StatusGameObjectEffect, StatusComp.Owner.transform);
        }

        protected internal override void OnStatusRelieved() {
            Object.Destroy(SpawnedEffect);
        }
        
    }
    
}