using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Parameters")]
    [Tooltip("Dictates if this Entity is Invincible")]
    [SerializeField] protected bool Invincible = false;
    [Tooltip("Dictates this Entity Health")]
    [SerializeField] protected int _health = 50;
    [HideInInspector] public int Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public virtual void TakeDamage(int value = 1)
    {
        if (!Invincible)
        {
            Health -= value;
            if (Health <= 0) Death();
        }
    }

    public virtual void Death()
    {
        Destroy(gameObject);
    }
}
