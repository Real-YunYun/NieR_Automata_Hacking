using System;
using UnityEngine;

namespace Items {
    
    [Serializable]
    public struct Stats {
        public string Name;
        public string Description;
        public string Sprite;
        public float Duration;
        public float Cooldown;
        public float Upkeep;

        public Stats(string name = "None", string description = "None", string sprite = "Player/UI Images/None", float duration = 0f, float cooldown = 0f, float upkeep = 0f) {
            Name = name;
            Description = description;
            Sprite = sprite;
            Duration = duration;
            Cooldown = cooldown;
            Upkeep = upkeep;
        }
    }
    public abstract class Item : MonoBehaviour {
        [SerializeField] protected Stats Stats;
        public Stats GetStats() { return Stats; }
        
        protected virtual void Awake() {
            Stats.Name = "";
            Stats.Description = "";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;
            this.enabled = false;
        }
    };
}
