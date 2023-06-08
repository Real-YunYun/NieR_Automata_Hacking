using System.Collections.Generic;
using UnityEngine;
using Entities;
using Items.Threads;
using Items.Threads.EventBased;
using Entities.Projectiles;

namespace Items {
    public class TrackingMissile : Proc {
        [Header("Proc Parameters")]
        private List<Projectile> HandlingProjectiles = new();
        private Entity HitEntity;
        
        protected new virtual void Awake() {
            Name = "Tracking Missiles";
            Description = "DEBUG ONLY";
            Sprite = "Player/UI Images/None";
            Duration = 0f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Type = EThreadBehaviour.EventBased;
            ChanceToProc = 0.1f;
            this.enabled = true;
        }

        // Start is called before the first frame update
        private void OnEnable() {
            Owner.GetComponent<ShootingComponent>().OnFireEnded += HandleProjectile;
        }

        private void HandleProjectile(Projectile SpawnedProjectile) {
            HandlingProjectiles.Add(SpawnedProjectile);
            HandlingProjectiles[^1].OnProjectileHit += OnHit;
        }

        protected override void OnHit(Entity _HitEntity) {
            HitEntity = _HitEntity;
            TryProc();
        }

        protected override void TryProc() {
            if (Random.value <= ChanceToProc) OnProc();
        }

        protected override void OnProc() {
            Execute_OnThreadStarted();
            Vector3 RandomDirection = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);
            HomingProjectile ProcedProjectile =
                Instantiate(
                    Resources.Load<GameObject>("Projectiles/Homing Projectile"),
                    Owner.gameObject.transform.position + RandomDirection * 1.5f,
                    Quaternion.identity
                ).GetComponent<HomingProjectile>();
            ProcedProjectile.Result = new(Owner);
            ProcedProjectile.ChangeDirection(RandomDirection);
            ProcedProjectile.SetTargetTransform(HitEntity.transform);
        }

        private void OnDisable()
        {
            Owner.GetComponent<ShootingComponent>().OnFireEnded -= HandleProjectile;
        }
    }
}
