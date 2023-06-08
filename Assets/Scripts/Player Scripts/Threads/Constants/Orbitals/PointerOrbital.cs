using System;
using UnityEngine;
using Items.Threads.Constant.Obritals;

namespace Items {
    public class PointerOrbital : Orbital {
        #pragma warning disable
        protected void Awake() {
            string Name = "Pointer Orbital";
            string Description = "Summons a permanent Pointer Orbital!";
            string Sprite = "Player/UI Images/None";
            float Duration = 0f;
            float Cooldown = 0f;
            float Upkeep = 0f;
        }
        #pragma warning restore
    }
}
