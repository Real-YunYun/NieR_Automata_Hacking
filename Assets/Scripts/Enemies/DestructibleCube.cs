using System;
using UnityEngine;

namespace Entities.Enemies {
    public class DestructibleCube : Entity {

        private void Awake() {
            TryGetComponent(out HealthComponent);
        }

        private void OnEnable() {
            HealthComponent.OnDeath += Death;
        }

        private void Death() {
            Destroy(gameObject);
        }
        
        private void OnDisable() {
            HealthComponent.OnDeath -= Death;
        }
    }
}
