using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overclock : Executable
{
    [Header("Overclock Parameter")]
    private GameObject AfterImage;
    GameObject[] Trail = new GameObject[10];
    private bool CreateImage = true;

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
        AfterImage = Resources.Load<GameObject>("Player/Player After Image");

        OnCooldown = true;
        Time.timeScale = 0.1f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed = 10f / Time.timeScale + 2f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate = 8f / Time.timeScale;
        StartCoroutine("Cooldown");
    }

    protected override void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            Stats.Upkeep += Time.unscaledDeltaTime;
            if (Stats.Upkeep >= Stats.Cooldown)
            {
                Stats.Upkeep = 0;
                OnCooldown = false;
                this.enabled = false;
            }
            if (Time.timeScale != 1 && CreateImage) StartCoroutine("CreateAfterImage");
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(Stats.Duration * Time.timeScale);
        CreateImage = false;
        Time.timeScale = 1f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().MoveSpeed = 10f;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().FireRate = 8f;
    }

    IEnumerator CreateAfterImage()
    {
        GameObject TempObject = Instantiate(AfterImage, GameManager.Instance.PlayerInstance.transform.position, GameManager.Instance.PlayerInstance.transform.Find("Player Mesh").transform.rotation);
        Destroy(TempObject, 0.1f);
        CreateImage = false;
        yield return new WaitForSecondsRealtime(0.1f);
        CreateImage = true;
    }
}
