using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Camera Parameters")]
    public Vector3 Posistion = new Vector3(0, 10, -8);
    public float SmoothTime = 0.25f;
    public Transform Player;
    private Vector3 Velocity = Vector3.zero;

    private bool ExitingGame = false;

    [Header("Interface Parameters")]
    private Canvas UI_Canvas;
    private GameObject UI_HUD_Canvas;
    private GameObject UI_Menu_Canvas;
    private GameObject UI_Load_Canvas;
    private GameObject UI_Exit_Canvas;
    private Image[] UI_Abilities_Canvas = new Image[4];
    [HideInInspector] public GameObject InteractText;

    void Awake()
    {
        //Getting UI Children
        UI_Canvas = transform.Find("UI Canvas").GetComponent<Canvas>();
        InteractText = transform.Find("UI Canvas/InteractText").gameObject;
        UI_HUD_Canvas = transform.Find("UI Canvas/HUD Canvas").gameObject;
        UI_Menu_Canvas = transform.Find("UI Canvas/Menu Canvas").gameObject;
        UI_Load_Canvas = transform.Find("UI Canvas/Menu Canvas/Load Game Canvas").gameObject;
        UI_Exit_Canvas = transform.Find("UI Canvas/Menu Canvas/Exit Canvas").gameObject;
        for (int i = 0; i < UI_Abilities_Canvas.Length; i++) UI_Abilities_Canvas[i] = transform.Find("UI Canvas/HUD Canvas/Abilities/Ability " + (i + 1)).GetComponent<Image>();

        //Events and Delegates
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnAbilityUsedEvent += OnAbilityUsed;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnAbilityAddedEvent += OnAbilityAdded;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, Player.position + Posistion, ref Velocity, SmoothTime);
        HandleUI();
    }

    public void HandleUI(GameState gameState = GameState.Title)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Title)
        {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(false);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(false);
        }
        if (GameManager.Instance.CurrentGameState == GameState.Pause)
        {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(true);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(false);

            //Handling UI Buttons
            transform.Find("UI Canvas/Menu Canvas/Parent/Save Game Button").GetComponent<Button>().interactable = GameManager.Instance.CurrentLevel == "HUB";
            transform.Find("UI Canvas/Menu Canvas/Parent/Exit to Lobby Button").GetComponent<Button>().interactable = GameManager.Instance.CurrentLevel != "HUB";
        }
        if (GameManager.Instance.CurrentGameState == GameState.Load)
        {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(true);
            UI_Load_Canvas.SetActive(true);
            UI_Exit_Canvas.SetActive(false);
        }
        if (GameManager.Instance.CurrentGameState == GameState.Playing)
        {
            UI_HUD_Canvas.SetActive(true);
            UI_Menu_Canvas.SetActive(false);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(false);

            //Handling Health Bar
            string Health = "";
            for (int i = 1; i <= Player.gameObject.GetComponent<PlayerController>().Health; i++) Health += "/";
            UI_HUD_Canvas.transform.Find("Bars/Health Bar").GetComponent<Text>().text = Health;

            //Handling Energy Bar
            string Energy = "";
            for (int i = 1; i <= Player.gameObject.GetComponent<PlayerController>().Energy; i++) Energy += "/";
            UI_HUD_Canvas.transform.Find("Bars/Energy Bar").GetComponent<Text>().text = Energy;

            //Handling Bits Bars
            UI_HUD_Canvas.transform.Find("Bits/Text").GetComponent<Text>().text = "Bits: " + GameManager.Instance.Data.Bits.ToString();

            //Handling Abilities 
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Abilities[0].OnCooldown) UpdateAbility(0);
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Abilities[1].OnCooldown) UpdateAbility(1);
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Abilities[2].OnCooldown) UpdateAbility(2);
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Abilities[3].OnCooldown) UpdateAbility(3);

        }
        if (GameManager.Instance.CurrentGameState == GameState.Exiting)
        {
            UI_HUD_Canvas.SetActive(false);
            UI_Menu_Canvas.SetActive(true);
            UI_Load_Canvas.SetActive(false);
            UI_Exit_Canvas.SetActive(true);
        }
    }

    private void OnAbilityUsed(int slot)
    {
        UI_Abilities_Canvas[slot].transform.Find("Fill").GetComponent<Image>().fillAmount = 1;
    }

    private void UpdateAbility(int slot)
    {
        AbilityStats stats = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Abilities[slot].GetStats();
        UI_Abilities_Canvas[slot].transform.Find("Fill").GetComponent<Image>().fillAmount = 1 - (stats.Upkeep / stats.Cooldown);
    }

    public void OnAbilityAdded(int slot)
    {
       UI_Abilities_Canvas[slot].sprite = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Abilities[slot].GetSprite(slot);
    }

    public void Resume()
    {
        GameManager.Instance.PauseGame();
    }

    //Saving Handle
    public void SaveGame()
    {
        GameManager.Instance.SaveGame();
    }

    //Loading Handles
    public void PromptLoadGame()
    {
        GameManager.Instance.CurrentGameState = GameState.Load;
    }

    public void LoadGame(bool state)
    {
        if (state) GameManager.Instance.LoadGame();
        else GameManager.Instance.CurrentGameState = GameState.Pause;
    }

    //Exiting Game Handles
    public void PromptExit(bool ExitGame = true)
    {
        if (ExitGame) ExitingGame = true;
        else ExitingGame = false;
        GameManager.Instance.CurrentGameState = GameState.Exiting;
    }

    public void ExitTo(bool state)
    {
        if (state && !ExitingGame)
        {
            GameManager.Instance.PauseGame();
            GameManager.Instance.LoadGame();
            GameManager.Instance.CurrentGameState = GameState.Playing;
            SceneManager.LoadScene("HUB");
        }
        else if (state && ExitingGame) GameManager.Instance.ExitGame();
        else GameManager.Instance.CurrentGameState = GameState.Pause;
    }
}
