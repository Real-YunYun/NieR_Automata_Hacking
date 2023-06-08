using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Items {

    [Serializable]
    public class ItemTemplate {
        public string ComponentName = "None";
        public string InGameName = "None";
        public string Description = "";
    }

    [Serializable]
    public class ItemDictionary {
        public List<ItemTemplate> ItemPool = new List<ItemTemplate>();

        public void LoadDictionary() {
            File.ReadAllText(Application.dataPath + "/Dictionary/Items.dictionary");
        }

        public void SaveDictionary() {
            File.WriteAllText(Application.dataPath + "/Dictionary/Items.dictionary", JsonUtility.ToJson(this, true));
        }

        public void GetRandomItem() {
            
        }
    }

    public abstract class Item : MonoBehaviour {
        public string Name = "";
        public string Description = "";
        public string Sprite = "Player/UI Images/None";
        public float Duration = 0f;
        public float Cooldown = 0f;
        public float Upkeep = 0f;
        
        protected Entity Owner => transform.GetComponent<Entity>();
        
        protected virtual void Awake() {
            this.enabled = false;
        }
    };
}
