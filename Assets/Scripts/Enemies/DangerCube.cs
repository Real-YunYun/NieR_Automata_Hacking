using Entities.Projectiles;
using UnityEngine;

namespace Entities {
    public class DangerCube : Entity {
        private void OnTriggerStay(Collider other) {
            // Cube Touched Entity 
            HealthComponent EntityHealthComponent;
            if (other.gameObject.TryGetComponent(out EntityHealthComponent)) {
                HitResult NULL = new HitResult(this);
                EntityHealthComponent.TakeDamage(1.0f, this, out NULL, true);
            }

            //Projectile Hits Cube
            if (other.gameObject.CompareTag("Player Projectile")) Destroy(other.gameObject);
            if (other.gameObject.CompareTag("Enemy Projectile")) Destroy(other.gameObject);
        }
    }
}
