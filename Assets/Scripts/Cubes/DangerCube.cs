using UnityEngine;

public class DangerCube : Entity
{
    void Awake()
    {
        Invincible = true;
    }

    private void OnTriggerStay(Collider other)
    {
        // Player Touched Entity 
        if (other.gameObject.CompareTag("Entity")) other.gameObject.GetComponent<Entity>().TakeDamage();

        //Projectile Hits Cube
        if (other.gameObject.CompareTag("Player Projectile")) Destroy(other.gameObject);
        if (other.gameObject.CompareTag("Enemy Projectile")) Destroy(other.gameObject);
    }
}
