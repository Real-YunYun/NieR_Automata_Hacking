using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overclock : Executable
{
    protected override void Awake()
    {
        Usable = true;
        Stats.Name = "Overclock";
        Stats.Description = "Slows down time and makes you move faster";
        Stats.Sprite = "Player/UI Images/None";
        Stats.Duration = 3f;
        Stats.Cooldown = 20f;
        Stats.Upkeep = 0f;
        this.enabled = false;
    }

    void OnEnable()
    {
        OnCooldown = true;
        Time.timeScale = 0.1f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed = 10f / Time.timeScale + 2f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate = 8f / Time.timeScale;
        StartCoroutine("Cooldown");
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(Stats.Duration * Time.timeScale);
        Time.timeScale = 1f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed = 10f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate = 8f;
    }
}
