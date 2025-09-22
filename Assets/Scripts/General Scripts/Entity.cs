using System;
using UnityEngine;
using JetBrains.Annotations;

using Entities.Status;
using Items.Executables;
using Items.Threads;

namespace Entities {
    
    [System.Serializable, DefaultExecutionOrder(-1)]
    public class Entity : MonoBehaviour {
        [Header("Entity Parameters")] 
        [Header("Components")] 
        
        [CanBeNull] public HealthComponent HealthComponent;
        [CanBeNull] public ExectuableComponent ExectuableComponent;
        [CanBeNull] public ThreadComponent ThreadComponent;
        [CanBeNull] public MovementComponent MovementComponent;
        [CanBeNull] public StatusComponent StatusComponent;
        [CanBeNull] public StatusComponent StatsComponent;

        private void Awake() {
            // All the try gets :\
            TryGetComponent(out HealthComponent);
            TryGetComponent(out ExectuableComponent);
            TryGetComponent(out ThreadComponent);
            TryGetComponent(out MovementComponent);
            TryGetComponent(out StatusComponent);
            TryGetComponent(out StatsComponent);
        }
    }
}
