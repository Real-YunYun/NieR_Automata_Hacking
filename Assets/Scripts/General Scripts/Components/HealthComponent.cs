using System.Collections;
using System.Collections.Generic;
using Entities.Modules;
using UnityEngine;
using Entities.Projectiles;

namespace Entities {
    public class HealthComponent : EntityModule, IDamageable {
        [Header("Entity Parameters")]

        [Tooltip("Dictates if this Entity is Invincible")]
        [SerializeField] public bool Invincible = false;
        [SerializeField] public bool HasInvincibilityFrames = false;

        [Tooltip("Dictates this Entity Health")] 
        [SerializeField] private float _health = 50;

        public float Health => _health;

        // Most likely for Executables and Threads 
        #region Delegates and Events

        public delegate void OnDeathDelegate();
        public delegate void OnTakeDamageDelegate();

        public event OnDeathDelegate OnDeath;
        public event OnTakeDamageDelegate OnTakeDamage;

        #endregion

        public void TakeDamage(float Damage, Entity Instigator, out HitResult EntityHitResult, bool BypassTypeCheck = false) {
            EntityHitResult = new HitResult(Instigator, gameObject.GetComponent<Entity>());
            if (Invincible) return;
            
            if ((Instigator == null && Instigator.GetType() != EntityHitResult.HitEntity.GetType()) || !Instigator.gameObject.CompareTag(EntityHitResult.HitEntity.gameObject.tag)) {
                _health -= Damage;
                if (OnTakeDamage != null) OnTakeDamage();
                if (HasInvincibilityFrames) StartCoroutine(InvincibilityFrames());
                if (_health <= 0.0f) {
                    // On Death Events
                    if (OnDeath != null) OnDeath();
                }
            }
        }

        public void TakeDamage(float Damage, HitResult Result) {
            if (Invincible) return;
            
            if ((Result.Instigator == null && Result.Instigator.GetType() != Result.HitEntity.GetType()) || !Result.Instigator.gameObject.CompareTag(Result.HitEntity.gameObject.tag)) {
                _health -= Damage;
                if (OnTakeDamage != null) OnTakeDamage();
                if (HasInvincibilityFrames) StartCoroutine(InvincibilityFrames());
                if (_health <= 0.0f) {
                    // On Death Events
                    if (OnDeath != null) OnDeath();
                }
            }
        }

        private IEnumerator InvincibilityFrames() {
            Invincible = true;
            yield return new WaitForSecondsRealtime(0.25f);
            Invincible = false;
        }
    }
}
