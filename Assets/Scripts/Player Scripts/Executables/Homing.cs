using System.Collections;
using UnityEngine;
using Executables;

public class Homing : Executable
{
    [Header("Abilities Parameters")] private GameObject OriginalProjectile;
    private GameObject HomingProjectile;

    protected override void Awake()
    {
        Usable = true;
        Stats.Name = "Homing";
        Stats.Description = "Shoot Homing Projectiles";
        Stats.Sprite = "Player/UI Images/None";
        Stats.Duration = 10f;
        Stats.Cooldown = 25f;
        Stats.Upkeep = 0f;
        this.enabled = false;
    }

    void OnEnable()
    {
        OnCooldown = true;

        OriginalProjectile = Resources.Load<GameObject>("Projectiles/Player Projectile");
        HomingProjectile = Resources.Load<GameObject>("Projectiles/Homing Projectile");
        StartCoroutine("HomingCooldown");
    }

    IEnumerator HomingCooldown()
    {
        transform.GetComponent<PlayerController>().ChangeProjectile(HomingProjectile);
        yield return new WaitForSeconds(Stats.Duration);
        transform.GetComponent<PlayerController>().ChangeProjectile(OriginalProjectile);
    }
}
