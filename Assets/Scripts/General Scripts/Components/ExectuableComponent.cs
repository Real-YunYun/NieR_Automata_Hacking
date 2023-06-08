using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Entities;

namespace Items.Executables {
    [DefaultExecutionOrder(1)]
    public class ExectuableComponent : MonoBehaviour {
        [Header("Abillites")] 
        [SerializeField] public bool InItemPedstalVolume = false;
        public Entity Owner => transform.GetComponent<Entity>(); 
        [SerializeField] public Executable[] Executables = new Executable[4];

        private void Awake() {
            for (int i = 1; i < Executables.Length + 1; i++) AddExecutable<NoExecutable>(i);
        }

        #region Delegates and Events

        public delegate void OnExecutableAddedDelegate<TExecutable>(int slot);
        public delegate void OnExecutableChangedDelegate(ExectuableComponent Component, int slot);
        public delegate void OnExecutableUsedDelegate(int slot);

        public event OnExecutableAddedDelegate<Executable> OnExecutableAdded;
        public event OnExecutableChangedDelegate OnExecutableChanged;
        public event OnExecutableUsedDelegate OnExecutableUsed;

        protected void Execute_OnExecutableAdded<TExecutable>(int slot) {
            if (OnExecutableAdded != null) OnExecutableAdded(slot);
        }

        protected void Execute_OnExecutableUsed(int slot) {
            if (OnExecutableUsed != null) OnExecutableUsed(slot);
        }

        #endregion

        // {slot} is on a 1-based indexing instead of 0-based indexing
        public void AddExecutable<T>(int slot) where T : Executable {
            Destroy(Executables[slot - 1]);
            Executables[slot - 1] = gameObject.AddComponent<T>();
            if (OnExecutableAdded != null) OnExecutableAdded(slot - 1);
            Executables[slot - 1].enabled = false;
        }

        // {slot} is on a 1-based indexing instead of 0-based indexing
        public void AddExecutable(Type type, int slot) {
            Destroy(Executables[slot - 1]);
            Executables[slot - 1] = gameObject.AddComponent(type) as Executable;
            if (OnExecutableAdded != null) OnExecutableAdded(slot - 1);
            Executables[slot - 1].enabled = false;
        }

        // {slot} is on a 1-based indexing instead of 0-based indexing
        public void TryUseExecutable(int slot) {
            // Returns if it is on cooldown or in use
            if (Executables[slot - 1].OnCooldown || Executables[slot - 1].enabled) return;
            if (InItemPedstalVolume && OnExecutableUsed != null) {
                OnExecutableChanged(this, slot - 1);
                return;
            }
            
            if (!Executables[slot - 1].Usable) return;

            Executables[slot - 1].OnCooldown = true;
            Executables[slot - 1].enabled = true;
            if (OnExecutableUsed != null) OnExecutableUsed(slot - 1);
        }
    }
}
