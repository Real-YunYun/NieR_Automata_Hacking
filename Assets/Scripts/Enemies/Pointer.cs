using System.Collections;
using System.Collections.Generic;
using Entities.Projectiles;
using UnityEngine;

namespace Entities.Enemies {
    public class Pointer : Enemy {
        [Header("Pointer Parameters")] [SerializeField]
        private Transform OrbTransform;
        private Transform[] Orbs = new Transform[4];
        private ShootingComponent[] OrbShootingComponents = new ShootingComponent[4];

        void Awake() {
            OrbTransform = transform.Find("Cylinder/Orbs");
            
            for (int i = 0; i < OrbTransform.childCount; i++) 
                Orbs[i] = OrbTransform.GetChild(i).transform;
            
            for (var i = 0; i < OrbTransform.childCount; i++) {
                ShootingComponent OrbShootingComponent;
                if (Orbs[i].TryGetComponent(out OrbShootingComponent)) {
                    OrbShootingComponents[i] = OrbShootingComponent;
                    OrbShootingComponent.SetInstigator(this);
                    OrbShootingComponent.InvokeRepeating(nameof(OrbShootingComponent.Fire), 0f, 1f / OrbShootingComponent.FireRate);
                }
            }
            
            Invoke(nameof(FireAllOrbs), 0.01f);
        }

        // Update is called once per frame
        void Update() {
            OrbTransform.Rotate(new Vector3(0, 100 * Time.deltaTime, 0), Space.Self);
        }

        private void FireAllOrbs() {
            if (OrbShootingComponents[1]) OrbShootingComponents[1].Fire();
            if (OrbShootingComponents[2]) OrbShootingComponents[2].Fire();
            if (OrbShootingComponents[3]) OrbShootingComponents[3].Fire();
        }
    }
}
