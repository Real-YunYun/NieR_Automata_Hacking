using UnityEngine;
using Items.Threads.EventBased;
using Items.Threads;
using Entities.Projectiles;
using Entities;

namespace Items {
    public class HomingThread : Proc {
        protected new virtual void Awake() {
            Name = "HomingThread";
            Description = "DEBUG ONLY";
            Sprite = "Player/UI Images/None";
            Duration = 0f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Type = EThreadBehaviour.EventBased;
            ChanceToProc = 0.15f;
            this.enabled = true;
        }
        
        private void OnEnable() {
            if (!Owner || Owner.GetComponent<ShootingComponent>() == null) return;
            Owner.GetComponent<ShootingComponent>().OnFireStarted += TryProc;
            Owner.GetComponent<ShootingComponent>().OnFireEnded += OnReset;
        }

        protected override void OnHit(Entity HitEntity) {
            throw new System.NotImplementedException();
        }

        protected override void TryProc() {
            if (Random.value <= ChanceToProc) OnProc();
        }

        protected override void OnProc() {
            Execute_OnThreadStarted();
            Owner.GetComponent<ShootingComponent>().ChangeProjectile(Resources.Load<GameObject>("Projectiles/Homing Projectile"));
        }

        private void OnReset(Projectile TempProjectile) {
            Execute_OnThreadEnded();
            Owner.GetComponent<ShootingComponent>().ChangeProjectile(Resources.Load<GameObject>("Projectiles/Player Projectile"));
        }

        private void OnDisable() {
            if (!Owner || Owner.GetComponent<ShootingComponent>() == null) return;
            Owner.GetComponent<ShootingComponent>().OnFireStarted -= TryProc;
            Owner.GetComponent<ShootingComponent>().OnFireEnded -= OnReset;
        }
    }
}
