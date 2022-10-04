using UnityEngine;

public class PlayerProjectile : Projectile
{
    override protected void Awake()
    {
        base.Awake();
    }

    override protected void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }
}
