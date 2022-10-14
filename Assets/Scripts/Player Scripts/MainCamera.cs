using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Camera Parameters")]
    public Vector3 Position = new Vector3(0, 12, -10);
    public float SmoothTime = 0.25f;
    public Transform Player;
    private float VelocityX = 0.0f;
    private float VelocityY = 0.0f;
    private float VelocityZ = 0.0f;

    private bool ExitingGame = false;

    [Header("Interface Parameters")]
    private Canvas UI_Canvas;
    private GameObject UI_HUD_Canvas;
    private GameObject UI_Menu_Canvas;
    private GameObject UI_Load_Canvas;
    private GameObject UI_Exit_Canvas;
    private Image[] UI_Executables_Canvas = new Image[4];
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
        for (int i = 0; i < UI_Executables_Canvas.Length; i++) UI_Executables_Canvas[i] = transform.Find("UI Canvas/HUD Canvas/Executables/Executable " + (i + 1)).GetComponent<Image>();

        //Events and Delegates
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnExecutableUsedEvent += OnAbilityUsed;
        GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().OnExecutableAddedEvent += OnAbilityAdded;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            float NewX = Mathf.SmoothDamp(transform.position.x, Player.position.x + Position.x, ref VelocityX, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            float NewY = Mathf.SmoothDamp(transform.position.y, Player.position.y + Position.y, ref VelocityY, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            float NewZ = Mathf.SmoothDamp(transform.position.z, Player.position.z + Position.z, ref VelocityZ, SmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            transform.position = new Vector3(NewX, NewY, NewZ);
        }
    }

    void LateUpdate()
    {
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

            //Handling Bits Text
            UI_HUD_Canvas.transform.Find("Bits/Text").GetComponent<Text>().text = "Bits: " + GameManager.Instance.Data.Bits.ToString();

            //Handling Bounty Text
            if (GameManager.Instance.DirectorInstance)
            {
                UI_HUD_Canvas.transform.Find("Bounty").gameObject.SetActive(true);
                Bounty CurrentBounty = GameManager.Instance.DirectorInstance.GetComponent<Director>().GetBounty();
                string Objective = "";
                string TimeRemaining = "";

                if (CurrentBounty.Completed) UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().color = new Color32(73, 216, 24, 255);
                else if (CurrentBounty.Expired) UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().color = new Color32(216, 41, 24, 255);
                else UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().color = new Color32(50, 50, 50, 255);

                if (CurrentBounty.Type == BountyType.Kill) Objective = CurrentBounty.Type + ": " + CurrentBounty.Current + "/" + CurrentBounty.Target + " Entites\n";
                if (CurrentBounty.Type == BountyType.Earn) Objective = CurrentBounty.Type + ": " + CurrentBounty.Current + " Bits/" + CurrentBounty.Target + " Bits\n";

                if (!CurrentBounty.Expired && !CurrentBounty.Completed) TimeRemaining = "Time Remaining: " + (CurrentBounty.Duration - CurrentBounty.DeltaTime).ToString("n0") + "s\n";
                else if (CurrentBounty.Completed) TimeRemaining = "Completed Objective";
                else if (CurrentBounty.Expired) TimeRemaining = "Expired Objective";

                UI_HUD_Canvas.transform.Find("Bounty/Text").GetComponent<Text>().text =
                    Objective +
                    "Reward: " + CurrentBounty.Reward + " Bits\n" +
                    TimeRemaining;
            }
            else UI_HUD_Canvas.transform.Find("Bounty").gameObject.SetActive(false);

            //Handling Abilities 
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Executables[0].OnCooldown) UpdateAbility(0);
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Executables[1].OnCooldown) UpdateAbility(1);
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Executables[2].OnCooldown) UpdateAbility(2);
            if (GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Executables[3].OnCooldown) UpdateAbility(3);

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
        UI_Executables_Canvas[slot].transform.Find("Fill").GetComponent<Image>().fillAmount = 1;
    }

    private void UpdateAbility(int slot)
    {
        ExecutableStats stats = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Executables[slot].GetStats();
        UI_Executables_Canvas[slot].transform.Find("Fill").GetComponent<Image>().fillAmount = 1 - (stats.Upkeep / stats.Cooldown);
    }

    public void OnAbilityAdded(int slot)
    {
        UI_Executables_Canvas[slot].sprite = GameManager.Instance.PlayerInstance.GetComponent<PlayerController>().Executables[slot].GetSprite(slot);
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
