using System.Collections;
using UnityEngine;
using Items.Executables;
using Entities;
using Entities.Projectiles;

namespace Items {
    public class Homing : Executable {
        [Header("Abilities Parameters")] 
        private GameObject OriginalProjectile;
        private GameObject HomingProjectile;

        protected new virtual void Awake() {
            Name = "None";
            Description = "";
            Sprite = "Player/UI Images/None";
            Duration = 0f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Usable = true;
            this.enabled = false;
        }

        void OnEnable() {
            OnCooldown = true;

            OriginalProjectile = Resources.Load<GameObject>("Projectiles/Player Projectile");
            HomingProjectile = Resources.Load<GameObject>("Projectiles/Homing Projectile");
            StartCoroutine("HomingCooldown");
        }

        IEnumerator HomingCooldown()
        {
            transform.GetComponent<ShootingComponent>().ChangeProjectile(HomingProjectile);
            yield return new WaitForSeconds(Duration);
            transform.GetComponent<ShootingComponent>().ChangeProjectile(OriginalProjectile);
        }
    }
}
