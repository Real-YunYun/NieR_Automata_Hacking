using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Parameters")]
    [SerializeField] private bool Indestructible = false;

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 250f));
    }

    private void OnTriggerEnter(Collider other)
    {
        // More Difficult
        //if (other.gameObject.CompareTag("Player Projectile") && Indestructible) Destroy(other.gameObject);
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
