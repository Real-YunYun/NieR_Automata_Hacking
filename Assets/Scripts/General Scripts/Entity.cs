using UnityEngine;
using JetBrains.Annotations;

using Items.Executables;
using Items.Threads;

namespace Entities {
    public class Entity : MonoBehaviour {
        [Header("Entity Parameters")] 
        [Header("Components")] 
        
        [CanBeNull] public HealthComponent HealthComponent;
        [CanBeNull] public ExectuableComponent ExectuableComponent;
        [CanBeNull] public ThreadComponent ThreadComponent;
        [CanBeNull] public MovementComponent MovementComponent;
    }
}
