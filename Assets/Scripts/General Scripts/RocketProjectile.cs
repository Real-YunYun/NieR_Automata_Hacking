using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [Header("Rocket Projectile Parameters")]
    private float ProjectileSpeed = 500f;
    private int ExplosionDamage = 10;
    private float ExplosionRadius = 7.5f;
    private float ExplosionTime = 0.75f;
    private GameObject RocketParticle;

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, ProjectileSpeed));
        RocketParticle = Resources.Load<GameObject>("General/Explosion Particle");
        StartCoroutine("TriggerExplosion");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Indestructible")) Explode();
        if (other.gameObject.CompareTag("Destructible") || other.gameObject.CompareTag("Entity") || other.CompareTag("Enemy Projectile")) Explode();
    }

    private void Explode()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, ExplosionRadius / 2);
        
        foreach (Collider collider in Colliders)
        {
            if (collider.CompareTag("Entity"))
            {
                collider.GetComponent<Entity>().TakeDamage((int)(ExplosionDamage *  (1 - (transform.position - collider.transform.position).magnitude / ExplosionRadius)) + 1);
            }
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
