using UnityEngine;
using Items.Executables;

namespace Items {
    public class Nodes : Executable {
        [Header("Abilities Parameters")] private GameObject TurretPrefab;
        private Transform TurretSpawn1;
        private Transform TurretSpawn2;

        protected override void Awake() {
            Usable = true;
            Stats.Name = "Nodes";
            Stats.Description = "Summon 2 Turrets to assit you during combat";
            Stats.Sprite = "Player/UI Images/Nodes";
            Stats.Duration = 10f;
            Stats.Cooldown = 20f;
            Stats.Upkeep = 0f;
            this.enabled = false;
        }

        void OnEnable() {
            OnCooldown = true;

            TurretPrefab = Resources.Load<GameObject>("Player/Player Turret");
            TurretSpawn1 = transform.Find("Player Mesh/Turret1");
            TurretSpawn2 = transform.Find("Player Mesh/Turret2");

            GameObject Turret1 = Instantiate(TurretPrefab, TurretSpawn1.position, TurretSpawn1.transform.rotation);
            GameObject Turret2 = Instantiate(TurretPrefab, TurretSpawn2.position, TurretSpawn2.transform.rotation);

            Turret1.GetComponent<PlayerTurret>().PlayerTurretSpawn = TurretSpawn1;
            Turret2.GetComponent<PlayerTurret>().PlayerTurretSpawn = TurretSpawn2;

            Destroy(Turret1, Stats.Duration);
            Destroy(Turret2, Stats.Duration);
        }
    }
}
