using System.Collections;
using UnityEngine;
using Items.Executables;

namespace Items {
    public class Teleport : Executable {
        protected override void Awake() {
            Usable = true;
            Stats.Name = "Teleport";
            Stats.Description = "Teleport to the mouse Cursor";
            Stats.Sprite = "Player/UI Images/Teleport";
            Stats.Duration = 0.01f;
            Stats.Cooldown = 15f;
            Stats.Upkeep = 0f;
            this.enabled = false;
        }

        void OnEnable() {
            OnCooldown = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 0b_0111_1011)) {
                if (raycastHit.collider.CompareTag("Room")) {
                    transform.GetComponent<PlayerController>().GravityOn = false;
                    raycastHit.point = new Vector3(raycastHit.point.x, raycastHit.point.y + 1.5f, raycastHit.point.z);
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, raycastHit.point,
                        Vector3.Distance(raycastHit.point, gameObject.transform.position));
                    StartCoroutine("TeleportCooldown");
                }
            }
        }

        IEnumerator TeleportCooldown() {
            yield return new WaitForSeconds(0.1f);
            transform.GetComponent<PlayerController>().GravityOn = true;
            yield return new WaitForSeconds(1f);
        }
    }
}
