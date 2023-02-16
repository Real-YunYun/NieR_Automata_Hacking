using UnityEngine;
using Projectiles;

public class PlayerProjectile : Projectile
{
    protected override void OnEnable()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/M_Player_Projectile");
    }
}
