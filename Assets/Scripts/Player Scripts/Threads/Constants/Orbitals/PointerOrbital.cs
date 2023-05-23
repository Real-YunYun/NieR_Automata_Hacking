using System;
using UnityEngine;
using Items.Threads.Constant.Obritals;

namespace Items {
    public class PointerOrbital : Orbital {
        protected override void Awake() {
            Stats.Name = "Pointer Orbital!";
            Stats.Description = "Summons a permanent Pointer Orbital!";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;
        }
    }
}
