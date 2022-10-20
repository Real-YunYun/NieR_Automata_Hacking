using UnityEngine;

public class DestructibleCube : Entity
{
    public override void Death()
    {
        Destroy(gameObject);
    }
}
