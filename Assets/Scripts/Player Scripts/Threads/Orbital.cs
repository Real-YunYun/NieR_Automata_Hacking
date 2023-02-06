using System;
using UnityEngine;
using Unity.Collections;

namespace Threads.Obritals
{
    public enum EOrbitalBehaviour
    {
        Fixed,
        Lagged,
    }

    public abstract class Orbital : Thread
    {

        [Header("Orbital Fields")] 
        [SerializeField] protected GameObject OrbitingPrefab;

        [ReadOnly] protected new EThreadBehaviour Type = EThreadBehaviour.Constant;
        [ReadOnly] protected EOrbitalBehaviour OrbitalBehaviour = EOrbitalBehaviour.Fixed;

        [ReadOnly] protected Collider Collider;

        [Tooltip("Based off 'DeltaTime', this will determine how fast this Orbital will orbit around the Entity")]
        [SerializeField] protected float RotationalSpeed = 12.0f;

        [Tooltip("How big is the collider bound to this orbital")] 
        [SerializeField] protected float Radius = 0.5f;
        
        [Tooltip("How far out will this Orbital will orbit around")] 
        [SerializeField] protected float Range = 2.5f;
        
        [Tooltip("How big is the collider bound to this orbital")] 
        [SerializeField] protected Vector2 LocalOffset = new(0, 0);

        [Tooltip("Based off 'DeltaTime', this is the damage dealt when the sphere collider is overlapping an Entity")]
        [SerializeField] protected float TickDamage = 0.1f;

        protected override void Awake()
        {
            Stats.Name = "None";
            Stats.Description = "";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;
        }

        protected void Start()
        {
            try
            {
                Collider = OrbitingPrefab.GetComponent<Collider>();
            }
            catch (Exception e)
            {
                Collider = OrbitingPrefab.AddComponent<Collider>();
            }

            Collider.isTrigger = true;
            OrbitingPrefab.transform.position = new Vector3(Range, 0.0f, 0.0f);
        }

        protected void Update()
        {
            // Find a way to detach from the parent and either lerp or be fixed to the "imaginary" anchor point
            transform.Rotate(new Vector3(0, RotationalSpeed, 0) * Time.deltaTime);
        }
    }
}