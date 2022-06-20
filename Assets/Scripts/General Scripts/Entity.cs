using System;
using UnityEngine;

[Serializable]
public struct EntityStats
{
    [Tooltip("How long this will take the Director to build (s)")]
    public float BuildTime;
    [Tooltip("How much this will cost the Director queue (credits)")]
    public float BuildCost;
}

public class Entity : MonoBehaviour
{
    [Header("Entity Parameters")]
    [Tooltip("Dictates if this Entity is Invincable")]
    [SerializeField] private bool Invincible = false;
    [Tooltip("Dictates this Entity Health")]
    [SerializeField] private int _health;
    [Tooltip("Dictates What this Entity stats for the Director")]
    [SerializeField] public EntityStats Stats;
    [Tooltip("Particle Effect for when this Entity dies")]
    [SerializeField] public GameObject DeathParticle;
    [HideInInspector] public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    //Delegates and Events
    public delegate void OnDeathDelegate(GameObject entity);
    public event OnDeathDelegate OnDeathEvent;

    public virtual void TakeDamage()
    {
        if (!Invincible)
        {
            Health -= 1;
            if (Health <= 0) Death();
        }
    }

    public virtual void Death()
    {
        OnDeathEvent(gameObject);
        GameManager.Instance.Data.Bits += (int)Stats.BuildCost;
        Instantiate(DeathParticle, transform.position, DeathParticle.transform.rotation);
        Destroy(gameObject);
    }
}
