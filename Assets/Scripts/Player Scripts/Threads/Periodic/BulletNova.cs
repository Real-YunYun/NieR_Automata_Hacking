using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using UnityEngine;
using Items.Threads;
using Items.Threads.Periodic;

namespace Items {
    
    public class BulletNova : Periodic {

        [Header("Nova Parameters")] 
        [SerializeField] private int ProjCount = 8;
        [SerializeField] private GameObject ProjGameObject;
        
        protected new void Awake() {
            base.Awake();
            Name = "BulletNova";
            Description = "DEBUG ONLY";
            Sprite = "Player/UI Images/None";
            Duration = 1;
            Cooldown = 5;
            Upkeep = 0;

            Type = EThreadBehaviour.Periodic;
        }

        private void OnEnable() {
            OnThreadStarted += OnActivated;
            if (Owner == null) return;
            if (Owner.TryGetComponent(out ShootingComponent Comp)) {
                ProjGameObject = Comp.GetProjectile();
            }
        }

        private void OnDisable() {
            OnThreadStarted -= OnActivated;
        }

        // Update is called once per frame
        void Update() {
            Upkeep += Time.deltaTime;
            if (Upkeep >= Cooldown) {
                Execute_OnThreadStarted();
                Upkeep = 0;
                Execute_OnThreadEnded();
            }
        }

        private void OnActivated() {
            for (int i = 0; i < ProjCount; i++) {
                float Angle = (2 * Mathf.PI) / ProjCount * i;
                Vector3 ProjDir = new Vector3(Mathf.Cos(Angle), 0, -Mathf.Sin(Angle));
                Vector3 SpawnPos = Owner.transform.position + (ProjDir * 2.5f);
                Quaternion SpawnRot = Quaternion.Euler(0, Angle * Mathf.Rad2Deg + 90, 0);
                Projectile SpawnedProj = Instantiate(ProjGameObject, SpawnPos, SpawnRot).GetComponent<Projectile>();
                SpawnedProj.Result = new HitResult(Owner, null, Owner.GetType());
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            for (int i = 0; i < ProjCount; i++) {
                float Angle = (2 * Mathf.PI) / ProjCount * i;
                Vector3 ProjDir = new Vector3(Mathf.Cos(Angle), 0, Mathf.Sin(Angle)).normalized * 2.5f;
                Gizmos.DrawWireSphere(Owner.transform.position + ProjDir, 0.1f);
            }
        }
    }
    
}