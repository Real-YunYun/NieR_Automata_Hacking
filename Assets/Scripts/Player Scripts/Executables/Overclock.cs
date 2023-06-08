using System.Collections;
using UnityEngine;
using Items.Executables;
using Entities;
using Entities.Projectiles;

namespace Items {
    public class Overclock : Executable {
        [Header("Overclock Parameter")] 
        private Transform PlayerBody;
        private GameObject AfterImage;
        GameObject[] Trail = new GameObject[10];
        private bool CreateImage = true;

        [Header("Previous Parameters")] 
        private float pre_MoveSpeed = 0.0f;
        private float pre_FireRate = 0.0f;

        protected override void Awake() {
            Name = "Overclock";
            Description = "Slows down time and makes you move faster";
            Sprite = "Player/UI Images/None";
            Duration = 3f;
            Cooldown = 20f;
            Upkeep = 0f;

            Usable = true;
            this.enabled = false;
        }

        void OnEnable() {
            MovementComponent MovementComponent = GameManager.Instance.PlayerControllerInstance.PlayerMovementComponent;
            ShootingComponent ShootingComponent = GameManager.Instance.PlayerControllerInstance.PlayerShootingComponent;
            if (MovementComponent == null || ShootingComponent == null) return;
            
            AfterImage = Resources.Load<GameObject>("Player/Player Overclock Clone");
            if (GameManager.Instance.PlayerControllerInstance.PlayerCharacter != null)
                PlayerBody = GameManager.Instance.PlayerControllerInstance.PlayerCharacter.transform.Find("Player Mesh").transform;

            OnCooldown = true;
            Time.timeScale = 0.1f;
            pre_MoveSpeed = MovementComponent.MoveSpeed;
            pre_FireRate = ShootingComponent.FireRate;

            MovementComponent.MoveSpeed = 10f / Time.timeScale + 2f;
            ShootingComponent.FireRate = 10f / Time.timeScale + 2f;
            StartCoroutine("ExecutableCooldown");
        }

        protected override void Update() {
            if (!GameManager.Instance.IsGamePaused) {
                Upkeep += Time.unscaledDeltaTime;
                if (Upkeep >= Cooldown) {
                    Upkeep = 0;
                    OnCooldown = false;
                    this.enabled = false;
                }

                if (Time.timeScale != 1 && CreateImage) StartCoroutine(CreateAfterImage());
            }
        }

        IEnumerator ExecutableCooldown() {
            yield return new WaitForSeconds(Duration * Time.timeScale);
            
            MovementComponent MovementComponent = GameManager.Instance.PlayerControllerInstance.PlayerMovementComponent;
            ShootingComponent ShootingComponent = GameManager.Instance.PlayerControllerInstance.PlayerShootingComponent;
            if (MovementComponent == null || ShootingComponent == null) yield break;
            
            CreateImage = false;
            Time.timeScale = 1f;
            MovementComponent.MoveSpeed = pre_MoveSpeed;
            ShootingComponent.FireRate = pre_FireRate;
        }

        IEnumerator CreateAfterImage() {
            GameObject TempObject = Instantiate(AfterImage, transform.position, PlayerBody.rotation);
            Destroy(TempObject, 0.1f);
            CreateImage = false;
            yield return new WaitForSecondsRealtime(0.075f);
            CreateImage = true;
        }
    }
}