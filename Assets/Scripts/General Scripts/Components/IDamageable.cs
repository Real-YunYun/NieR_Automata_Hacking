using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Projectiles;

namespace Entities {
    public interface IDamageable {
        
        // Method for making the Entity take Damage
        public void TakeDamage(float Damage, Entity Instigator, out HitResult EntityHitResult, bool BypassTypeCheck = false);
        
        // Method for when an Entity Dies
        
    }
}
