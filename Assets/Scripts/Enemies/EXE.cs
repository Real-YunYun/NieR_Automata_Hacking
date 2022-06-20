using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXE : Entity
{
    public override void Death()
    {
        Instantiate(DeathParticle, transform.position, DeathParticle.transform.rotation);
        Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}
