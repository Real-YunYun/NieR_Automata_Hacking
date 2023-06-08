using UnityEngine;
using Entities;
using Entities.Projectiles;

public class EnemyProjectile : Projectile
{
    [Header("Projectile Parameters")]
    [SerializeField] private bool Indestructible = false;

    protected override void Awake() {
        ProjectileSpeed = 1000f;
        LifeSpan = 5f;
        base.Awake();
    }

    protected override void OnEnable() {
        if (Indestructible) GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Enemy_Projectile1");
        else GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Enemy_Projectile2");
    }

    protected new void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player Projectile") && !Indestructible) {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Destructible")) {
            other.gameObject.GetComponent<HealthComponent>().TakeDamage(1.0f, _Result.Instigator, out _Result);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Indestructible")) Destroy(gameObject);
    }
}
