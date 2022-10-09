using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    protected float ProjectileSpeed = 1000f;
    protected int Damage = 1;
    protected virtual void Awake()
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, ProjectileSpeed));
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other);
    }

    protected void HandleTrigger(Collider other)
    {
        if (other.gameObject.CompareTag("Indestructible")) Destroy(gameObject);
        if (other.gameObject.CompareTag("Entity"))
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(Damage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Destructible"))
        {
            other.gameObject.GetComponent<DestructibleCube>().TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
