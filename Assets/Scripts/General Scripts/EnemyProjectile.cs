using UnityEngine;

public class EnemyProjectile : Projectile
{
    [Header("Projectile Parameters")]
    [SerializeField] private bool Indestructible = false;

    override protected void Awake()
    {
        ProjectileSpeed = 1000f;
        LifeSpan = 5f;
        base.Awake();
    }

    override protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player Projectile") && !Indestructible)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage();
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Indestructible")) Destroy(gameObject);
    }
}
