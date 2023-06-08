using System.Collections;
using UnityEngine;
using Entities;
using Items.Executables;

namespace Items {
    public class Teleport : Executable {
        protected override void Awake() {
            Name = "Teleport";
            Description = "Teleport to the mouse cursor";
            Sprite = "Player/UI Images/Teleport";
            Duration = 0.01f;
            Cooldown = 15f;
            Upkeep = 0f;

            Usable = true;
            this.enabled = false;
        }

        void OnEnable() {
            OnCooldown = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 0b_0111_1011)) {
                if (raycastHit.collider.CompareTag("Room")) {
                    raycastHit.point = new Vector3(raycastHit.point.x, raycastHit.point.y + 1.5f, raycastHit.point.z);
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, raycastHit.point,
                        Vector3.Distance(raycastHit.point, gameObject.transform.position));
                }
            }
        }
    }
}
