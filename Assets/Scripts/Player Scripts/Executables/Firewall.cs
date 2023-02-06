using UnityEngine;
using Executables;

public class Firewall : Executable
{
    [Header("Abilities Parameters")] 
    private GameObject FirewallPrefab;

    protected override void Awake()
    {
        Usable = true;
        Stats.Name = "Firewall";
        Stats.Description = "Summon a Firewall to block projectiles and move enemies";
        Stats.Sprite = "Player/UI Images/Firewall";
        Stats.Duration = 10f;
        Stats.Cooldown = 15f;
        Stats.Upkeep = 0f;
        this.enabled = false;
    }

    void OnEnable()
    {
        OnCooldown = true;
        FirewallPrefab = Resources.Load<GameObject>("Player/Firewall");
        Transform ProjectileSpawn = transform.Find("Player Mesh/Projectile Spawn");
        GameObject Firewall = Instantiate(FirewallPrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);
        Firewall.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 5), ForceMode.Impulse);
        Destroy(Firewall, Stats.Duration);
    }
}
