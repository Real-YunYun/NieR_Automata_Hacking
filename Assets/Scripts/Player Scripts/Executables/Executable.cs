using UnityEngine;
using UnityEngine.UI;

namespace Items.Executables {
    [System.Serializable, DefaultExecutionOrder(2)]
    public abstract class Executable : Item
    {
        [Header("Executable Fields")]
        public bool OnCooldown = false;
        public bool Usable = true;

        protected new virtual void Awake() {
            Usable = false;
            Stats.Name = "";
            Stats.Description = "";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;
            this.enabled = false;
        }

        protected virtual void Update() {
            if (GameManager.Instance.IsGamePaused) return;
            
            Stats.Upkeep += Time.unscaledDeltaTime;
            if (!(Stats.Upkeep >= Stats.Cooldown)) return;
            
            Stats.Upkeep = 0;
            OnCooldown = false;
            this.enabled = false;
        }

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(Stats.Sprite);
        }
    }
}
