using System;
using UnityEngine;

namespace Items.Executables {
    [Serializable, DefaultExecutionOrder(2)]
    public abstract class Executable : Item {
        [Header("Executable Fields")]
        public bool OnCooldown = false;
        public bool Usable = true;

        protected new virtual void Awake() {
            Name = "None";
            Description = "";
            Sprite = "Player/UI Images/None";
            Duration = 0f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Usable = false;
            this.enabled = false;
        }

        protected virtual void Update() {
            if (GameManager.Instance.IsGamePaused) return;
            
            Upkeep += Time.unscaledDeltaTime;
            if (!(Upkeep >= Cooldown)) return;
            
            Upkeep = 0;
            OnCooldown = false;
            this.enabled = false;
        }

        public Sprite GetSprite() {
            return Resources.Load<Sprite>(Sprite);
        }
    }
}
