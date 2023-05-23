using System.Collections;
using UnityEngine;
using Items.Executables;

namespace Items {
    public class Overclock : Executable {
        [Header("Overclock Parameter")] private Transform PlayerBody;
        private GameObject AfterImage;
        GameObject[] Trail = new GameObject[10];
        private bool CreateImage = true;

        [Header("Previous Parameters")] private float pre_MoveSpeed = 0.0f;
        private float pre_FireRate = 0.0f;

        protected override void Awake() {
            Usable = true;
            Stats.Name = "Overclock";
            Stats.Description = "Slows down time and makes you move faster";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 3f;
            Stats.Cooldown = 20f;
            Stats.Upkeep = 0f;
            this.enabled = false;
        }

        void OnEnable() {
            AfterImage = Resources.Load<GameObject>("Player/Player Overclock Clone");
            PlayerBody = GameManager.Instance.PlayerInstance.transform.Find("Player Mesh").transform;

            OnCooldown = true;
            Time.timeScale = 0.1f;
            pre_MoveSpeed = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed;
            pre_FireRate = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate;

            GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed = 10f / Time.timeScale + 2f;
            GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate = 10f / Time.timeScale;
            StartCoroutine("Cooldown");
        }

        protected override void Update() {
            if (!GameManager.Instance.IsGamePaused) {
                Stats.Upkeep += Time.unscaledDeltaTime;
                if (Stats.Upkeep >= Stats.Cooldown) {
                    Stats.Upkeep = 0;
                    OnCooldown = false;
                    this.enabled = false;
                }

                if (Time.timeScale != 1 && CreateImage) StartCoroutine(CreateAfterImage());
            }
        }

        IEnumerator Cooldown() {
            yield return new WaitForSeconds(Stats.Duration * Time.timeScale);
            CreateImage = false;
            Time.timeScale = 1f;
            GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed = pre_MoveSpeed;
            GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate = pre_FireRate;
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