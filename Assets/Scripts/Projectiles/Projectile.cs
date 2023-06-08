using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace Entities.Projectiles {
    public struct HitResult {
        // Entity that fired this Projectile
        public Entity Instigator;
        public Entity HitEntity;
        public System.Type EntityType;

        // Pre Initialization of the Projectile shot, although not all the information needs to be acquired yet
        public HitResult(Entity _instigator, Entity _hitEntity = null, System.Type _EntityType = null) {
            Instigator = _instigator;
            HitEntity = _hitEntity;
            EntityType = _EntityType;
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public abstract class Projectile : MonoBehaviour
    {
        [Header("Projectile Parameters")] 
        protected HitResult _Result;
        public HitResult Result {
            get => _Result;
            set => _Result = value;
        }
        protected float ProjectileSpeed = 2500f;
        protected Rigidbody Rigidbody;
        protected float LifeSpan = 3f;
        protected float Damage = 1f;
        
        #region Events and Delegates
        
        public delegate void OnProjectileSpawnedDelegate();
        public delegate void OnProjectileHitDelegate(Entity HitEntity);

        public event OnProjectileSpawnedDelegate OnProjectileSpawned;
        [Tooltip("Only Generates \"OnHit\" when it hits an Entity with a HealthComponent")]
        public event OnProjectileHitDelegate OnProjectileHit;

        protected void Execute_OnProjectileSpawned() { if (OnProjectileSpawned != null) OnProjectileSpawned(); }
        protected void Execute_OnProjectileHit() { if (OnProjectileHit != null) OnProjectileHit(Result.HitEntity); }

        #endregion

        protected virtual void Awake() {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            Rigidbody.AddRelativeForce(new Vector3(0, 0, ProjectileSpeed));
            Destroy(gameObject, LifeSpan);
            
            foreach (Projectile removing in GetComponents<Projectile>()) {
                if (removing.GetType() != GetType()) {
                    _Result = removing.Result;
                    Destroy(removing);
                }
            }

            if (OnProjectileSpawned != null) OnProjectileSpawned();
        }

        public void ChangeDirection(Vector3 NewDirection) {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.AddRelativeForce(NewDirection * ProjectileSpeed);
            transform.rotation = Quaternion.LookRotation(Rigidbody.velocity);
        }

        protected abstract void OnEnable();

        protected void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Indestructible")) Destroy(gameObject);

            if (other.gameObject.GetComponent<Entity>() == _Result.Instigator) return;
            if (!other.gameObject.GetComponent<Entity>()) return;
            
            _Result = new(_Result.Instigator, other.gameObject.GetComponent<Entity>(),  other.gameObject.GetComponent<Entity>().GetType());
            HandleTrigger(other);
        }

        protected void HandleTrigger(Collider other) {
            HealthComponent EntityHealthComponent;
            if (other.gameObject.TryGetComponent(out EntityHealthComponent)) {
                if (OnProjectileHit != null) OnProjectileHit(_Result.HitEntity);
                EntityHealthComponent.TakeDamage(Damage, Result.Instigator, out _Result);
                Destroy(gameObject);
            }
        }
    }
}