using UnityEngine;
using Projectiles;

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

    protected override void OnEnable()
    {
        if (Indestructible) GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Enemy_Projectile1");
        else GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Enemy_Projectile2");
    }

    protected new void OnTriggerEnter(Collider other)
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
