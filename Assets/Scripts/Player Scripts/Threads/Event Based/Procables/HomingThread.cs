using UnityEngine;
using Items.Threads.EventBased.Proc;
using Items.Threads;
using Projectiles;

namespace Items {
    public class HomingThread : Proc {
        [Header("Proc Parameters")] protected new float ChanceToProc = 0.15f;

        protected override void Awake() {
            Stats.Name = "";
            Stats.Description = "DEBUG ONLY";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;

            Type = EThreadBehaviour.EventBased;
        }

        // Start is called before the first frame update
        private void OnEnable() {
            Owner.OnFireStarted += TryProc;
            Owner.OnFireEnded += OnReset;
        }

        protected override void TryProc() {
            if (Random.value <= ChanceToProc) OnProc();
        }

        protected override void OnProc() {
            Execute_OnThreadStarted();
            Owner.ChangeProjectile(Resources.Load<GameObject>("Projectiles/Homing Projectile"));
        }

        private void OnReset(Projectile TempProjectile) {
            Execute_OnThreadEnded();
            Owner.ChangeProjectile(Resources.Load<GameObject>("Projectiles/Player Projectile"));
        }

        private void OnDisable() {
            Owner.OnFireStarted -= TryProc;
            Owner.OnFireEnded -= OnReset;
        }
    }
}
