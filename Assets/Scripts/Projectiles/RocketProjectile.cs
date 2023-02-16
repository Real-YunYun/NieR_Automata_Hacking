using System.Collections;
using UnityEngine;
using Projectiles;

public class RocketProjectile : Projectile
{
    [Header("Rocket Projectile Parameters")]
    private int ExplosionDamage = 10;
    private float ExplosionRadius = 7.5f;
    private float ExplosionTime = 0.75f;
    private GameObject RocketParticle;

    protected override void Awake()
    {
        base.Awake();
        RocketParticle = Resources.Load<GameObject>("General/Explosion Particle");
        StartCoroutine("TriggerExplosion");
    }

    protected override void OnEnable()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Enemy_Projectile2");
    }

    protected new void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Indestructible") || other.gameObject.CompareTag("Destructible") || other.gameObject.CompareTag("Entity") || other.CompareTag("Enemy Projectile")) Explode();
    }

    private void Explode()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, ExplosionRadius / 2);
        
        foreach (Collider collider in Colliders)
        {
            if (collider.GetComponent<Entity>()) collider.GetComponent<Entity>().TakeDamage((int)(ExplosionDamage - (ExplosionDamage *  (1 - (transform.position - collider.transform.position).magnitude / ExplosionRadius)) + 1));

            if (collider.CompareTag("Enemy Projectile")) Destroy(collider.gameObject);
        }
        Instantiate(RocketParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator TriggerExplosion()
    {
        yield return new WaitForSeconds(ExplosionTime);
        Explode();
    }
}
