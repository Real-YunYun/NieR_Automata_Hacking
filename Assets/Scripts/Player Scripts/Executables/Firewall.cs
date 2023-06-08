using Entities.Projectiles;
using UnityEngine;
using Items.Executables;

namespace Items {
    public class Firewall : Executable {
        [Header("Abilities Parameters")] 
        private GameObject FirewallPrefab;

        protected override void Awake() {
            Name = "Firewall";
            Description = "Summon a Firewall to block projectiles and move enemies";
            Sprite = "Player/UI Images/Firewall";
            Duration = 10f;
            Cooldown = 25f;
            Upkeep = 0f;

            Usable = true;
            this.enabled = false;
        }

        void OnEnable()
        {
            OnCooldown = true;
            FirewallPrefab = Resources.Load<GameObject>("Player/Firewall");
            Transform ProjectileSpawn = Owner.GetComponent<ShootingComponent>().GetProjectileSpawn();
            GameObject Firewall = Instantiate(FirewallPrefab, ProjectileSpawn.position, ProjectileSpawn.rotation);
            Firewall.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 5), ForceMode.Impulse);
            Destroy(Firewall, Duration);
        }
    }
}
