using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : Executable
{
    void Awake()
    {
        Usable = true;
        Stats.Name = "Teleport";
        Stats.Description = "Teleport to the mouse Cursor";
        Stats.Sprite = "Player/UI Images/Teleport";
        Stats.Duration = 0.01f;
        Stats.Cooldown = 15f;
        Stats.Upkeep = 0f;
        this.enabled = false;
    }

    void OnEnable()
    {
        OnCooldown = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, 0b_0000_0111))
        {
            transform.GetComponent<PlayerController>().GravityOn = false;
            raycastHit.point = new Vector3(raycastHit.point.x, raycastHit.point.y + 1.5f, raycastHit.point.z);
            this.gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, raycastHit.point, Vector3.Distance(raycastHit.point, gameObject.transform.position));
            StartCoroutine("TeleportCooldown");
        }
    }

    void Update()
    {
        Stats.Upkeep += Time.deltaTime;
        if (Stats.Upkeep >= Stats.Cooldown)
        {
            Stats.Upkeep = 0;
            OnCooldown = false;
            this.enabled = false;
        }
    }

    IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        transform.GetComponent<PlayerController>().GravityOn = true;
        yield return new WaitForSeconds(1f);
    }
}
