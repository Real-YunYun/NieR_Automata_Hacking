using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies {
    public class EXE : Enemy {

        private void Awake() {
            TryGetComponent(out HealthComponent);
            if (HealthComponent)
                HealthComponent.Invincible = true;
        }

        private void OnEnable() {
            if (HealthComponent == null) return;
            HealthComponent.OnDeath += Death;
        }

        protected override void Death() {
            Instantiate(DeathParticle, transform.position, DeathParticle.transform.rotation);
            Destroy(transform.parent.gameObject);
            Destroy(gameObject);
            Vector3 quaternion = new Vector3(90, 0, 0);
            Instantiate(Resources.Load("Level/Level Interact"),
                new Vector3(transform.position.x, transform.position.y - 0.99f, transform.position.z),
                Quaternion.Euler(quaternion));
        }
        
        private void OnDisable() {
            if (HealthComponent == null) return;
            HealthComponent.OnDeath -= Death;
        }
    }
}
