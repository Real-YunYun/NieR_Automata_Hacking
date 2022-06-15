using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private float ProjectileSpeed = 1000f;
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, ProjectileSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Indestructible")) Destroy(gameObject);
        if (other.gameObject.CompareTag("Destructible"))
        {
            other.gameObject.GetComponent<Entity>().TakeDamage();
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Entity"))
        {
            other.gameObject.GetComponent<Entity>().TakeDamage();
            Destroy(gameObject);
        }

    }
}
