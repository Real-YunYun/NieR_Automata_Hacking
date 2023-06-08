using UnityEngine;
using Items.Executables;

namespace Items {
    public class NoExecutable : Executable {
        #pragma warning disable
        protected void Awake() {
            string Name = "None";
            string Description = "";
            string Sprite = "Player/UI Images/None";
            float Duration = 0f;
            float Cooldown = 0f;
            float Upkeep = 0f;

            bool Usable = false;
        }
        #pragma warning restore
    }
}