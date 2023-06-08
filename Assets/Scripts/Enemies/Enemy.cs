using System;
using UnityEngine;

namespace Entities.Enemies {
    [Serializable]
    public struct EnemyStats {
        [Tooltip("How long this will take the Director to build (s)")]
        public float BuildTime;

        [Tooltip("How much this will cost the Director queue (credits)")]
        public float BuildCost;
    }

    [DefaultExecutionOrder(1)]
    public class Enemy : Entity {
        [Header("Enemy Parameters")] [Tooltip("Dictates What this Entity stats for the Director")] [SerializeField]
        public EnemyStats Stats;

        [Tooltip("Particle Effect for when this Entity dies")] [SerializeField]
        public GameObject DeathParticle;

        //Delegates and Events
        public delegate void OnDeathDelegate(GameObject entity);

        public event OnDeathDelegate OnEnemyDeath;

        private void Awake() {
            TryGetComponent(out HealthComponent);
        }

        private void OnEnable() {
            if (HealthComponent == null) return;
            HealthComponent.OnDeath += Death;
        }

        protected virtual void Death() {
            if (OnEnemyDeath != null) OnEnemyDeath(gameObject);
            GameManager.Instance.Data.Bits += (int)Stats.BuildCost;
            Instantiate(DeathParticle, transform.position, DeathParticle.transform.rotation);
            Destroy(gameObject);
        }
        
        private void OnDisable() {
            if (HealthComponent == null) return;
            HealthComponent.OnDeath -= Death;
        }
    }
}
