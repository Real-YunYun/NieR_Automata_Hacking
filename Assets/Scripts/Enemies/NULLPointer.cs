using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using UnityEngine;

namespace Entities.Enemies {
    [RequireComponent(typeof(ShootingComponent))]
    public class NULLPointer : Enemy {
        [Header("Pointer Parameters")] 
        private GameObject NULL;
        private Transform Player;
        private Transform Rotator;
        private Transform SpawnTransform;

        private ShootingComponent ShootingComponent; 

        void Awake() {
            NULL = Resources.Load<GameObject>("Enemies/NULL");
            Rotator = transform.Find("Rotator");
            SpawnTransform = transform.Find("Rotator/NULL Spawn Point");
            gameObject.TryGetComponent(out ShootingComponent);
            if (ShootingComponent == null) return;
            ShootingComponent.ChangeProjectile(NULL);
        }

        protected void Start() {
            Player = GameManager.Instance.PlayerControllerInstance.Character ? GameManager.Instance.PlayerControllerInstance.Character.transform : transform;
            if (ShootingComponent == null) return;
            ShootingComponent.InvokeRepeating(nameof(ShootingComponent.Fire), 1.0f / ShootingComponent.FireRate, 1.0f / ShootingComponent.FireRate);
        }

        // Update is called once per frame
        void Update() {
            Rotator.rotation = Quaternion.LookRotation(Player.position, Rotator.forward);
        }
    }
}
