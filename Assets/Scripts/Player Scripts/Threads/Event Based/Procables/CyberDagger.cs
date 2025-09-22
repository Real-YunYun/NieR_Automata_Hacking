using UnityEngine;
using System.Collections.Generic;
using Entities;
using Entities.Projectiles;
using Items.Threads;
using Items.Threads.EventBased;

namespace Items {
    public class CyberDagger : Proc {

        [Header("Cyber Dagger Components")]
        private Entity HitEntity;
        private List<Projectile> Projectiles = new List<Projectile>();
        
        protected new virtual void Awake() {
            Name = "Tracking Missiles";
            Description = "Chance to inflict \"Data Leak\" on hits!";
            Sprite = "Player/UI Images/None";
            Duration = 0f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Type = EThreadBehaviour.EventBased;
            ChanceToProc = 0.07f;
            this.enabled = true;
        }
        
        private void OnEnable() {
            if (!Owner) return;
            if (!Owner.TryGetComponent(out ShootingComponent Comp)) return;
            Comp.OnFireEnded += HandleProjectile;
        }
        
        private void HandleProjectile(Projectile SpawnedProjectile) {
            SpawnedProjectile.OnProjectileHit += OnHit;
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
            if (!HitEntity.TryGetComponent(out StatusComponent Status)) return;
            Status.ApplyStatus("Status.DataLeak", new HitResult(Owner, HitEntity));
        }
        
        private void OnDisable() {
            if (!Owner) return;
            if (!Owner.TryGetComponent(out ShootingComponent Comp)) return;
            Comp.OnFireEnded -= HandleProjectile;
        }
        
    }
}