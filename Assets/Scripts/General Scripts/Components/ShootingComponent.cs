using System.Collections;
using System.Collections.Generic;
using Entities.Modules;
using UnityEngine;

namespace Entities.Projectiles {
    public class ShootingComponent : EntityModule {
        [Header("Component Parameters")] 
        [SerializeField] protected Transform ProjectileSpawn;
        [SerializeField] protected GameObject ProjectilePrefab;
        [SerializeField] protected bool CanShoot = true;
        [SerializeField] public float FireRate = 8f;
        [SerializeField] protected bool FireRateDelay = false;
        [SerializeField] private Entity ComponentInstigator; // Backup Instigator for changing instigators

        [Header("Audio Clips")] 
        [SerializeField] protected AudioSource ShootingSource;

        #region Delegates and Events

        public delegate void OnFireStartedDelegate();
        public delegate void OnFireEndedDelegate(Projectile SpawnedProjectile);

        public event OnFireStartedDelegate OnFireStarted;
        public event OnFireEndedDelegate OnFireEnded;
        
        #endregion

        // Used for cases where the intimidate GameObject isn't an Entity 
        public void SetInstigator(Entity Instigator) {
            ComponentInstigator = Instigator;
        }

        public Transform GetProjectileSpawn() {
            return ProjectileSpawn;
        }
    
        // Attempts to shoot a projectile, if the attempt doesn't happen but the method is still called, the projectile returns null
        public void Fire() {
            if (GameManager.Instance.IsGamePaused || FireRateDelay || !CanShoot) return;

            if (ShootingSource) ShootingSource.Play();
            StartCoroutine(ShootingDelay());
            if (OnFireStarted != null) OnFireStarted();
            GameObject SpawnedGameObject = Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);

            if (SpawnedGameObject.GetComponent<Entity>()) return;
            Projectile SpawnedProjectile = SpawnedGameObject.GetComponent<Projectile>();
            
            SpawnedProjectile.Result = new HitResult(Owner != null ? Owner : ComponentInstigator);
            
            if (OnFireEnded != null) OnFireEnded(SpawnedProjectile);
        }

        private IEnumerator ShootingDelay() {
            FireRateDelay = true;
            yield return new WaitForSeconds(1f / FireRate);
            FireRateDelay = false;
        }

        #region Projectile Funcitons

        public void ChangeProjectile(GameObject Projectile) {
            ProjectilePrefab = Projectile;
        }

        public GameObject GetProjectile() {
            return ProjectilePrefab;
        }

        #endregion
        
    }
}
