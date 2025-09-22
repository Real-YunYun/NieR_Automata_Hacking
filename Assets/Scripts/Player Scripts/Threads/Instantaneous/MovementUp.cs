using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Threads;
using Items.Threads.Instantaneous;
using Entities;

namespace Items {

    public class MovementUp : Instantaneous {

        protected new void Awake() {
            base.Awake();
            Name = "Clock Speed Boost";
            Description = "DEBUG ONLY";
            Sprite = "Player/UI Images/None";
            Duration = 0;
            Cooldown = 0;
            Upkeep = 0;

            Type = EThreadBehaviour.Instantaneous;
        }
        
        void Start() {
            if (!HasActivated) {
                Owner.GetComponent<MovementComponent>().MoveSpeed += 2f;
                HasActivated = true;
                this.enabled = true;
            }
        }
    }

}