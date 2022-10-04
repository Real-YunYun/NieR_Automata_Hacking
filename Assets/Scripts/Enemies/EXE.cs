using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXE : Enemy
{
    public override void TakeDamage(int value = 1)
    {
        if (!transform.parent.Find("Shield").gameObject.activeSelf) base.TakeDamage(value);
    }

    public override void Death()
    {
        Instantiate(DeathParticle, transform.position, DeathParticle.transform.rotation);
        Destroy(transform.parent.gameObject);
        Destroy(gameObject);
        Vector3 quaternion = new Vector3(90, 0, 0);
        Instantiate(Resources.Load("General/Level Interact"), new Vector3(transform.position.x, transform.position.y - 0.99f, transform.position.z), Quaternion.Euler(quaternion));
    }
}
