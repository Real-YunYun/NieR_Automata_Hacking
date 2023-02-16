using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace Projectiles
{

    public struct HitResult
    {
        // Entity that fired this Projectile
        public Entity Instigator;
        public Entity HitEntity;
        public float Distance;
        public Vector3 Location;

        // Pre Initialization of the Projectile shot, although not all the information needs to be acquired yet
        public void Init(Entity _Instigator, Entity _hitEntity, float _Distance = 0.0f, Vector3 _Location = default)
        {
            Instigator = _Instigator;
            HitEntity = _hitEntity;
            Distance = _Distance;
            Location = _Location;
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public abstract class Projectile : MonoBehaviour
    {
        [Header("Projectile Parameters")] 
        protected HitResult _Result;
        public HitResult Result => _Result;
        protected float ProjectileSpeed = 2500f;
        protected Rigidbody Rigidbody;
        protected float LifeSpan = 5f;
        protected int Damage = 1;
        
        #region Events and Delegates
        
        public delegate void OnProjectileSpawnedDelegate();
        public delegate void OnProjectileHitDelegate(Entity HitEntity);

        public event OnProjectileSpawnedDelegate OnProjectileSpawned;
        public event OnProjectileHitDelegate OnProjectileHit;

        protected void Execute_OnProjectileSpawned() { if (OnProjectileSpawned != null) OnProjectileSpawned(); }
        protected void Execute_OnProjectileHit() { if (OnProjectileHit != null) OnProjectileHit(Result.HitEntity); }

        #endregion

        protected virtual void Awake()
        {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            Rigidbody.AddRelativeForce(new Vector3(0, 0, ProjectileSpeed));
            Destroy(gameObject, LifeSpan);
            
            foreach (Projectile removing in GetComponents<Projectile>())
            {
                if (removing.GetType() != GetType())
                {
                    _Result = removing.Result;
                    Destroy(removing);
                }
            }

            if (OnProjectileSpawned != null) OnProjectileSpawned();
        }

        public void ChangeDirection(Vector3 NewDirection)
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.AddRelativeForce(NewDirection * ProjectileSpeed);
            transform.rotation = Quaternion.LookRotation(Rigidbody.velocity);
        }

        protected abstract void OnEnable();

        public void InitResult(Entity _instigator)
        {
            _Result.Instigator = _instigator;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Entity>() == _Result.Instigator) return;
            if (!other.gameObject.GetComponent<Entity>()) return;
            
            _Result.Init(_Result.Instigator, other.gameObject.GetComponent<Entity>(), Vector3.Distance(Result.Instigator.transform.position, transform.position));
            if (OnProjectileHit != null) OnProjectileHit(_Result.HitEntity);
            HandleTrigger(other);
        }

        protected void HandleTrigger(Collider other)
        {
            other.gameObject.GetComponent<Entity>().TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}