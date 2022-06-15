using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerCube : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        // Player Touched Entity 
        if (other.gameObject.CompareTag("Entity")) other.gameObject.GetComponent<Entity>().TakeDamage();
        if (other.gameObject.CompareTag("Player")) other.gameObject.GetComponent<PlayerController>().TakeDamage();

        //Projectile Hits Cube
        if (other.gameObject.CompareTag("Player Projectile")) Destroy(other.gameObject);
        if (other.gameObject.CompareTag("Enemy Projectile")) Destroy(other.gameObject);
    }
}
