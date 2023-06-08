using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Projectiles {
    public class ShootingComponent : MonoBehaviour {
        [Header("Component Parameters")] 
        [SerializeField] protected Transform ProjectileSpawn;
        [SerializeField] protected GameObject ProjectilePrefab;
        [SerializeField] protected bool CanShoot = true;
        [SerializeField] public float FireRate = 8f;
        [SerializeField] protected bool FireRateDelay = false;
        [SerializeField] private Entity ComponentInstigator; 

        [Header("Audio Clips")] 
        [SerializeField] protected AudioSource ShootingSource;

        #region Delegates and Events

        public delegate void OnFireStartedDelegate();
        public delegate void OnFireEndedDelegate(Projectile SpawnedProjectile);

        public event OnFireStartedDelegate OnFireStarted;
        public event OnFireEndedDelegate OnFireEnded;

        // Make for Each Event!
        protected void Execute_OnFireStarted()
        {
            if (OnFireStarted != null) OnFireStarted();
        }

        protected void Execute_OnFireEnded(Projectile SpawnedProjectile)
        {
            if (OnFireEnded != null) OnFireEnded(SpawnedProjectile);
        }
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
            if (FireRateDelay || !CanShoot) return;

            if (ShootingSource) ShootingSource.Play();
            StartCoroutine(ShootingDelay());
            if (OnFireStarted != null) OnFireStarted();
            GameObject SpawnedGameObject = Instantiate(ProjectilePrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);

            if (SpawnedGameObject.GetComponent<Entity>()) return;
            Projectile SpawnedProjectile = SpawnedGameObject.GetComponent<Projectile>();

            Entity Instigator;
            if (gameObject.TryGetComponent(out Instigator)) 
                SpawnedProjectile.Result = new HitResult(Instigator);
            else SpawnedProjectile.Result = new HitResult(ComponentInstigator);
            
            if (OnFireEnded != null) OnFireEnded(SpawnedProjectile);
        }

        private IEnumerator ShootingDelay() {
            FireRateDelay = true;
            yield return new WaitForSeconds(1f / FireRate);
            FireRateDelay = false;
        }

        #region Projectile Funcitons

        public void ChangeProjectile(GameObject Projectile)
        {
            ProjectilePrefab = Projectile;
        }

        public GameObject GetProjectile()
        {
            return ProjectilePrefab;
        }

        #endregion
        
    }
}
