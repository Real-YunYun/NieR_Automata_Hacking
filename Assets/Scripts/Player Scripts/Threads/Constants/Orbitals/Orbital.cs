using System;
using UnityEngine;
using Unity.Collections;
using Entities;
using Entities.Projectiles;

namespace Items.Threads.Constant.Obritals
{
    public enum EOrbitalBehaviour
    {
        Fixed,
        Lagged,
    }

    public abstract class Orbital : Constant {
        [Header("Orbital Fields")] 
        protected GameObject OrbitingPrefab;

        [ReadOnly] protected new EThreadBehaviour Type = EThreadBehaviour.Constant;
        [ReadOnly] protected EOrbitalBehaviour OrbitalBehaviour = EOrbitalBehaviour.Fixed;

        [ReadOnly, SerializeField] 
        protected SphereCollider Collider;
        protected OrbitalCollider OrbitalCollider;
        private float Angle;

        [Tooltip("Based off 'DeltaTime', this will determine how fast this Orbital will orbit around the Entity")]
        [SerializeField]
        protected float RotationSpeed = 12.0f;

        [Tooltip("How big is the collider bound to this orbital")] [SerializeField]
        protected float Radius = 1.5f;

        [Tooltip("How far out will this Orbital will orbit around")] [SerializeField]
        protected float Range = 2.5f;

        [Tooltip("How big is the collider bound to this orbital")] [SerializeField]
        protected Vector2 LocalOffset = new(0, 0);

        [Tooltip("Based off 'DeltaTime', this is the damage dealt when the sphere collider is overlapping an Entity")]
        [SerializeField]
        protected float TickDamage = 0.1f;

        protected virtual void OnEnable() {
            OrbitingPrefab = Instantiate(Resources.Load<GameObject>("Building Blocks/Danger Cube 1x1"));
            OrbitingPrefab.transform.position = transform.position + new Vector3(Range, 0.0f, 0.0f);
            Entity preExistingEntity;
            if (OrbitingPrefab.TryGetComponent(out preExistingEntity)) Destroy(preExistingEntity);
            Execute_OnThreadStarted();
        }

        protected virtual void OnDisable() {
            Destroy(OrbitingPrefab);
            Execute_OnThreadEnded();
        }

        protected virtual void Start()
        {
            Collider[] preExistingColliders = OrbitingPrefab.GetComponents<Collider>();
            // Create class for now collider events!
            foreach (Collider tempCollider in preExistingColliders)
                Destroy(tempCollider);

            Collider = OrbitingPrefab.AddComponent<SphereCollider>();
            OrbitingPrefab.AddComponent<OrbitalCollider>().OnOrbitalStayOverlap += DealDamage;
            Collider.isTrigger = true;
            Collider.radius = Radius;

            OrbitingPrefab.transform.position = new Vector3(Range, 0.0f, 0.0f);
            Angle = UnityEngine.Random.Range(0, 360);
        }

        private void LateUpdate()
        {
            Vector3 positionOffset = ComputePositionOffset(Angle);

            OrbitingPrefab.transform.position = transform.position + positionOffset;
            OrbitingPrefab.transform.rotation = Quaternion.LookRotation(transform.position - OrbitingPrefab.transform.position, transform.up);

            Angle += Time.deltaTime * RotationSpeed;
        }

        private Vector3 ComputePositionOffset(float a)
        {
            a *= Mathf.Deg2Rad;
            Vector3 positionOffset = new Vector3(Mathf.Cos(a) * Range, 0, Mathf.Sin(a) * Range);
            return positionOffset;
        }

        protected virtual void DealDamage(Entity OverlappedEntity) {
            if (OverlappedEntity == Owner) return;
            HealthComponent OverlappedEntityHealthComponent;
            HitResult Result = new HitResult(Owner);
            if (OverlappedEntity.TryGetComponent(out OverlappedEntityHealthComponent)) 
                OverlappedEntityHealthComponent.TakeDamage(TickDamage, Owner, out Result);
        }

        #if UNITY_EDITOR
        [SerializeField] private bool drawGizmos = true;
        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos)
                return;

            // Draw an arc around the target
            Vector3 position = transform.position;
            Vector3 normal = Vector3.up;
            Vector3 forward = Vector3.forward;

            Vector3 positionOffset = ComputePositionOffset(0);

            Vector3 verticalOffset = positionOffset.y * normal;

            position += verticalOffset;
            positionOffset -= verticalOffset;

            UnityEditor.Handles.DrawWireArc(transform.position, normal, forward, 360, Range);

            // Draw label to indicate radius
            UnityEditor.Handles.DrawLine(position, position + positionOffset);
            Vector3 labelPosition = position + positionOffset * 0.5f;
            labelPosition += Vector3.Cross(positionOffset.normalized, transform.up) * 0.25f; 
            UnityEditor.Handles.Label(labelPosition, Range.ToString("0.00"));
        }
        #endif
    }
}