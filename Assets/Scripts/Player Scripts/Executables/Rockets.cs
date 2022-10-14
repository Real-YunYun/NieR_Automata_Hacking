using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockets : Executable
{
    [Header("Abilities Parameters")]
    private GameObject OriginalProjectile;
    private GameObject RocketProjectile;

    // Start is called before the first frame update
    protected override void Awake()
    {
        Usable = true;
        Stats.Name = "Rockets";
        Stats.Description = "Replaces your projectiles with Rockets that explode";
        Stats.Sprite = "Player/UI Images/Rockets";
        Stats.Duration = 10f;
        Stats.Cooldown = 20f;
        Stats.Upkeep = 0f;
        this.enabled = false;
    }

    void OnEnable()
    {
        OnCooldown = true;

        OriginalProjectile = Resources.Load<GameObject>("General/Player Projectile");
        RocketProjectile = Resources.Load<GameObject>("General/Rocket Projectile");
        StartCoroutine("RocketCooldown");
    }

    IEnumerator RocketCooldown()
    {
        transform.GetComponent<PlayerController>().ChangeProjectile(RocketProjectile);
        transform.GetComponent<PlayerController>().FireRate = 1f;
        yield return new WaitForSeconds(Stats.Duration);
        transform.GetComponent<PlayerController>().ChangeProjectile(OriginalProjectile);
        transform.GetComponent<PlayerController>().FireRate = 8f;
    }
}
