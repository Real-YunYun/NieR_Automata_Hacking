using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Entities;

namespace Items.Threads {
    public class ThreadComponent : MonoBehaviour {
        public Entity Owner => transform.GetComponent<Entity>(); 
        public List<Thread> Threads = new();

        #region Delegates and Events

        public delegate void OnThreadAddedDelegate<TThread>();

        public delegate void OnThreadUsedDelegate();

        public event OnThreadAddedDelegate<Thread> OnThreadAdded;
        public event OnThreadUsedDelegate OnThreadUsed;

        protected void Execute_OnThreadAdded<TThread>()
        {
            if (OnThreadAdded != null) OnThreadAdded();
        }

        protected void Execute_OnThreadUsed()
        {
            if (OnThreadUsed != null) OnThreadUsed();
        }

        #endregion

        public void AddThread<T>() where T : Thread {
            if (Threads != null) Threads.Add(gameObject.AddComponent<T>());
            if (OnThreadAdded != null) OnThreadAdded();
        }
        
        public void AddThread(Type type) {
            Threads.Add(gameObject.AddComponent(type) as Thread);
            if (OnThreadAdded != null) OnThreadAdded();
        }
    }
}
