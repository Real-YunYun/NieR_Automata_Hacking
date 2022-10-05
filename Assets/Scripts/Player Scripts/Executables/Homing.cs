using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : Executable
{
    [Header("Abilities Parameters")]
    private GameObject OriginalProjectile;
    private GameObject HomingProjectile;

    void Awake()
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

        OriginalProjectile = Resources.Load<GameObject>("General/Player Projectile");
        HomingProjectile = Resources.Load<GameObject>("General/Homing Projectile");
        StartCoroutine("HomingCooldown");
    }

    void Update()
    {
        Stats.Upkeep += Time.deltaTime;
        if (Stats.Upkeep >= Stats.Cooldown)
        {
            Stats.Upkeep = 0;
            OnCooldown = false;
            this.enabled = false;
        }
    }

    IEnumerator HomingCooldown()
    {
        transform.GetComponent<PlayerController>().ChangeProjectile(HomingProjectile);
        yield return new WaitForSeconds(Stats.Duration);
        transform.GetComponent<PlayerController>().ChangeProjectile(OriginalProjectile);
    }
}