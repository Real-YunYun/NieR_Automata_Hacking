using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCube : Entity
{
    public override void Death()
    {
        Destroy(gameObject);
    }
}
