using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Items.Threads.Constant.Obritals {
public class OrbitalCollider : MonoBehaviour {
        public delegate void OnOrbitalStayDelegate(Entity OverlappedEntity);

        public event OnOrbitalStayDelegate OnOrbitalStayOverlap;

        private void OnTriggerStay(Collider other) {
            Entity TempEntity = other.gameObject.GetComponent<Entity>();
            if (TempEntity && !other.gameObject.CompareTag("Indestructible") && OnOrbitalStayOverlap != null)
                OnOrbitalStayOverlap(TempEntity);
        }
    }
}
