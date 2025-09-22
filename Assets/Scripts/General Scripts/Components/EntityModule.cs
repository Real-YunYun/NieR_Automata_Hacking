using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Modules {

    [DefaultExecutionOrder(-1)]
    public class EntityModule : MonoBehaviour {

        [Header("Module Parameters")]
        [SerializeField] private Entity _Owner;

        public Entity Owner => _Owner;

        protected void Awake() {
            if (!TryGetComponent(out _Owner)) {
                Debug.LogError("Failed to Initialize Owner of Component: \"" + gameObject.name + "\"");
                return;
            }
        }
    }
    
}