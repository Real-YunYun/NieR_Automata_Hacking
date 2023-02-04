using System;
using UnityEngine;
using Threads;
using Unity.Collections;
using UnityEditor;

public enum EOrbitalBehaviour
{
    Fixed,
    Lag,
}

public abstract class Orbitals : Thread
{

    [Header("Orbital Fields")] 
    [SerializeField] protected GameObject OrbitingPrefab;
    
    [ReadOnly]
    protected Collider Collider;

    [Tooltip("Based off 'DeltaTime', this will determine how fast this Orbital will orbit around the Entity")]
    [SerializeField] protected float RotationalSpeed;

    [Tooltip("How far out will this Orbital will orbit around")]
    [SerializeField] protected float Range = 2.5f;
    
    [Tooltip("How big is the collider bound to this orbital")]
    [SerializeField] protected float Radius = 0.5f;

    [Tooltip("Based off 'DeltaTime', this is the damage dealt when the sphere collider is overlapping an Entity")]
    [SerializeField] protected float TickDamage;

    protected void Start()
    {
        if (!OrbitingPrefab) {
            enabled = false;
            return;
        }
        
        try {
            Collider = OrbitingPrefab.GetComponent<Collider>();
        }
        catch (Exception e) {
            Collider = OrbitingPrefab.AddComponent<Collider>();
        }
        
        Collider.isTrigger = true;

        OrbitingPrefab.transform.parent = transform;
        OrbitingPrefab.transform.position = new Vector3(Range, 0.5f, 0.0f);
    }

    protected void Update()
    {
        transform.Rotate(new Vector3(0, RotationalSpeed, 0) * Time.deltaTime);
    }
}
