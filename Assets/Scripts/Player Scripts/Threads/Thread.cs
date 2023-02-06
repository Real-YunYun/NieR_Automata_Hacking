using System;
using UnityEngine;

namespace Threads
{
    public enum EThreadBehaviour
    {
        EventBased,     // OnHit Events, OnFire Events, etc
        Conditional,    // Standing Still for X seconds, then it activates
        Periodic,       // After a (conditional) period of time, they activate
        Instantaneous,  // Movement speed increase, Health increase, etc
        Constant,       // Constantly Active
    }

    [Serializable]
    public struct ThreadStats
    {
        public string Name;
        public string Description;
        public string Sprite;
        public float Duration;
        public float Cooldown;
        public float Upkeep;

        public ThreadStats(string name = "None", string description = "None", string sprite = "Player/UI Images/None", float duration = 0f, float cooldown = 0f, float upkeep = 0f)
        {
            Name = name;
            Description = description;
            Sprite = sprite;
            Duration = duration;
            Cooldown = cooldown;
            Upkeep = upkeep;
        }
    }

    [Serializable, DefaultExecutionOrder(2)]
    public abstract class Thread : MonoBehaviour
    {
        [Header("Thread Parameters")]
        [SerializeField] protected ThreadStats Stats;
        protected EThreadBehaviour Type = EThreadBehaviour.EventBased;

        /// Event Based Parameters
        public delegate void OnThreadStartedDelegate();
        public delegate void OnThreadEndedDelegate();
        public event OnThreadStartedDelegate OnThreadStarted;
        public event OnThreadEndedDelegate OnThreadEnded;
 
        
        public ThreadStats GetStats() { return Stats; }
        
        protected virtual void Awake()
        {
            Stats.Name = "None";
            Stats.Description = "";
            Stats.Sprite = "Player/UI Images/None";
            Stats.Duration = 0f;
            Stats.Cooldown = 0f;
            Stats.Upkeep = 0f;
        }
    }
}
